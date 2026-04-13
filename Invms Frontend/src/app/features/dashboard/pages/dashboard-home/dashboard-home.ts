import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';
import { UserService } from '../../../users/services/user.service';
import { CategoryService } from '../../../categories/services/category.service';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { CustomerService } from '../../../customers/services/customer.service';
import { WarehouseService } from '../../../warehouses/services/warehouse.service';
import { RoleService } from '../../../roles/services/role.service';

@Component({
  selector: 'app-dashboard-home',
  standalone: false,
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss'
})
export class DashboardHome implements OnInit {
  canManageUsers = false;
  canManageRoles = false;
  canViewCategories = false;
  canViewCustomers = false;
  canViewWarehouses = false;
  
  username = '';
  userCount = 0;
  roleCount = 0;
  categoryCount = 0;
  supplierCount = 0;
  customerCount = 0;
  warehouseCount = 0;
  isLoading = false;

  constructor(
    public storageService: StorageService,
    private userService: UserService,
    private categoryService: CategoryService,
    private supplierService: SupplierService,
    private customerService: CustomerService,
    private warehouseService: WarehouseService,
    private roleService: RoleService
  ) {
    this.checkPermissions();
    this.username = this.storageService.getUser()?.username || 'User';
  }

  checkPermissions(): void {
    this.canManageUsers = this.storageService.hasPermission('ManageUsers');
    this.canManageRoles = this.storageService.hasPermission('ManageRoles');
    this.canViewCategories = this.storageService.hasPermission('ViewCategories');
    this.canViewCustomers = this.storageService.hasPermission('ViewCustomers');
    this.canViewWarehouses = this.storageService.hasPermission('ViewWarehouses');
  }

  ngOnInit(): void {
    if (this.canManageUsers) {
      this.fetchUserCount();
    }
    if (this.canManageRoles) {
      this.fetchRoleCount();
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
    if (this.canViewWarehouses) {
      this.fetchWarehouseCount();
    }
  }

  fetchRoleCount(): void {
    this.roleService.getAllRoles().subscribe({
      next: (res) => {
        if (res.status) {
          this.roleCount = res.data?.length || 0;
        }
      }
    });
  }

  fetchWarehouseCount(): void {
    this.warehouseService.getAllWarehouses().subscribe({
      next: (res: any) => {
        if (res.status) {
          this.warehouseCount = res.data?.length || 0;
        }
      }
    });
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
    const params = { pageNumber: 1, pageSize: 1 };
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
