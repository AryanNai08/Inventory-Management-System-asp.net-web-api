import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private endpoints = API_CONFIG.ENDPOINTS.CATEGORIES;

  constructor(private http: HttpClient) {}

  getAllCategories(): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(`${this.apiUrl}${this.endpoints.GET_ALL}`, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No categories found', 
            data: [] 
          } as APIResponse<any[]>);
        }
        return throwError(() => err);
      })
    );
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
