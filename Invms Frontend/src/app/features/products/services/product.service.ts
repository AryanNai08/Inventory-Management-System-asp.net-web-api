import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse, PaginatedResult, PaginationParams } from '../../../core/models/api.model';
import { API_CONFIG } from '../../../shared/config/api.config';

export interface Product {
  id: number;
  name: string;
  sku: string;
  description?: string;
  purchasePrice: number;
  salePrice: number;
  reorderLevel: number;
  categoryId: number;
  categoryName: string;
  supplierId: number;
  supplierName: string;
  totalStock: number;
  rowVersion: any;
  createdBy?: string;
  createdDate?: string;
  updatedBy?: string;
  modifiedDate?: string;
  stockStatus: number; // 0: OutOfStock, 1: LowStock, 2: InStock
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = API_CONFIG.BASE_URL;
  private endpoints = API_CONFIG.ENDPOINTS.PRODUCTS;

  constructor(private http: HttpClient) {}

  getAllProducts(params: PaginationParams): Observable<APIResponse<PaginatedResult<Product>>> {
    const queryParams = {
      pageNumber: params.pageNumber.toString(),
      pageSize: params.pageSize.toString(),
      sortColumn: params.sortColumn || 'Id',
      sortOrder: params.sortOrder || 'asc'
    };
    return this.http.get<APIResponse<PaginatedResult<Product>>>(`${this.apiUrl}${this.endpoints.GET_ALL}`, { params: queryParams });
  }

  getProductById(id: number): Observable<APIResponse<Product>> {
    return this.http.get<APIResponse<Product>>(`${this.apiUrl}${this.endpoints.BY_ID(id)}`);
  }

  createProduct(product: any): Observable<APIResponse<Product>> {
    return this.http.post<APIResponse<Product>>(`${this.apiUrl}${this.endpoints.CREATE}`, product);
  }

  updateProduct(id: number, product: any): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}${this.endpoints.UPDATE(id)}`, product);
  }

  deleteProduct(id: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(`${this.apiUrl}${this.endpoints.DELETE(id)}`);
  }

  searchProducts(name?: string, categoryId?: number, supplierId?: number): Observable<APIResponse<Product[]>> {
    let params: any = {};
    if (name) params.name = name;
    if (categoryId) params.categoryId = categoryId.toString();
    if (supplierId) params.supplierId = supplierId.toString();
    return this.http.get<APIResponse<Product[]>>(`${this.apiUrl}${this.endpoints.SEARCH}`, { params });
  }

  getLowStock(): Observable<APIResponse<Product[]>> {
    return this.http.get<APIResponse<Product[]>>(`${this.apiUrl}${this.endpoints.LOW_STOCK}`);
  }

  getProductStockBreakdown(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}${this.endpoints.STOCK_LOCATIONS(id)}`);
  }
}
