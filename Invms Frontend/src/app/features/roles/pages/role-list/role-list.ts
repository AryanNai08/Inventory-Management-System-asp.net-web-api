import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RoleService, Role, Privilege } from '../../services/role.service';
import { StorageService } from '../../../../core/services/storage.service';
import { ToastService } from '../../../../core/services/toast';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-role-list',
  standalone: false,
  templateUrl: './role-list.html',
  styleUrl: './role-list.scss'
})
export class RoleList implements OnInit {
  roles: Role[] = [];
  filteredRoles: Role[] = [];
  allPrivileges: Privilege[] = [];
  searchTerm: string = '';
  
  // Privilege tracking sets
  originalPrivilegeIds: number[] = [];
  selectedPrivilegeIds: number[] = [];
  
  isLoading = false;
  isProcessing = false;
  isModalOpen = false;
  isEditMode = false;
  selectedRoleId: number | null = null;
  
  roleForm: FormGroup;
  canManageRoles = false;

  constructor(
    private fb: FormBuilder,
    private roleService: RoleService,
    private toast: ToastService,
    private storageService: StorageService,
    private cdr: ChangeDetectorRef
  ) {
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['']
    });
    this.canManageRoles = this.storageService.hasPermission('ManageRoles');
  }

  ngOnInit(): void {
    this.loadRoles();
    this.loadAllPrivileges();
  }

  loadRoles(): void {
    this.isLoading = true;
    this.roleService.getAllRoles().subscribe({
      next: (res) => {
        this.roles = res.data || [];
        this.applyFilter();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('System', 'Failed to load roles');
        this.isLoading = false;
      }
    });
  }

  loadAllPrivileges(): void {
    this.roleService.getAllPrivileges().subscribe({
      next: (res) => {
        this.allPrivileges = res.data || [];
        this.cdr.detectChanges();
      }
    });
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedRoleId = null;
    this.selectedPrivilegeIds = [];
    this.originalPrivilegeIds = [];
    this.roleForm.reset();
    this.isModalOpen = true;
  }

  openEditModal(role: Role): void {
    this.isEditMode = true;
    this.selectedRoleId = role.id;
    this.roleForm.patchValue({
      name: role.name,
      description: role.description
    });
    
    this.isLoading = true;
    this.roleService.getRolePrivileges(role.id).subscribe({
      next: (res) => {
        this.originalPrivilegeIds = (res.data || []).map(p => p.id);
        this.selectedPrivilegeIds = [...this.originalPrivilegeIds];
        this.isLoading = false;
        this.isModalOpen = true;
      },
      error: () => {
        this.toast.error('Access Denied', 'Failed to load role permissions');
        this.isLoading = false;
      }
    });
  }

  togglePrivilege(privilegeId: number): void {
    if (this.isProcessing) return;
    
    const index = this.selectedPrivilegeIds.indexOf(privilegeId);
    if (index > -1) {
      this.selectedPrivilegeIds.splice(index, 1);
    } else {
      this.selectedPrivilegeIds.push(privilegeId);
    }
  }

  isPrivilegeSelected(privilegeId: number): boolean {
    return this.selectedPrivilegeIds.includes(privilegeId);
  }

  onSave(): void {
    if (this.roleForm.invalid || this.isProcessing) return;

    this.isProcessing = true;
    const roleData = this.roleForm.value;

    if (this.isEditMode && this.selectedRoleId) {
      // 1. Update metadata first
      this.roleService.updateRole(this.selectedRoleId, roleData).subscribe({
        next: () => {
          // 2. Sync privileges using set comparison logic
          this.syncAll(this.selectedRoleId!);
        },
        error: () => {
          this.toast.error('Update Failed', 'Could not update role profile');
          this.isProcessing = false;
        }
      });
    } else {
      // Create logic (Prompt says skip privilege assignment during initial create)
      this.roleService.createRole(roleData).subscribe({
        next: () => {
          this.toast.success('Identity', 'New role created successfully');
          this.loadRoles();
          this.closeModal();
        },
        error: () => {
          this.toast.error('Creation Failed', 'Failed to register new role');
          this.isProcessing = false;
        }
      });
    }
  }

  private syncAll(roleId: number): void {
    this.roleService.syncPrivileges(roleId, this.originalPrivilegeIds, this.selectedPrivilegeIds).subscribe({
      next: () => {
        this.toast.success('Compliance', 'Role and permissions synced successfully');
        this.loadRoles();
        this.closeModal();
      },
      error: () => {
        this.toast.info('Partial Sync', 'Role updated, but permission sync met some errors');
        this.loadRoles();
        this.closeModal();
      }
    });
  }

  deleteRole(id: number): void {
    if (confirm('Are you sure you want to delete this role from the system?')) {
      this.roleService.deleteRole(id).subscribe({
        next: () => {
          this.toast.success('System cleanup', 'Role removed successfully');
          this.loadRoles();
        },
        error: (err) => {
          const msg = err.error?.Message || err.error?.Error || 'Failed to delete role';
          this.toast.error('Access Denied', msg);
        }
      });
    }
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.isProcessing = false;
    this.isLoading = false;
  }
  onSearch(term: string): void {
    this.searchTerm = term;
    this.applyFilter();
  }

  applyFilter(): void {
    if (!this.searchTerm) {
      this.filteredRoles = [...this.roles];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredRoles = this.roles.filter(r => 
        r.name.toLowerCase().includes(term) || 
        (r.description && r.description.toLowerCase().includes(term))
      );
    }
  }
}
