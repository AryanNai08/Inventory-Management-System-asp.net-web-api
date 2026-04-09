import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing-module';
import { LoginPage } from './pages/login-page/login-page';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [LoginPage],
  imports: [CommonModule, AuthRoutingModule, ReactiveFormsModule],
})
export class AuthModule {}
