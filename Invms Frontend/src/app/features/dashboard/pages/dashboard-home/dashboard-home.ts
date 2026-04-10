import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';
import { UserService } from '../../../users/services/user.service';
import { CategoryService } from '../../../categories/services/category.service';

@Component({
  selector: 'app-dashboard-home',
  standalone: false,
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss'
})
export class DashboardHome implements OnInit {
  canManageUsers = false;
  canViewCategories = false;
  
  username = '';
  userCount = 0;
  categoryCount = 0;
  isLoading = false;

  constructor(
    private storageService: StorageService,
    private userService: UserService,
    private categoryService: CategoryService
  ) {
    this.checkPermissions();
    this.username = this.storageService.getUser()?.username || 'User';
  }

  checkPermissions(): void {
    this.canManageUsers = this.storageService.hasPermission('ManageUsers');
    this.canViewCategories = this.storageService.hasPermission('ViewCategories');
  }

  ngOnInit(): void {
    if (this.canManageUsers) {
      this.fetchUserCount();
    }
    if (this.canViewCategories) {
      this.fetchCategoryCount();
    }
  }

  fetchUserCount(): void {
    this.userService.getAllUsers().subscribe({
      next: (res: any) => {
        if (res.status) {
          this.userCount = res.data?.length || 0;
        }
      }
    });
  }

  fetchCategoryCount(): void {
    this.isLoading = true;
    this.categoryService.getAllCategories().subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.categoryCount = res.data?.length || 0;
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }
}
