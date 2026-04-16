export interface DashboardSummaryDto {
  totalSales: number;
  totalPurchases: number;
  totalProducts: number;
  totalSuppliers: number;
  totalCustomers: number;
  lowStockItemsCount: number;
  topSellingProducts: TopProductDto[];
}

export interface TopProductDto {
  productId: number;
  productName: string;
  totalQuantitySold: number;
  totalRevenue: number;
}

export interface LowStockDto {
  productId: number;
  productName: string;
  currentStock: number;
  reorderLevel: number;
  categoryName: string;
}
