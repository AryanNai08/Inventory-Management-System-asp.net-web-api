import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { 
    path: 'auth', 
    loadChildren: () => import('./features/auth/auth-module').then(m => m.AuthModule) 
  },
  { 
    path: 'dashboard', 
    canActivate: [authGuard],
    loadChildren: () => import('./features/dashboard/dashboard-module').then(m => m.DashboardModule) 
  },
  {
    path: 'users',
    canActivate: [authGuard],
    loadChildren: () => import('./features/users/user-module').then(m => m.UserModule)
  },
  {
    path: 'categories',
    canActivate: [authGuard],
    loadChildren: () => import('./features/categories/category-module').then(m => m.CategoryModule)
  },
  {
    path: 'suppliers',
    canActivate: [authGuard],
    loadChildren: () => import('./features/suppliers/supplier-module').then(m => m.SupplierModule)
  },
  {
    path: 'customers',
    canActivate: [authGuard],
    loadChildren: () => import('./features/customers/customer-module').then(m => m.CustomerModule)
  },
  {
    path: 'warehouses',
    canActivate: [authGuard],
    loadChildren: () => import('./features/warehouses/warehouse-module').then(m => m.WarehouseModule)
  },
  {
    path: 'roles',
    canActivate: [authGuard],
    loadChildren: () => import('./features/roles/role-module').then(m => m.RoleModule)
  },
  {
    path: 'reports',
    canActivate: [authGuard],
    loadChildren: () => import('./features/reports/reports-module').then(m => m.ReportsModule)
  },
  {
    path: 'products',
    canActivate: [authGuard],
    loadChildren: () => import('./features/products/product-module').then(m => m.ProductModule)
  },
  {
    path: 'purchase-orders',
    canActivate: [authGuard],
    loadChildren: () => import('./features/purchase-orders/purchase-order-module').then(m => m.PurchaseOrderModule)
  },
  {
    path: 'sales-orders',
    canActivate: [authGuard],
    loadChildren: () => import('./features/sales-orders/sales-order-module').then(m => m.SalesOrderModule)
  },
  {
    path: 'stock-adjustments',
    canActivate: [authGuard],
    loadChildren: () => import('./features/stock-adjustments/stock-adjustment-module').then(m => m.StockAdjustmentModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
