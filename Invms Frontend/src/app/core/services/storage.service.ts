import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'usr_details';
  private permissionsCache: string[] = [];
  
  // Signal to notify components that storage is hydrated and permissions are ready
  private initializedSubject = new BehaviorSubject<boolean>(false);
  public initialization$ = this.initializedSubject.asObservable();

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

  public refreshPermissionsCache(): void {
    const token = this.getToken();
    if (!token) {
      this.permissionsCache = [];
      this.initializedSubject.next(true); // Always signal ready (even if empty)
      return;
    }

    try {
      const payloadBase64Url = token.split('.')[1];
      // Robust decoding for Base64Url (JWT standard)
      const base64 = payloadBase64Url.replace(/-/g, '+').replace(/_/g, '/');
      const decodedJson = atob(base64);
      const decoded = JSON.parse(decodedJson);

      // Backend uses "Permission" (Capitalized, Singular)
      const permissionClaim = decoded['Permission'] || decoded['permission'] || [];
      if (Array.isArray(permissionClaim)) {
        this.permissionsCache = permissionClaim.map(p => p.toLowerCase());
      } else if (typeof permissionClaim === 'string') {
        this.permissionsCache = [permissionClaim.toLowerCase()];
      } else {
        this.permissionsCache = [];
      }
      
      // Notify all listening components that permissions are fully hydrated
      this.initializedSubject.next(true);
    } catch (e) {
      console.error('Error decoding JWT permissions:', e);
      this.permissionsCache = [];
      this.initializedSubject.next(true);
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
    this.initializedSubject.next(false);
  }
}
