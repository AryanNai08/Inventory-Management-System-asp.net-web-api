import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { PurchaseOrderList } from './pages/purchase-order-list/purchase-order-list';

const routes: Routes = [
  { path: '', component: PurchaseOrderList }
];

@NgModule({
  declarations: [
    PurchaseOrderList
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class PurchaseOrderModule { }
