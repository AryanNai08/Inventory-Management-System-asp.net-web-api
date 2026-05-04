import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ToastContainer } from './shared/components/ui/toast-container/toast-container';
import { ShellLayoutModule } from './shared/components/layout/shell-layout.module';

import { authInterceptor } from './core/http-interceptors/auth.interceptor';
import { errorInterceptor } from './core/http-interceptors/error.interceptor';
import { StorageService } from './core/services/storage.service';

// Factory to ensure StorageService decoded permissions before any component starts
export function initializeApp(storageService: StorageService) {
  return () => {
    storageService.refreshPermissionsCache();
    return Promise.resolve();
  };
}

@NgModule({
  declarations: [
    App, 
    ToastContainer
  ],
  imports: [
    BrowserModule, 
    AppRoutingModule,
    ShellLayoutModule
  ],
  providers: [
    provideHttpClient(
      withInterceptors([authInterceptor, errorInterceptor])
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [StorageService],
      multi: true
    }
  ],
  bootstrap: [App],
})
export class AppModule {}
