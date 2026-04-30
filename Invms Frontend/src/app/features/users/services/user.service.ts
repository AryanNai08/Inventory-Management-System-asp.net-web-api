import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  
  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.USERS.GET_ALL}`,
      { context: new HttpContext().set(BYPASS_ERROR_TOAST, true) }
    ).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No users found', 
            data: [] 
          } as APIResponse<any[]>);
        }
        return throwError(() => err);
      })
    );
  }

  getUserById(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.USERS.BY_ID(id)}`
    );
  }

  getMyProfile(): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.USERS.ME}`
    );
  }

  updateUser(id: number, dto: any): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.USERS.UPDATE(id)}`,
      dto
    );
  }

  deleteUser(id: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.USERS.DELETE(id)}`
    );
  }
}
