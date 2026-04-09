import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { UserAdd } from './pages/user-add/user-add';
import { UserList } from './pages/user-list/user-list';
import { ChangePassword } from './pages/change-password/change-password';
import { authGuard } from '../../core/guards/auth-guard';

const routes: Routes = [
  { 
    path: '', 
    component: UserList,
    canActivate: [authGuard],
    data: { role: 'Admin' }
  },
  { 
    path: 'add', 
    component: UserAdd,
    canActivate: [authGuard],
    data: { role: 'Admin' }
  },
  {
    path: 'change-password',
    component: ChangePassword,
    canActivate: [authGuard]
  }
];

@NgModule({
  declarations: [
    UserAdd,
    UserList,
    ChangePassword
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class UserModule { }
