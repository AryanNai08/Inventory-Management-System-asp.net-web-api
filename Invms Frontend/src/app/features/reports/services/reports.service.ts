import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';

/**
 * Report DTOs for type safety (optional - not fully used but helpful for structure)
 */
export interface ReportParams {
  startDate?: Date | string;
  endDate?: Date | string;
  year?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private apiUrl = `${API_CONFIG.BASE_URL}`;
  private endpoints = API_CONFIG.ENDPOINTS.REPORTS;

  constructor(private http: HttpClient) {}

  /**
   * Get Sales by Product report as PDF
   * @param startDate Optional start date for filtering
   * @param endDate Optional end date for filtering
   */
  getSalesByProductReport(startDate?: Date | string, endDate?: Date | string): Observable<Blob> {
    let params = '';
    if (startDate) {
      params += `?startDate=${this.formatDate(startDate)}`;
    }
    if (endDate) {
      params += params ? `&endDate=${this.formatDate(endDate)}` : `?endDate=${this.formatDate(endDate)}`;
    }

    return this.http.get(
      `${this.apiUrl}${this.endpoints.SALES_BY_PRODUCT}${params}`,
      { responseType: 'blob' }
    );
  }

  /**
   * Get Purchases by Supplier report as PDF
   * @param startDate Optional start date for filtering
   * @param endDate Optional end date for filtering
   */
  getPurchasesBySupplierReport(startDate?: Date | string, endDate?: Date | string): Observable<Blob> {
    let params = '';
    if (startDate) {
      params += `?startDate=${this.formatDate(startDate)}`;
    }
    if (endDate) {
      params += params ? `&endDate=${this.formatDate(endDate)}` : `?endDate=${this.formatDate(endDate)}`;
    }

    return this.http.get(
      `${this.apiUrl}${this.endpoints.PURCHASES_BY_SUPPLIER}${params}`,
      { responseType: 'blob' }
    );
  }

  /**
   * Get Stock Movement report as PDF
   * @param year The year for the report (defaults to current year if not provided)
   */
  getStockMovementReport(year?: number): Observable<Blob> {
    const queryYear = year || new Date().getFullYear();
    return this.http.get(
      `${this.apiUrl}${this.endpoints.STOCK_MOVEMENT}?year=${queryYear}`,
      { responseType: 'blob' }
    );
  }

  /**
   * Get Revenue report as PDF
   * @param startDate Optional start date for filtering
   * @param endDate Optional end date for filtering
   */
  getRevenueReport(startDate?: Date | string, endDate?: Date | string): Observable<Blob> {
    let params = '';
    if (startDate) {
      params += `?startDate=${this.formatDate(startDate)}`;
    }
    if (endDate) {
      params += params ? `&endDate=${this.formatDate(endDate)}` : `?endDate=${this.formatDate(endDate)}`;
    }

    return this.http.get(
      `${this.apiUrl}${this.endpoints.REVENUE}${params}`,
      { responseType: 'blob' }
    );
  }

  /**
   * Get Order Status Summary report as PDF
   */
  getOrderStatusSummaryReport(): Observable<Blob> {
    return this.http.get(
      `${this.apiUrl}${this.endpoints.ORDER_STATUS_SUMMARY}`,
      { responseType: 'blob' }
    );
  }

  /**
   * Download PDF file
   * @param blob The PDF blob data
   * @param filename The name for the downloaded file
   */
  downloadPDF(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  /**
   * Format date to string for API parameters
   * @param date Date object or string
   */
  private formatDate(date: Date | string): string {
    if (typeof date === 'string') {
      return date;
    }
    return date.toISOString().split('T')[0]; // Format: YYYY-MM-DD
  }

  /**
   * Generate filename with timestamp
   * @param baseName Base name for the file
   */
  generateFilename(baseName: string): string {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').split('T')[0];
    return `${baseName}_${timestamp}.pdf`;
  }
}
