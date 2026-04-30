import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserAdd } from './pages/user-add/user-add';
import { UserList } from './pages/user-list/user-list';
import { ChangePassword } from './pages/change-password/change-password';
import { authGuard } from '../../core/guards/auth-guard';
import { SharedModule } from '../../shared/shared.module';

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
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class UserModule { }
