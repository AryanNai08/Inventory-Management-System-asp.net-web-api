import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';
import { DashboardSummaryDto, LowStockDto, TopProductDto } from '../models/dashboard.model';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private http: HttpClient) { }

  getSummary(): Observable<APIResponse<DashboardSummaryDto>> {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.DASHBOARD.SUMMARY}`;
    return this.http.get<APIResponse<DashboardSummaryDto>>(url, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'Dashboard data not found', 
            data: { totalProducts: 0, totalCategories: 0, totalSuppliers: 0, totalOrders: 0, totalSales: 0, lowStockCount: 0 } as any 
          } as APIResponse<DashboardSummaryDto>);
        }
        return throwError(() => err);
      })
    );
  }

  getTopSelling(count: number = 5): Observable<APIResponse<TopProductDto[]>> {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.DASHBOARD.TOP_SELLING}?count=${count}`;
    return this.http.get<APIResponse<TopProductDto[]>>(url, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ status: true, statusCode: 200, message: 'No top products', data: [] } as APIResponse<TopProductDto[]>);
        }
        return throwError(() => err);
      })
    );
  }

  getLowStock(): Observable<APIResponse<LowStockDto[]>> {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.DASHBOARD.LOW_STOCK}`;
    return this.http.get<APIResponse<LowStockDto[]>>(url, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ status: true, statusCode: 200, message: 'No low stock', data: [] } as APIResponse<LowStockDto[]>);
        }
        return throwError(() => err);
      })
    );
  }
}
