import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { StockAdjustmentList } from './pages/stock-adjustment-list/stock-adjustment-list';

const routes: Routes = [
  { path: '', component: StockAdjustmentList }
];

@NgModule({
  declarations: [
    StockAdjustmentList
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class StockAdjustmentModule { }
