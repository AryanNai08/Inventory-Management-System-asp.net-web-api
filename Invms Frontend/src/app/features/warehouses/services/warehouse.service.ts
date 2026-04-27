import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';

@Injectable({
  providedIn: 'root'
})
export class WarehouseService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private endpoints = API_CONFIG.ENDPOINTS.WAREHOUSES;

  constructor(private http: HttpClient) {}

  getAllWarehouses(): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(`${this.apiUrl}${this.endpoints.GET_ALL}`);
  }

  getWarehouseById(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}${this.endpoints.BY_ID(id)}`);
  }

  createWarehouse(data: any): Observable<APIResponse<any>> {
    return this.http.post<APIResponse<any>>(`${this.apiUrl}${this.endpoints.CREATE}`, data);
  }

  updateWarehouse(id: number, data: any): Observable<APIResponse<any>> {
    return this.http.put<APIResponse<any>>(`${this.apiUrl}${this.endpoints.UPDATE(id)}`, data);
  }

  deleteWarehouse(id: number): Observable<APIResponse<any>> {
    return this.http.delete<APIResponse<any>>(`${this.apiUrl}${this.endpoints.DELETE(id)}`);
  }

  getWarehouseStock(id: number): Observable<APIResponse<any[]>> {
    return this.http.get<APIResponse<any[]>>(`${this.apiUrl}${this.endpoints.GET_STOCK(id)}`);
  }
}
