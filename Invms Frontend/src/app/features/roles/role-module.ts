import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { RoleList } from './pages/role-list/role-list';

const routes: Routes = [
  { path: '', component: RoleList }
];

@NgModule({
  declarations: [
    RoleList
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class RoleModule { }
