import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { SupplierList } from './pages/supplier-list/supplier-list';
import { authGuard } from '../../core/guards/auth-guard';

const routes: Routes = [
  {
    path: '',
    component: SupplierList,
    canActivate: [authGuard],
    data: { roles: ['Admin', 'Manager', 'Staff'] }
  }
];

@NgModule({
  declarations: [
    SupplierList
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class SupplierModule { }
