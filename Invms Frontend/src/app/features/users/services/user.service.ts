import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.USERS.BASE}`;
  private http = inject(HttpClient);

  getAll(): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(`${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.USERS.GET_ALL}`);
  }

  getById(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}/${id}`);
  }

  getMe(): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.USERS.ME}`);
  }

  update(id: number, dto: any): Observable<APIResponse<any>> {
    return this.http.put<APIResponse<any>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
