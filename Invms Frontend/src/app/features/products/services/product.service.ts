import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';
import { APIResponse, PaginatedResult, PaginationParams } from '../../../core/models/api.model';
import { API_CONFIG } from '../../../shared/config/api.config';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

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
    const queryParams: any = {
      pageNumber: params.pageNumber.toString(),
      pageSize: params.pageSize.toString(),
      sortColumn: params.sortColumn || 'Id',
      sortOrder: params.sortOrder || 'asc'
    };
    if (params.searchTerm) {
      queryParams.searchTerm = params.searchTerm;
    }
    return this.http.get<APIResponse<PaginatedResult<Product>>>(`${this.apiUrl}${this.endpoints.GET_ALL}`, { 
      params: queryParams,
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No products found', 
            data: { 
              items: [], 
              totalCount: 0,
              pageNumber: 1,
              pageSize: 10,
              totalPages: 0,
              hasNextPage: false,
              hasPreviousPage: false
            } 
          } as APIResponse<PaginatedResult<Product>>);
        }
        return throwError(() => err);
      })
    );
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
    return this.http.get<APIResponse<Product[]>>(`${this.apiUrl}${this.endpoints.SEARCH}`, { 
      params,
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No products found', 
            data: [] 
          } as APIResponse<Product[]>);
        }
        return throwError(() => err);
      })
    );
  }

  getLowStock(): Observable<APIResponse<Product[]>> {
    return this.http.get<APIResponse<Product[]>>(`${this.apiUrl}${this.endpoints.LOW_STOCK}`, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ status: true, statusCode: 200, message: 'No low stock products', data: [] } as APIResponse<Product[]>);
        }
        return throwError(() => err);
      })
    );
  }

  getProductStockBreakdown(id: number): Observable<APIResponse<any>> {
    return this.http.get<APIResponse<any>>(`${this.apiUrl}${this.endpoints.STOCK_LOCATIONS(id)}`);
  }
}
