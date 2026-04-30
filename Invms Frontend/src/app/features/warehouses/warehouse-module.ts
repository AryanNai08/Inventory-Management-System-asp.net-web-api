import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { WarehouseList } from './pages/warehouse-list/warehouse-list';

const routes: Routes = [
  { path: '', component: WarehouseList }
];

@NgModule({
  declarations: [
    WarehouseList
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class WarehouseModule { }
