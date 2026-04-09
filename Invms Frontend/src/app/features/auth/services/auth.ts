import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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

  constructor(
    private http: HttpClient, 
    private storageService: StorageService
  ) {}

  login(dto: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.AUTH.LOGIN}`, 
      dto,
      { withCredentials: true } // Crucial for cookie-based auth
    ).pipe(
      map(response => {
        // Handle both Status (Backend) and status (CamelCase)
        const isSuccess = response.status;
        if (isSuccess && response.data && response.data.token) {
          this.storageService.saveToken(response.data.token);
          this.storageService.saveUser(response.data);
        }
        return response;
      })
    );
  }

  logout(): void {
    this.storageService.clean();
    this.router.navigate(['/auth/login']);
  }

  isLoggedIn(): boolean {
    const token = this.storageService.getToken();
    return !!token;
  }
}
