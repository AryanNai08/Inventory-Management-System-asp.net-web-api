import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportsListComponent } from './pages/reports-list/reports-list';
import { authGuard } from '../../core/guards/auth-guard';
import { AppLayout } from '../../shared/components/layout/app-layout/app-layout';

const routes: Routes = [
  {
    path: '',
    component: AppLayout,
    children: [
      {
        path: '',
        component: ReportsListComponent,
        canActivate: [authGuard],
        data: { roles: ['Admin', 'Manager', 'Staff'] }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
