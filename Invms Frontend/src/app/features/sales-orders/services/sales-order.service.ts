import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpContext } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse, PaginatedResult, PaginationParams } from '../../../core/models/api.model';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

export interface SalesOrder {
  id: number;
  orderNumber: string;
  customerId: number;
  customerName: string;
  orderDate: string;
  statusId: number;
  statusName: string;
  warehouseId: number;
  warehouseName: string;
  totalAmount: number;
  notes?: string;
  warnings: string[];
  createdBy?: string;
  updatedBy?: string;
  modifiedDate?: string;
  items: SalesOrderItem[];
}

export interface SalesOrderItem {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  quantity: number;
  unitPrice: number;
}

@Injectable({
  providedIn: 'root'
})
export class SalesOrderService {
  private readonly endpoints = API_CONFIG.ENDPOINTS.SALES_ORDERS;

  constructor(private http: HttpClient) {}

  getAllOrders(params: PaginationParams): Observable<APIResponse<PaginatedResult<SalesOrder>>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());
    
    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }

    return this.http.get<APIResponse<PaginatedResult<SalesOrder>>>(`${API_CONFIG.BASE_URL}${this.endpoints.GET_ALL}`, { 
      params: httpParams,
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No sales orders found', 
            data: { 
              items: [], 
              totalCount: 0,
              pageNumber: 1,
              pageSize: 10,
              totalPages: 0,
              hasNextPage: false,
              hasPreviousPage: false
            } 
          } as APIResponse<PaginatedResult<SalesOrder>>);
        }
        return throwError(() => err);
      })
    );
  }

  getOrderById(id: number): Observable<APIResponse<SalesOrder>> {
    return this.http.get<APIResponse<SalesOrder>>(`${API_CONFIG.BASE_URL}${this.endpoints.BY_ID(id)}`);
  }

  createOrder(order: any): Observable<APIResponse<SalesOrder>> {
    return this.http.post<APIResponse<SalesOrder>>(`${API_CONFIG.BASE_URL}${this.endpoints.CREATE}`, order);
  }

  updateOrder(id: number, order: any): Observable<APIResponse<SalesOrder>> {
    return this.http.put<APIResponse<SalesOrder>>(`${API_CONFIG.BASE_URL}${this.endpoints.UPDATE(id)}`, order);
  }

  confirmOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.patch<APIResponse<boolean>>(`${API_CONFIG.BASE_URL}${this.endpoints.APPROVE(id)}`, {});
  }

  shipOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.patch<APIResponse<boolean>>(`${API_CONFIG.BASE_URL}${this.endpoints.SHIP(id)}`, {});
  }

  deliverOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.patch<APIResponse<boolean>>(`${API_CONFIG.BASE_URL}${this.endpoints.DELIVER(id)}`, {});
  }

  cancelOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(`${API_CONFIG.BASE_URL}${this.endpoints.CANCEL(id)}`);
  }
}
