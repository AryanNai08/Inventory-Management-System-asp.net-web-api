import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly USER_KEY = 'usr_details';
  private permissionsCache: string[] = [];
  
  // Signal to notify components that storage is hydrated and permissions are ready
  private initializedSubject = new BehaviorSubject<boolean>(false);
  public initialization$ = this.initializedSubject.asObservable();

  constructor() {
    this.refreshPermissionsCache();
  }

  saveUser(user: any): void {
    // We only save identity data, NEVER the token
    const userData = {
      username: user.username,
      roles: user.roles || [],
      permissions: user.permissions || []
    };
    localStorage.setItem(this.USER_KEY, JSON.stringify(userData));
    this.refreshPermissionsCache();
  }

  getUser(): any | null {
    const user = localStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  public refreshPermissionsCache(): void {
    const user = this.getUser();
    if (!user) {
      this.permissionsCache = [];
      this.initializedSubject.next(true);
      return;
    }

    try {
      const permissions = user.permissions || [];
      if (Array.isArray(permissions)) {
        this.permissionsCache = permissions.map((p: string) => p.toLowerCase());
      } else {
        this.permissionsCache = [];
      }
      
      this.initializedSubject.next(true);
    } catch (e) {
      console.error('Error hydrating permissions:', e);
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

  // Helper for AuthGuard
  isLoggedIn(): boolean {
    return !!this.getUser();
  }

  clean(): void {
    localStorage.removeItem(this.USER_KEY);
    this.permissionsCache = [];
    this.initializedSubject.next(false);
  }
}
