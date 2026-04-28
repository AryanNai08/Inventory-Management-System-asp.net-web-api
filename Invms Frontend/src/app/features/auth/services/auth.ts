import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of, throwError } from 'rxjs';
import { map, tap, catchError, switchMap, filter, take } from 'rxjs/operators';
import { Router } from '@angular/router';
import { API_CONFIG } from '../../../shared/config/api.config';
import { StorageService } from '../../../core/services/storage.service';
import { APIResponse } from '../../../core/models/api.model';
import { ToastService } from '../../../core/services/toast';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private router = inject(Router);
  
  // Refresh Token state
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<boolean | null> = new BehaviorSubject<boolean | null>(null);

  constructor(
    private http: HttpClient, 
    private storageService: StorageService,
    private toastService: ToastService
  ) {}

  login(dto: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.LOGIN}`, 
      dto,
      { withCredentials: true }
    ).pipe(
      map(response => {
        if (response.status) {
          // We only save the user identity/permissions.
          // The JWT is securely stored in an HttpOnly cookie by the browser.
          this.storageService.saveUser(response.data);
        }
        return response;
      })
    );
  }

  register(dto: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.REGISTER}`, 
      dto
    );
  }

  /**
   * Performs the actual Refresh Token API call.
   * Note: We no longer send the refresh token in the body; 
   * the backend reads it from the HttpOnly cookie.
   */
  refreshToken(): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.REFRESH}`,
      {}, // Empty body
      { withCredentials: true }
    ).pipe(
      tap(response => {
        if (response.status) {
          this.storageService.saveUser(response.data);
        }
      })
    );
  }

  /**
   * Helper for the Interceptor to handle the refresh logic centrally
   */
  handle401(req: any, next: any): Observable<any> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.refreshToken().pipe(
        switchMap((res: any) => {
          this.isRefreshing = false;
          if (res.status) {
            this.refreshTokenSubject.next(true);
            // Re-run the original request. The browser will automatically 
            // include the new cookie thanks to withCredentials: true in the interceptor.
            return next(req.clone({ withCredentials: true }));
          }
          this.toastService.error('Session Expired', 'Please login again to continue.');
          this.logout();
          return throwError(() => new Error('Refresh failed'));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.toastService.error('Session Expired', 'Please login again to continue.');
          this.logout();
          return throwError(() => err);
        })
      );
    } else {
      // If already refreshing, wait for completion
      return this.refreshTokenSubject.pipe(
        filter(result => result !== null),
        take(1),
        switchMap(() => next(req.clone({ withCredentials: true })))
      );
    }
  }

  forgotPassword(email: string): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.FORGOT_PASSWORD}`,
      { email }
    );
  }

  resetPassword(dto: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.RESET_PASSWORD}`,
      dto
    );
  }

  changePassword(dto: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.CHANGE_PASSWORD}`,
      dto
    );
  }

  logout(): void {
    const user = this.storageService.getUser();
    if (!user) return;

    this.http.post(`${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.LOGOUT}`, {}, { withCredentials: true })
      .subscribe({
        next: () => this.finalizeLogout(),
        error: () => this.finalizeLogout()
      });
  }

  private finalizeLogout(): void {
    this.storageService.clean();
    this.router.navigate(['/auth/login']);
  }

  isLoggedIn(): boolean {
    return this.storageService.isLoggedIn();
  }
}
