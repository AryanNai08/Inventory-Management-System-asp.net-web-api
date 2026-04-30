import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, forkJoin, of, catchError, throwError } from 'rxjs';
import { API_CONFIG } from '../../../shared/config/api.config';
import { APIResponse } from '../../../core/models/api.model';
import { BYPASS_ERROR_TOAST } from '../../../core/http-interceptors/interceptor-tokens';

export interface Privilege {
  id: number;
  name: string;
  description?: string;
}

export interface Role {
  id: number;
  name: string;
  description?: string;
  createdDate?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = API_CONFIG.BASE_URL;

  constructor(private http: HttpClient) {}

  // Role CRUD
  getAllRoles(): Observable<APIResponse<Role[]>> {
    return this.http.get<APIResponse<Role[]>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLES.GET_ALL}`, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ 
            status: true, 
            statusCode: 200, 
            message: 'No roles found', 
            data: [] 
          } as APIResponse<Role[]>);
        }
        return throwError(() => err);
      })
    );
  }

  createRole(role: any): Observable<APIResponse<boolean>> {
    return this.http.post<APIResponse<boolean>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLES.CREATE}`, role);
  }

  updateRole(id: number, role: any): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLES.UPDATE(id)}`, role);
  }

  deleteRole(id: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLES.DELETE(id)}`);
  }

  // Privileges master list
  getAllPrivileges(): Observable<APIResponse<Privilege[]>> {
    return this.http.get<APIResponse<Privilege[]>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.PRIVILEGES.GET_ALL}`, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ status: true, statusCode: 200, message: 'No privileges found', data: [] } as APIResponse<Privilege[]>);
        }
        return throwError(() => err);
      })
    );
  }

  // Role Mapping Read
  getRolePrivileges(roleId: number): Observable<APIResponse<Privilege[]>> {
    return this.http.get<APIResponse<Privilege[]>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLE_PRIVILEGES.BY_ROLE(roleId)}`, {
      context: new HttpContext().set(BYPASS_ERROR_TOAST, true)
    }).pipe(
      catchError(err => {
        if (err.status === 404) {
          return of({ status: true, statusCode: 200, message: 'No privileges for this role', data: [] } as APIResponse<Privilege[]>);
        }
        return throwError(() => err);
      })
    );
  }

  // Atomic Assignment
  assignPrivilege(roleId: number, privilegeId: number): Observable<APIResponse<boolean>> {
    return this.http.post<APIResponse<boolean>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLE_PRIVILEGES.ASSIGN}`, { roleId, privilegeId });
  }

  // Atomic Removal
  removePrivilege(roleId: number, privilegeId: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(`${this.apiUrl}${API_CONFIG.ENDPOINTS.ROLE_PRIVILEGES.REMOVE(roleId, privilegeId)}`);
  }

  /**
   * Syncs privileges by comparing current state vs target state
   * strictly using individual assign/remove calls.
   */
  syncPrivileges(roleId: number, originalIds: number[], currentIds: number[]): Observable<any[]> {
    const added = currentIds.filter(id => !originalIds.includes(id));
    const removed = originalIds.filter(id => !currentIds.includes(id));

    const requests: Observable<any>[] = [
      ...added.map(id => this.assignPrivilege(roleId, id)),
      ...removed.map(id => this.removePrivilege(roleId, id))
    ];

    if (requests.length === 0) {
      return of([]);
    }

    return forkJoin(requests);
  }
}
