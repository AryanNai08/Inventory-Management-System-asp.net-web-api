import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { PurchaseOrderList } from './pages/purchase-order-list/purchase-order-list';

const routes: Routes = [
  { path: '', component: PurchaseOrderList }
];

@NgModule({
  declarations: [
    PurchaseOrderList
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class PurchaseOrderModule { }
