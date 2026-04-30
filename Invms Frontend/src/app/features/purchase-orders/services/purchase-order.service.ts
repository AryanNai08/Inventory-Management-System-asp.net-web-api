import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse, PaginatedResult, PaginationParams } from '../../../core/models/api.model';
import { API_CONFIG } from '../../../shared/config/api.config';

export interface PurchaseOrderItem {
  id: number;
  productId: number;
  productName: string;
  productSku: string;
  quantity: number;
  unitCost: number;
  lineTotal: number;
}

export interface PurchaseOrder {
  id: number;
  orderNumber: string;
  supplierId: number;
  supplierName: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  statusId: number;
  statusName: string;
  warehouseId: number;
  warehouseName: string;
  totalAmount: number;
  notes?: string;
  createdBy?: string;
  createdDate: string;
  updatedBy?: string;
  modifiedDate?: string;
  items: PurchaseOrderItem[];
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private apiUrl = API_CONFIG.BASE_URL;
  private endpoints = API_CONFIG.ENDPOINTS.PURCHASE_ORDERS;

  constructor(private http: HttpClient) {}

  getAllOrders(params: PaginationParams): Observable<APIResponse<PaginatedResult<PurchaseOrder>>> {
    const queryParams: any = {
      pageNumber: params.pageNumber.toString(),
      pageSize: params.pageSize.toString(),
      sortColumn: params.sortColumn || 'Id',
      sortOrder: params.sortOrder || 'desc'
    };
    if (params.searchTerm) {
      queryParams.searchTerm = params.searchTerm;
    }
    return this.http.get<APIResponse<PaginatedResult<PurchaseOrder>>>(`${this.apiUrl}${this.endpoints.GET_ALL}`, { params: queryParams });
  }

  getOrderById(id: number): Observable<APIResponse<PurchaseOrder>> {
    return this.http.get<APIResponse<PurchaseOrder>>(`${this.apiUrl}${this.endpoints.BY_ID(id)}`);
  }

  createOrder(order: any): Observable<APIResponse<PurchaseOrder>> {
    return this.http.post<APIResponse<PurchaseOrder>>(`${this.apiUrl}${this.endpoints.CREATE}`, order);
  }

  updateOrder(id: number, order: any): Observable<APIResponse<PurchaseOrder>> {
    return this.http.put<APIResponse<PurchaseOrder>>(`${this.apiUrl}${this.endpoints.UPDATE(id)}`, order);
  }

  approveOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}${this.endpoints.APPROVE(id)}`, {});
  }

  receiveOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}${this.endpoints.RECEIVE(id)}`, {});
  }

  cancelOrder(id: number): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}${this.endpoints.CANCEL(id)}`, {});
  }
}
