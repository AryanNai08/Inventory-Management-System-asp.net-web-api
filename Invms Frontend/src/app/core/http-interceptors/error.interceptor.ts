import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn, HttpEvent, HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastService } from '../services/toast';

export const errorInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const router = inject(Router);
  const toastService = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unknown error occurred';
      
      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = error.error.message;
      } else {
        // Server-side error
        errorMessage = error.error?.message || error.error?.error || error.message;
      }

      if (error.status === 401) {
        if (!req.url.includes('auth/login')) {
          toastService.error('Unauthorized', 'Your session has expired. Please login again.');
          router.navigate(['/auth/login']);
        }
      } else if (error.status === 403) {
        toastService.error('Forbidden', 'You do not have permission to perform this action.');
      } else if (error.status === 0) {
        toastService.error('Network Error', 'Cannot connect to the server. Please check your connection.');
      } else {
        toastService.error('Error', errorMessage);
      }

      return throwError(() => error);
    })
  );
};
