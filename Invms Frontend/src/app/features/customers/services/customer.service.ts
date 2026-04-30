import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpContext } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse, PaginatedResult, PaginationParams } from '../../../core/models/api.model';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private endpoints = API_CONFIG.ENDPOINTS.CUSTOMERS;

  constructor(private http: HttpClient) {}

  getAllCustomers(params: PaginationParams): Observable<APIResponse<PaginatedResult<any>>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.sortColumn) {
      httpParams = httpParams.set('sortColumn', params.sortColumn);
    }
    if (params.sortOrder) {
      httpParams = httpParams.set('sortOrder', params.sortOrder);
    }
    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }

    return this.http.get<APIResponse<PaginatedResult<any>>>(`${this.apiUrl}${this.endpoints.GET_ALL}`, { 
      params: httpParams,
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No customers found', 
            data: { 
              items: [], 
              totalCount: 0,
              pageNumber: 1,
              pageSize: 10,
              totalPages: 0,
              hasNextPage: false,
              hasPreviousPage: false
            } 
          } as APIResponse<PaginatedResult<any>>);
        }
        return throwError(() => err);
      })
    );
  }

  getCustomerById(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}${this.endpoints.BY_ID(id)}`);
  }

  createCustomer(data: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}${this.endpoints.CREATE}`, data);
  }

  updateCustomer(id: number, data: any): Observable<APIResponse<any>> {
    return this.http.put<APIResponse<any>>(`${this.apiUrl}${this.endpoints.UPDATE(id)}`, data);
  }

  deleteCustomer(id: number): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}${this.endpoints.DELETE(id)}`);
  }
}
