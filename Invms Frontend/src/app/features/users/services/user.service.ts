import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  
  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(
      `${this.apiUrl}${API_CONFIG.ENDPOINTS.USERS.GET_ALL}`
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
