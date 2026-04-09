import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth';
import { StorageService } from '../services/storage.service';
import { ToastService } from '../services/toast';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const storageService = inject(StorageService);
  const router = inject(Router);
  const toastService = inject(ToastService);

  // 1. Check if logged in
  if (!authService.isLoggedIn()) {
    toastService.error('Unauthorized', 'Please login to access this page.');
    router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url }});
    return false;
  }

  // 2. Check Role-based access if specified in route data
  const requiredRole = route.data?.['role'];
  if (requiredRole) {
    if (!storageService.hasRole(requiredRole)) {
      toastService.error('Access Denied', 'You do not have permission to access the requested resource.');
      router.navigate(['/dashboard']);
      return false;
    }
  }

  return true;
};
