import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ToastContainer } from './shared/components/ui/toast-container/toast-container';

import { authInterceptor } from './core/http-interceptors/auth.interceptor';
import { errorInterceptor } from './core/http-interceptors/error.interceptor';

@NgModule({
  declarations: [
    App, 
    ToastContainer
  ],
  imports: [
    BrowserModule, 
    AppRoutingModule
  ],
  providers: [
    provideHttpClient(
      withInterceptors([authInterceptor])
    )
  ],
  bootstrap: [App],
})
export class AppModule {}
