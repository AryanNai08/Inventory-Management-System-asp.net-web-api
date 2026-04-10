import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private endpoints = API_CONFIG.ENDPOINTS.CATEGORIES;

  constructor(private http: HttpClient) {}

  getAllCategories(): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(`${this.apiUrl}${this.endpoints.GET_ALL}`);
  }

  getCategoryById(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}${this.endpoints.BY_ID(id)}`);
  }

  createCategory(data: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}${this.endpoints.CREATE}`, data);
  }

  updateCategory(id: number, data: any): Observable<APIResponse<any>> {
    return this.http.put<APIResponse<any>>(`${this.apiUrl}${this.endpoints.UPDATE(id)}`, data);
  }

  deleteCategory(id: number): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}${this.endpoints.DELETE(id)}`);
  }
}
