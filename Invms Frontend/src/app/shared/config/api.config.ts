import { environment } from '../../../environments/environment';

export const API_CONFIG = {
  BASE_URL: environment.apiUrl,
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/api/auth/login',
      REGISTER: '/api/auth/register',
      REFRESH: '/api/auth/refresh',
      LOGOUT: '/api/auth/logout'
    }
  }
};
