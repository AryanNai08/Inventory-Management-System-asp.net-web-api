import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { StockAdjustmentList } from './pages/stock-adjustment-list/stock-adjustment-list';

const routes: Routes = [
  { path: '', component: StockAdjustmentList }
];

@NgModule({
  declarations: [
    StockAdjustmentList
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class StockAdjustmentModule { }
