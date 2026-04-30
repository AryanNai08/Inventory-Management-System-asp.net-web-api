import { HttpContextToken } from '@angular/common/http';

/**
 * Context token to skip the global error toast in ErrorInterceptor.
 * Use this for APIs where 404 or other errors are handled locally (e.g., list/search APIs).
 */
export const BYPASS_ERROR_TOAST = new HttpContextToken<boolean>(() => false);
