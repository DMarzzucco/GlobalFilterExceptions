package personmanager.com.demo.Exception;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.context.request.WebRequest;
import org.springframework.web.servlet.mvc.method.annotation.ResponseEntityExceptionHandler;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.Map;

@ControllerAdvice
public class GlobalExceptionHandler extends ResponseEntityExceptionHandler {
    private static final Logger logger = LoggerFactory.getLogger(GlobalExceptionHandler.class);

    @ExceptionHandler(Exception.class)
    public ResponseEntity<Object> handleGlobalException(Exception ex, WebRequest request) {
        logger.error("Error occurred: {}", ex.getMessage(), ex);

        StackTraceElement[] stackTrace = ex.getStackTrace();
        String fileName = (stackTrace.length > 0) ? stackTrace[0].getFileName() : "Unknown";
        int lineNumber = (stackTrace.length > 0) ? stackTrace[0].getLineNumber() : -1;

        HttpStatus status = switch (ex) {
            case IllegalArgumentException ignored -> HttpStatus.BAD_REQUEST;   // 400
            case NullPointerException ignored -> HttpStatus.INTERNAL_SERVER_ERROR; // 500
            case SecurityException ignored -> HttpStatus.UNAUTHORIZED;        // 401
            case IllegalStateException ignored -> HttpStatus.CONFLICT;        // 409
            case RuntimeException ignored -> HttpStatus.INTERNAL_SERVER_ERROR; // 500
            // you can add more http status code
            default -> HttpStatus.INTERNAL_SERVER_ERROR;
        };
        
        Map<String, Object> body = new HashMap<>();
        body.put("timestamp", LocalDateTime.now());
        body.put("status", status.value());
        body.put("error",  status.getReasonPhrase());
        body.put("message", ex.getMessage());
        body.put("cause", ex.getCause() != null ? ex.getCause().getMessage() : "N/A");
        body.put("file", fileName);
        body.put("line", lineNumber);
        body.put("path", request.getDescription(false));

        return new ResponseEntity<>(body, status);
    }
}
