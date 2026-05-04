import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { AppLayout } from './shared/components/layout/app-layout/app-layout';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth-module').then(m => m.AuthModule)
  },
  {
    path: '',
    component: AppLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard-module').then(m => m.DashboardModule)
      },
      {
        path: 'users',
        loadChildren: () => import('./features/users/user-module').then(m => m.UserModule)
      },
      {
        path: 'categories',
        loadChildren: () => import('./features/categories/category-module').then(m => m.CategoryModule)
      },
      {
        path: 'suppliers',
        loadChildren: () => import('./features/suppliers/supplier-module').then(m => m.SupplierModule)
      },
      {
        path: 'customers',
        loadChildren: () => import('./features/customers/customer-module').then(m => m.CustomerModule)
      },
      {
        path: 'warehouses',
        loadChildren: () => import('./features/warehouses/warehouse-module').then(m => m.WarehouseModule)
      },
      {
        path: 'roles',
        loadChildren: () => import('./features/roles/role-module').then(m => m.RoleModule)
      },
      {
        path: 'reports',
        loadChildren: () => import('./features/reports/reports-module').then(m => m.ReportsModule)
      },
      {
        path: 'products',
        loadChildren: () => import('./features/products/product-module').then(m => m.ProductModule)
      },
      {
        path: 'purchase-orders',
        loadChildren: () => import('./features/purchase-orders/purchase-order-module').then(m => m.PurchaseOrderModule)
      },
      {
        path: 'sales-orders',
        loadChildren: () => import('./features/sales-orders/sales-order-module').then(m => m.SalesOrderModule)
      },
      {
        path: 'stock-adjustments',
        loadChildren: () => import('./features/stock-adjustments/stock-adjustment-module').then(m => m.StockAdjustmentModule)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
