import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardSummaryDto, LowStockDto, TopProductDto } from '../../models/dashboard.model';

@Component({
  selector: 'app-dashboard-analytics',
  templateUrl: './dashboard-analytics.html',
  styleUrl: './dashboard-analytics.scss',
  standalone: false
})
export class DashboardAnalytics implements OnInit {
  summaryData: DashboardSummaryDto | null = null;
  topSellingProducts: TopProductDto[] = [];
  lowStockProducts: LowStockDto[] = [];
  
  isLoadingSummary = true;
  isLoadingLowStock = true;
  error: string | null = null;

  get isLoading(): boolean {
    return this.isLoadingSummary || this.isLoadingLowStock;
  }

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoadingSummary = true;
    this.isLoadingLowStock = true;
    this.error = null;

    this.dashboardService.getSummary().subscribe({
      next: (res) => {
        if (res.status) {
          console.log('Dashboard Summary Data:', res.data);
          this.summaryData = res.data;
          this.topSellingProducts = res.data?.topSellingProducts || [];
        }
        this.isLoadingSummary = false;
      },
      error: (err) => {
        this.handleError(err);
        this.isLoadingSummary = false;
      }
    });

    this.dashboardService.getLowStock().subscribe({
      next: (res) => {
        if (res.status && res.data) {
          this.lowStockProducts = res.data;
        }
        this.isLoadingLowStock = false;
      },
      error: (err) => {
        this.handleError(err);
        this.isLoadingLowStock = false;
      }
    });
  }

  private handleError(err: any): void {
    console.error('Analytics Loading Error:', err);
    this.error = 'Failed to load some analytics data. Please try again.';
  }
}
