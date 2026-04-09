import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';

@Component({
  selector: 'app-user-list',
  standalone: false,
  templateUrl: './user-list.html',
  styleUrl: './user-list.scss'
})
export class UserList implements OnInit {
  users: any[] = [];
  isLoading = false;
  isAdmin = false;

  constructor(
    private userService: UserService,
    private toastService: ToastService,
    private storageService: StorageService
  ) {
    this.isAdmin = this.storageService.isAdmin();
  }

  ngOnInit(): void {
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
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch users';
        this.toastService.error('Error', msg);
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
          this.fetchUsers(); // Refresh list
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
}
