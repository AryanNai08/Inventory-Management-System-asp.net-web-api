import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-user-list',
  standalone: false,
  templateUrl: './user-list.html',
  styleUrl: './user-list.scss'
})
export class UserList implements OnInit {
  users: any[] = [];
  isLoading = false;
  
  // Permission Access
  canManageUsers = false;
  canRegisterUsers = false;

  // Edit Modal State
  isEditModalOpen = false;
  editForm: FormGroup;
  selectedUserId: number | null = null;
  isSaving = false;

  constructor(
    private userService: UserService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.editForm = this.fb.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  checkPermissions(): void {
    this.canManageUsers = this.storageService.hasPermission('ManageUsers');
    this.canRegisterUsers = this.storageService.hasPermission('ManageUserRegistration');
  }

  ngOnInit(): void {
    // Ensuring immediate load
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status) {
          this.users = res.data || [];
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch users';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  onEdit(user: any): void {
    this.selectedUserId = user.id;
    this.editForm.patchValue({
      fullName: user.fullName,
      email: user.email
    });
    this.isEditModalOpen = true;
  }

  closeEditModal(): void {
    this.isEditModalOpen = false;
    this.selectedUserId = null;
    this.editForm.reset();
  }

  onUpdate(): void {
    if (this.editForm.invalid || !this.selectedUserId) return;

    this.isSaving = true;
    this.userService.updateUser(this.selectedUserId, this.editForm.value).subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', 'User updated successfully');
          this.closeEditModal();
          this.fetchUsers(); // Refresh list
        } else {
          this.toastService.error('Update Failed', res.error || 'Check your entry');
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isSaving = false;
        const msg = err.error?.Error || err.error?.message || 'Update failed';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  onDelete(id: number): void {
    if (!confirm('Are you sure you want to delete this user? This action cannot be undone.')) {
      return;
    }

    this.userService.deleteUser(id).subscribe({
      next: (res) => {
        if (res.status) {
          this.toastService.success('Deleted', 'User has been removed successfully');
          this.fetchUsers();
        } else {
          this.toastService.error('Error', res.error || 'Failed to delete user');
        }
      },
      error: (err) => {
        const msg = err.error?.Error || err.error?.message || 'Delete operation failed';
        this.toastService.error('Forbidden', msg);
      }
    });
  }

  getRoleBadgeClass(roleName: string): string {
    switch (roleName?.toLowerCase()) {
      case 'admin': return 'bg-purple-100 text-purple-700 ring-purple-200';
      case 'manager': return 'bg-blue-100 text-blue-700 ring-blue-200';
      case 'staff': return 'bg-emerald-100 text-emerald-700 ring-emerald-200';
      default: return 'bg-slate-100 text-slate-700 ring-slate-200';
    }
  }

  getUserWithRole(username: string | null): string {
    if (!username) return '-';
    const user = this.users.find(u => u.username?.toLowerCase() === username.toLowerCase());
    if (user && user.roles?.length > 0) {
      return `${username}(${user.roles[0]})`;
    }
    return username;
  }
}
