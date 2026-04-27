import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SalesOrderListComponent } from './pages/sales-order-list/sales-order-list';

const routes: Routes = [
  { path: '', component: SalesOrderListComponent }
];

@NgModule({
  declarations: [
    SalesOrderListComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class SalesOrderModule { }
