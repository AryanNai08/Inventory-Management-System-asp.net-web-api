import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of, throwError } from 'rxjs';
import { map, tap, catchError, switchMap, filter, take } from 'rxjs/operators';
import { Router } from '@angular/router';
import { API_CONFIG } from '../../../shared/config/api.config';
import { StorageService } from '../../../core/services/storage.service';
import { APIResponse } from '../../../core/models/api.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private router = inject(Router);
  
  // Refresh Token state
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(
    private http: HttpClient, 
    private storageService: StorageService
  ) {}

  login(dto: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.LOGIN}`, 
      dto,
      { withCredentials: true }
    ).pipe(
      map(response => {
        if (response.status && response.data?.token) {
          this.storageService.saveToken(response.data.token);
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
   * Performs the actual Refresh Token API call
   */
  refreshToken(): Observable<APIResponse<any>> {
    const user = this.storageService.getUser();
    const refreshToken = user?.refreshToken;

    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.REFRESH}`,
      { refreshToken },
      { withCredentials: true }
    ).pipe(
      tap(response => {
        if (response.status && response.data?.token) {
          this.storageService.saveToken(response.data.token);
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
          if (res.status && res.data?.token) {
            this.refreshTokenSubject.next(res.data.token);
            return next(this.addToken(req, res.data.token));
          }
          this.logout();
          return throwError(() => new Error('Refresh failed'));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.logout();
          return throwError(() => err);
        })
      );
    } else {
      // If already refreshing, wait for the new token
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => next(this.addToken(req, token!)))
      );
    }
  }

  private addToken(request: any, token: string) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
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
    const isClearing = this.storageService.getToken();
    if (!isClearing) return;

    this.http.post(`${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.LOGOUT}`, {})
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
    return !!this.storageService.getToken();
  }
}
