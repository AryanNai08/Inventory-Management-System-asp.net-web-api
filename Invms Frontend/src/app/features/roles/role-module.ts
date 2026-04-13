import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RoleList } from './pages/role-list/role-list';

const routes: Routes = [
  { path: '', component: RoleList }
];

@NgModule({
  declarations: [
    RoleList
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    FormsModule
  ]
})
export class RoleModule { }
