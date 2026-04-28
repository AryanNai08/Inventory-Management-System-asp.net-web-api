import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn, HttpEvent, HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ToastService } from '../services/toast';
import { AuthService } from '../../features/auth/services/auth';
import { API_CONFIG } from '../../shared/config/api.config';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const toastService = inject(ToastService);
  
  // SECURE COOKIE APPROACH: 
  // We no longer read tokens from LocalStorage or attach Authorization headers.
  // Instead, we force withCredentials: true so the browser automatically sends HttpOnly cookies.
  req = req.clone({
    withCredentials: true
  });

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // 401 Unauthorized Logic: Silent Refresh
      if (error.status === 401) {
        const isAuthRequest = req.url.includes(API_CONFIG.ENDPOINTS.AUTH.LOGIN) || 
                             req.url.includes(API_CONFIG.ENDPOINTS.AUTH.REFRESH);
        
        if (!isAuthRequest) {
          // Attempt silent refresh via the refresh-token cookie
          return authService.handle401(req, next);
        }
      } 
      
      // Generic Error Handling
      if (error.status === 403) {
        toastService.error('Access Denied', 'You do not have permission to perform this action.');
      } else if (error.status === 0) {
        toastService.error('Connection Error', 'Unable to connect to the server. Please check your internet.');
      }
      
      return throwError(() => error);
    })
  );
};
