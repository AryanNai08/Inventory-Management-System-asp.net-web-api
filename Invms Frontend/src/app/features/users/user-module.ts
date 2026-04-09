import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { UserAdd } from './pages/user-add/user-add';
import { ChangePassword } from './pages/change-password/change-password';
import { authGuard } from '../../core/guards/auth-guard';

const routes: Routes = [
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
    ChangePassword
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class UserModule { }
