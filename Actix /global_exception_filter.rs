use actix_web::{middleware, web, App, HttpServer, HttpResponse, ResponseError};
use serde::Serialize;
use std::fmt;
use std::error::Error;
use log::error;

#[derive(Debug, Serialize)]
struct ErrorResponse {
    status_code: u16,
    message: String,
    file: Option<String>,
    line: Option<u32>,
}

#[derive(Debug)]
pub struct CustomError {
    pub status_code: u16,
    pub message: String,
    pub source: Option<Box<dyn Error + Send + Sync>>,
    pub file: Option<&'static str>,
    pub line: Option<u32>,
}

impl fmt::Display for CustomError {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.message)
    }
}

impl Error for CustomError {}

impl ResponseError for CustomError {
    fn error_response(&self) -> HttpResponse {
        error!("Unhandled exception: {}", self.message);
        HttpResponse::build(actix_web::http::StatusCode::from_u16(self.status_code).unwrap())
            .json(ErrorResponse {
                status_code: self.status_code,
                message: self.message.clone(),
                file: self.file.map(String::from),
                line: self.line,
            })
    }
}

macro_rules! custom_error {
    ($status_code:expr, $msg:expr) => {
        CustomError {
            status_code: $status_code,
            message: $msg.to_string(),
            source: None,
            file: Some(file!()),
            line: Some(line!()),
        }
    };
}

async fn error_handler() -> Result<HttpResponse, CustomError> {
    Err(custom_error!(500, "Something went wrong"))
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    env_logger::init();
    HttpServer::new(|| {
        App::new()
            .wrap(middleware::Logger::default())
            .route("/error", web::get().to(error_handler))
    })
    .bind(("127.0.0.1", 8080))?
    .run()
    .await
}
