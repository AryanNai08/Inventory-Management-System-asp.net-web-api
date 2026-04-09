import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { 
    path: 'auth', 
    loadChildren: () => import('./features/auth/auth-module').then(m => m.AuthModule) 
  },
  { 
    path: 'dashboard', 
    loadChildren: () => import('./features/dashboard/dashboard-module').then(m => m.DashboardModule) 
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
