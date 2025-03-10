use axum::{
  body::Body,
  http::{Request, StatusCode},
  middleware::Next,
  response::{IntoResponse, Response},
};

use crate::utils::api_error::APIError;

pub async fn global_err_handler<B>(req: Request<B>, next: Next) -> Response
where
  B: Send + 'static,
  Body: From<B>,
{
  let req = req.map(Body::from);

  let response = next.run(req).await;

  if response.status().is_server_error() {
    let error_response = APIError {
      message: "Internal Error Server".to_string(),
      status_code: StatusCode::INTERNAL_SERVER_ERROR,
      error_code: Some(50),
    };
    return error_response.into_response();
  }
  response
}
// Strcuture APIErrror
use axum::{Json, http::StatusCode, response::IntoResponse};
use serde_json::json;

#[derive(Debug)]
pub struct APIError {
  pub message: String,
  pub status_code: StatusCode,
  pub error_code: Option<i8>,
}

impl IntoResponse for APIError {
  fn into_response(self) -> axum::response::Response {
    let status_code = self.status_code;
    (
      status_code,
      Json(json!({
        "status":"error",
        "statusCode":status_code.as_u16(),
        "errorCode":self.error_code,
        "message":self.message
      })),
    )
      .into_response()
  }
}
//Implementation 
use crate::middleware::global_filter_exception::global_err_handler;
use crate::middleware::verify_time::verify_time_middleware;
use axum::middleware;
use axum::{
  Router,
  routing::{delete, get, patch, post},
};

pub fn initial_router() -> Router {
  Router::new()
    .route("/list", get("Hallo World"))
    .layer(middleware::from_fn(global_err_handler))
}

