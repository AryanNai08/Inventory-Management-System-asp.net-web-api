import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { ProductList } from './pages/product-list/product-list';

const routes: Routes = [
  { path: '', component: ProductList }
];

@NgModule({
  declarations: [
    ProductList
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class ProductModule { }
