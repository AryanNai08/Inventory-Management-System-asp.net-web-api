import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardHome } from './pages/dashboard-home/dashboard-home';
import { DashboardAnalytics } from './pages/dashboard-analytics/dashboard-analytics';
import { AppLayout } from '../../shared/components/layout/app-layout/app-layout';

const routes: Routes = [
  {
    path: '',
    component: AppLayout,
    children: [
      { 
        path: '', 
        component: DashboardHome 
      },
      {
        path: 'analytics',
        component: DashboardAnalytics
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
