import { environment } from '../../../environments/environment';

export const API_CONFIG = {
  BASE_URL: environment.apiUrl,
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/api/auth/login',
      LOGOUT: '/api/auth/logout',
      REGISTER: '/api/auth/register',
      REFRESH: '/api/auth/refresh',
      FORGOT_PASSWORD: '/api/auth/forgot-password',
      RESET_PASSWORD: '/api/auth/reset-password',
      CHANGE_PASSWORD: '/api/auth/change-password'
    },
    USERS: {
      BASE: '/api/users',
      ME: '/api/users/me',
      GET_ALL: '/api/users/GetAllUsers',
      BY_ID: (id: number) => `/api/users/${id}`,
      DELETE: (id: number) => `/api/users/${id}`,
      UPDATE: (id: number) => `/api/users/${id}`
    },
    CATEGORIES: {
      GET_ALL: '/api/Categories/GetAllCategories',
      BY_ID: (id: number) => `/api/Categories/${id}`,
      CREATE: '/api/Categories/CreateCategory',
      UPDATE: (id: number) => `/api/Categories/${id}`,
      DELETE: (id: number) => `/api/Categories/${id}`
    },
    SUPPLIERS: {
      GET_ALL: '/api/Suppliers/GetAllSuppliers',
      BY_ID: (id: number) => `/api/Suppliers/${id}`,
      CREATE: '/api/Suppliers/CreateSupplier',
      UPDATE: (id: number) => `/api/Suppliers/${id}`,
      DELETE: (id: number) => `/api/Suppliers/${id}`
    },
    CUSTOMERS: {
      GET_ALL: '/api/Customers/GetAllCustomers',
      BY_ID: (id: number) => `/api/Customers/${id}`,
      CREATE: '/api/Customers/CreateCustomer',
      UPDATE: (id: number) => `/api/Customers/${id}`,
      DELETE: (id: number) => `/api/Customers/${id}`
    },
    WAREHOUSES: {
      GET_ALL: '/api/Warehouses/GetAllWarehouses',
      BY_ID: (id: number) => `/api/Warehouses/${id}`,
      CREATE: '/api/Warehouses/CreateWarehouse',
      UPDATE: (id: number) => `/api/Warehouses/${id}`,
      DELETE: (id: number) => `/api/Warehouses/${id}`
    },
    ROLES: {
      GET_ALL: '/api/Roles/all',
      CREATE: '/api/Roles/create',
      UPDATE: (id: number) => `/api/Roles/update/${id}`,
      DELETE: (id: number) => `/api/Roles/delete/${id}`
    },
    PRIVILEGES: {
      GET_ALL: '/api/Privileges/all',
      BY_ID: (id: number) => `/api/Privileges/id/${id}`
    },
    ROLE_PRIVILEGES: {
      ASSIGN: '/api/RolePrivileges/assign',
      BY_ROLE: (roleId: number) => `/api/RolePrivileges/${roleId}/privileges`,
      REMOVE: (roleId: number, privId: number) => `/api/RolePrivileges/${roleId}/${privId}`
    }
  }
};
