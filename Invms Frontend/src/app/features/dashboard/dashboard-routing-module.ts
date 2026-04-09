import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardHome } from './pages/dashboard-home/dashboard-home';
import { AppLayout } from '../../shared/components/layout/app-layout/app-layout';

const routes: Routes = [
  {
    path: '',
    component: AppLayout,
    children: [
      { path: '', component: DashboardHome }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
