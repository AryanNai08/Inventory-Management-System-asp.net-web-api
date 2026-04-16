import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardRoutingModule } from './dashboard-routing-module';
import { DashboardHome } from './pages/dashboard-home/dashboard-home';
import { DashboardAnalytics } from './pages/dashboard-analytics/dashboard-analytics';
import { ShellLayoutModule } from '../../shared/components/layout/shell-layout.module';

@NgModule({
  declarations: [
    DashboardHome,
    DashboardAnalytics
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    ShellLayoutModule
  ]
})
export class DashboardModule { }
