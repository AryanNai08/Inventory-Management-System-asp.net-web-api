import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse, PaginatedResult, PaginationParams } from '../../../core/models/api.model';
import { API_CONFIG } from '../../../shared/config/api.config';

export interface AdjustmentType {
  id: number;
  name: string;
}

export interface StockAdjustment {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  warehouseId: number;
  warehouseName: string;
  quantityChange: number;
  adjustmentTypeId: number;
  adjustmentTypeName: string;
  reason?: string;
  adjustedBy: number;
  adjustedByUserName: string;
  adjustmentDate: string;
}

export interface CreateStockAdjustmentDto {
  productId: number;
  warehouseId: number;
  quantityChange: number;
  adjustmentTypeId: number;
  reason?: string;
}

@Injectable({
  providedIn: 'root'
})
export class StockAdjustmentService {
  private apiUrl = API_CONFIG.BASE_URL;
  private endpoints = API_CONFIG.ENDPOINTS.STOCK_ADJUSTMENTS;

  constructor(private http: HttpClient) {}

  getAllAdjustments(params: PaginationParams): Observable<APIResponse<PaginatedResult<StockAdjustment>>> {
    const queryParams: any = {
      pageNumber: params.pageNumber.toString(),
      pageSize: params.pageSize.toString(),
      sortColumn: params.sortColumn || 'Id',
      sortOrder: params.sortOrder || 'desc'
    };
    if (params.searchTerm) {
      queryParams.searchTerm = params.searchTerm;
    }
    return this.http.get<APIResponse<PaginatedResult<StockAdjustment>>>(`${this.apiUrl}${this.endpoints.GET_ALL}`, { params: queryParams });
  }

  getAdjustmentById(id: number): Observable<APIResponse<StockAdjustment>> {
    return this.http.get<APIResponse<StockAdjustment>>(`${this.apiUrl}${this.endpoints.BY_ID(id)}`);
  }

  createAdjustment(dto: CreateStockAdjustmentDto): Observable<APIResponse<StockAdjustment>> {
    return this.http.post<APIResponse<StockAdjustment>>(`${this.apiUrl}${this.endpoints.CREATE}`, dto);
  }

  getAdjustmentTypes(): Observable<APIResponse<AdjustmentType[]>> {
    return this.http.get<APIResponse<AdjustmentType[]>>(`${this.apiUrl}${this.endpoints.TYPES}`);
  }
}
