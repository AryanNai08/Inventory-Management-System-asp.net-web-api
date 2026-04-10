import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'usr_details';

  saveToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
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

  getRoles(): string[] {
    const user = this.getUser();
    return (user?.roles || []).map((r: string) => r.toLowerCase());
  }

  getPermissions(): string[] {
    const user = this.getUser();
    return (user?.permissions || []).map((p: string) => p.toLowerCase());
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role.toLowerCase());
  }

  isAdmin(): boolean {
    return this.hasRole('admin');
  }

  hasPermission(permission: string): boolean {
    return this.getPermissions().includes(permission.toLowerCase());
  }

  clean(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}
