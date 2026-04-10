import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';
import { UserService } from '../../../users/services/user.service';
import { CategoryService } from '../../../categories/services/category.service';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { CustomerService } from '../../../customers/services/customer.service';

@Component({
  selector: 'app-dashboard-home',
  standalone: false,
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss'
})
export class DashboardHome implements OnInit {
  canManageUsers = false;
  canViewCategories = false;
  canViewCustomers = false;
  
  username = '';
  userCount = 0;
  categoryCount = 0;
  supplierCount = 0;
  customerCount = 0;
  isLoading = false;

  constructor(
    public storageService: StorageService,
    private userService: UserService,
    private categoryService: CategoryService,
    private supplierService: SupplierService,
    private customerService: CustomerService
  ) {
    this.checkPermissions();
    this.username = this.storageService.getUser()?.username || 'User';
  }

  checkPermissions(): void {
    this.canManageUsers = this.storageService.hasPermission('ManageUsers');
    this.canViewCategories = this.storageService.hasPermission('ViewCategories');
    this.canViewCustomers = this.storageService.hasPermission('ViewCustomers');
  }

  ngOnInit(): void {
    if (this.canManageUsers) {
      this.fetchUserCount();
    }
    if (this.canViewCategories) {
      this.fetchCategoryCount();
    }
    if (this.storageService.hasPermission('ViewSuppliers')) {
      this.fetchSupplierCount();
    }
    if (this.canViewCustomers) {
      this.fetchCustomerCount();
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

  fetchSupplierCount(): void {
    // We can use the service with a small page size just to get the totalCount
    const params = { pageNumber: 1, pageSize: 1 };
    
    // Lazy inject or just add to constructor. I will add to constructor in next chunk.
    this.supplierService.getAllSuppliers(params).subscribe({
      next: (res: any) => {
        if (res.status) {
          this.supplierCount = res.data?.totalCount || 0;
        }
      }
    });
  }

  fetchCustomerCount(): void {
    const params = { pageNumber: 1, pageSize: 1 };
    this.customerService.getAllCustomers(params).subscribe({
      next: (res: any) => {
        if (res.status) {
          this.customerCount = res.data?.totalCount || 0;
        }
      }
    });
  }
}
