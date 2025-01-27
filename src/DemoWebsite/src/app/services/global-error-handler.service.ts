import { ErrorHandler, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GlobalErrorHandlerService implements ErrorHandler {
  handleError(error: any): void {
    // Log the error or send it to a server
    console.error('An unexpected error occurred:', error);
    // Optionally, display a user-friendly message
  }
}
