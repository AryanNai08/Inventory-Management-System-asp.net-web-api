import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { AppLayout } from './app-layout/app-layout';
import { Sidebar } from './sidebar/sidebar';
import { Topbar } from './topbar/topbar';

@NgModule({
  declarations: [
    AppLayout,
    Sidebar,
    Topbar
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    AppLayout,
    Sidebar,
    Topbar
  ]
})
export class ShellLayoutModule { }
