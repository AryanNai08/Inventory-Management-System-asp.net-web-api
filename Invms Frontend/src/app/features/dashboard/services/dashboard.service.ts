import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';
import { DashboardSummaryDto, LowStockDto, TopProductDto } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private http: HttpClient) { }

  getSummary(): Observable<APIResponse<DashboardSummaryDto>> {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.DASHBOARD.SUMMARY}`;
    return this.http.get<APIResponse<DashboardSummaryDto>>(url);
  }

  getTopSelling(count: number = 5): Observable<APIResponse<TopProductDto[]>> {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.DASHBOARD.TOP_SELLING}?count=${count}`;
    return this.http.get<APIResponse<TopProductDto[]>>(url);
  }

  getLowStock(): Observable<APIResponse<LowStockDto[]>> {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.DASHBOARD.LOW_STOCK}`;
    return this.http.get<APIResponse<LowStockDto[]>>(url);
  }
}
