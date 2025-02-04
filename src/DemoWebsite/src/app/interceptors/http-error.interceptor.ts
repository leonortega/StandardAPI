import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private notificationService: NotificationService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unknown error occurred';
        let errorDetails = '';

        if (error.error instanceof ErrorEvent) {
          // Client-side error (e.g., network issue, timeout, or browser error)
          errorMessage = `Client-side error: ${error.error.message}`;
        } else if (error.status === 0) {
          // Server is unreachable (e.g., CORS error, no internet, DNS failure)
          errorMessage = `Network error: Unable to reach the server: ${req.url}`;
          errorDetails = `
            Possible causes:
            - No internet connection
            - API server might be down
            - CORS policy blocking the request
            - DNS resolution failed
          `;
        } else {
          // Server-side error
          errorMessage = `Server-side error: ${error.status} - ${error.statusText}`;
          errorDetails = `
            - URL: ${req.url}
            - Method: ${req.method}
            - Status: ${error.status}
            - Message: ${error.message}
            - Response: ${JSON.stringify(error.error)}
          `;
        }

        // Log the error for debugging
        console.error(errorMessage, errorDetails);

        // Notify the user
        this.notificationService.showError(`${errorMessage}\n\n${errorDetails}`);

        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
