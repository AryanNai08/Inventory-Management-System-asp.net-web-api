import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'usr_details';
  private permissionsCache: string[] = [];

  constructor() {
    this.refreshPermissionsCache();
  }

  saveToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    this.refreshPermissionsCache();
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  saveUser(user: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  getUser(): any | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  private refreshPermissionsCache(): void {
    const token = this.getToken();
    if (!token) {
      this.permissionsCache = [];
      return;
    }

    try {
      const payloadBase64 = token.split('.')[1];
      const decodedJson = atob(payloadBase64);
      const decoded = JSON.parse(decodedJson);

      // Handle both single string and array of strings for "Permission" claim
      const permissionClaim = decoded['Permission'];
      if (Array.isArray(permissionClaim)) {
        this.permissionsCache = permissionClaim.map(p => p.toLowerCase());
      } else if (typeof permissionClaim === 'string') {
        this.permissionsCache = [permissionClaim.toLowerCase()];
      } else {
        this.permissionsCache = [];
      }
    } catch (e) {
      console.error('Error decoding JWT permissions:', e);
      this.permissionsCache = [];
    }
  }

  getRoles(): string[] {
    const user = this.getUser();
    return (user?.roles || []).map((r: string) => r.toLowerCase());
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role.toLowerCase());
  }

  isAdmin(): boolean {
    return this.hasRole('admin');
  }

  hasPermission(permission: string): boolean {
    if (!permission) return false;
    return this.permissionsCache.includes(permission.toLowerCase());
  }

  getPermissions(): string[] {
    return this.permissionsCache;
  }

  clean(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.permissionsCache = [];
  }
}
