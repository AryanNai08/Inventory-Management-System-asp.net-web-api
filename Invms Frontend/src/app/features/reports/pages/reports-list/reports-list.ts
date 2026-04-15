import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportsService } from '../../services/reports.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

interface ReportConfig {
  id: string;
  title: string;
  description: string;
  endpoint: string;
  requiresDateRange: boolean;
  requiresYear: boolean;
  icon: string;
}

@Component({
  selector: 'app-reports-list',
  standalone: false,
  templateUrl: './reports-list.html',
  styleUrl: './reports-list.scss'
})
export class ReportsListComponent implements OnInit, OnDestroy {
  // Reports Configuration
  reports: ReportConfig[] = [
    {
      id: 'sales-by-product',
      title: 'Sales by Product Report',
      description: 'Detailed sales data grouped by product with quantities and revenue',
      endpoint: 'sales-by-product',
      requiresDateRange: true,
      requiresYear: false,
      icon: 'chart-bar'
    },
    {
      id: 'purchases-by-supplier',
      title: 'Purchases by Supplier Report',
      description: 'Purchase details organized by supplier with order counts and investments',
      endpoint: 'purchases-by-supplier',
      requiresDateRange: true,
      requiresYear: false,
      icon: 'truck'
    },
    {
      id: 'stock-movement',
      title: 'Stock Movement Report',
      description: 'Monthly stock in/out movements showing inventory trends',
      endpoint: 'stock-movement',
      requiresDateRange: false,
      requiresYear: true,
      icon: 'boxes'
    },
    {
      id: 'revenue',
      title: 'Revenue Report',
      description: 'Revenue, cost and profit analysis for specified period',
      endpoint: 'revenue',
      requiresDateRange: true,
      requiresYear: false,
      icon: 'money-bill-wave'
    },
    {
      id: 'order-status',
      title: 'Order Status Summary',
      description: 'Summary of all orders grouped by their current status',
      endpoint: 'order-status-summary',
      requiresDateRange: false,
      requiresYear: false,
      icon: 'list-check'
    }
  ];

  // State Properties
  selectedReport: ReportConfig | null = null;
  isModalOpen = false;
  isGenerating = false;
  hasViewReportsPermission = false;

  // Form Properties
  reportForm!: FormGroup;
  availableYears: number[] = [];

  // Unsubscribe helper
  private destroy$ = new Subject<void>();

  constructor(
    private reportsService: ReportsService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder
  ) {
    this.checkPermissions();
    this.initializeForm();
    this.generateAvailableYears();
  }

  ngOnInit(): void {
    // No additional initialization needed
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Check if user has permission to view reports
   */
  private checkPermissions(): void {
    this.hasViewReportsPermission = this.storageService.hasPermission('ViewReports');
  }

  /**
   * Initialize the report form
   */
  private initializeForm(): void {
    this.reportForm = this.fb.group({
      startDate: [''],
      endDate: [''],
      year: [new Date().getFullYear()]
    });
  }

  /**
   * Generate available years for year selector (current year and past 5 years)
   */
  private generateAvailableYears(): void {
    const currentYear = new Date().getFullYear();
    for (let i = 0; i < 6; i++) {
      this.availableYears.push(currentYear - i);
    }
  }

  /**
   * Handle report card click
   */
  onReportClick(report: ReportConfig): void {
    if (!this.hasViewReportsPermission) {
      this.toastService.error('Access Denied', 'You do not have permission to generate reports');
      return;
    }

    this.selectedReport = report;
    this.resetForm();
    this.isModalOpen = true;
  }

  /**
   * Close modal and reset state
   */
  closeModal(): void {
    this.isModalOpen = false;
    this.selectedReport = null;
    this.resetForm();
  }

  /**
   * Reset form to default values
   */
  private resetForm(): void {
    this.reportForm.reset({
      startDate: '',
      endDate: '',
      year: new Date().getFullYear()
    });
  }

  /**
   * Validate form based on selected report
   */
  private validateForm(): boolean {
    if (!this.selectedReport) {
      return false;
    }

    if (this.selectedReport.requiresDateRange) {
      const startDate = this.reportForm.get('startDate')?.value;
      const endDate = this.reportForm.get('endDate')?.value;

      if (!startDate || !endDate) {
        this.toastService.error('Validation', 'Please select both start and end dates');
        return false;
      }

      // Validate date order
      const start = new Date(startDate);
      const end = new Date(endDate);
      if (start > end) {
        this.toastService.error('Validation', 'Start date cannot be after end date');
        return false;
      }
    }

    if (this.selectedReport.requiresYear) {
      const year = this.reportForm.get('year')?.value;
      if (!year) {
        this.toastService.error('Validation', 'Please select a year');
        return false;
      }
    }

    return true;
  }

  /**
   * Generate and download the report
   */
  generateReport(): void {
    if (!this.validateForm()) {
      return;
    }

    if (!this.selectedReport) {
      return;
    }

    this.isGenerating = true;
    const report = this.selectedReport;

    let reportObservable;

    switch (report.id) {
      case 'sales-by-product':
        reportObservable = this.reportsService.getSalesByProductReport(
          this.reportForm.get('startDate')?.value,
          this.reportForm.get('endDate')?.value
        );
        break;

      case 'purchases-by-supplier':
        reportObservable = this.reportsService.getPurchasesBySupplierReport(
          this.reportForm.get('startDate')?.value,
          this.reportForm.get('endDate')?.value
        );
        break;

      case 'stock-movement':
        reportObservable = this.reportsService.getStockMovementReport(
          this.reportForm.get('year')?.value
        );
        break;

      case 'revenue':
        reportObservable = this.reportsService.getRevenueReport(
          this.reportForm.get('startDate')?.value,
          this.reportForm.get('endDate')?.value
        );
        break;

      case 'order-status':
        reportObservable = this.reportsService.getOrderStatusSummaryReport();
        break;

      default:
        this.toastService.error('Error', 'Unknown report type');
        this.isGenerating = false;
        return;
    }

    reportObservable
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (blob: Blob) => {
          const filename = this.reportsService.generateFilename(report.id.replace(/-/g, '_'));
          this.reportsService.downloadPDF(blob, filename);
          this.toastService.success('Success', `${report.title} downloaded successfully`);
          this.isGenerating = false;
          this.closeModal();
        },
        error: (error) => {
          this.isGenerating = false;
          const errorMsg = error?.error?.Error || error?.error?.message || 'Failed to generate report';
          this.toastService.error('Error', errorMsg);
        }
      });
  }

  /**
   * Get CSS classes for report card
   */
  getReportCardClass(reportId: string): string {
    const baseClass = 'report-card';
    const icon = this.getReportIcon(reportId);
    return `${baseClass} ${icon}`;
  }

  /**
   * Get icon class for report
   */
  getReportIcon(reportId: string): string {
    const report = this.reports.find(r => r.id === reportId);
    return report ? `icon-${report.icon}` : '';
  }

  /**
   * Get icon element for display
   */
  getIconElement(iconClass: string): string {
    const iconMap: { [key: string]: string } = {
      'chart-bar': '📊',
      'truck': '🚚',
      'boxes': '📦',
      'money-bill-wave': '💰',
      'list-check': '✅'
    };
    return iconMap[iconClass] || '📄';
  }
}
