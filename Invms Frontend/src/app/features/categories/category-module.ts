import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { CategoryList } from './pages/category-list/category-list';
import { authGuard } from '../../core/guards/auth-guard';

const routes: Routes = [
  {
    path: '',
    component: CategoryList,
    canActivate: [authGuard],
    data: { roles: ['Admin', 'Manager', 'Staff'] }
  }
];

@NgModule({
  declarations: [
    CategoryList
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class CategoryModule { }
