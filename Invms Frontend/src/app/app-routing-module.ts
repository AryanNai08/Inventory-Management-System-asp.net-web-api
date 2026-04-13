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
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
