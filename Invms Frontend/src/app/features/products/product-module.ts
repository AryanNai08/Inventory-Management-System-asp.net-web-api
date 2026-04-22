import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ProductList } from './pages/product-list/product-list';

const routes: Routes = [
  { path: '', component: ProductList }
];

@NgModule({
  declarations: [
    ProductList
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class ProductModule { }
