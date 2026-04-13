import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter, Subscription } from 'rxjs';
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
export class DashboardHome implements OnInit, OnDestroy {
  // Reactive Getters to always read latest permissions from StorageService
  get canManageUsers() { return this.storageService.hasPermission('ManageUsers'); }
  get canManageRoles() { return this.storageService.hasPermission('ManageRoles'); }
  get canViewCategories() { return this.storageService.hasPermission('ViewCategories'); }
  get canViewCustomers() { return this.storageService.hasPermission('ViewCustomers'); }
  get canViewWarehouses() { return this.storageService.hasPermission('ViewWarehouses'); }

  username = '';
  userCount = 0;
  roleCount = 0;
  categoryCount = 0;
  supplierCount = 0;
  customerCount = 0;
  warehouseCount = 0;
  isLoading = false;

  private routerSubscription?: Subscription;

  constructor(
    public storageService: StorageService,
    private userService: UserService,
    private categoryService: CategoryService,
    private supplierService: SupplierService,
    private customerService: CustomerService,
    private warehouseService: WarehouseService,
    private roleService: RoleService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    this.username = this.storageService.getUser()?.username || 'User';
  }

  ngOnInit(): void {
    // Since AppInitializer ensures StorageService is hydrated, we can load immediately
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    // No longer needed: subscriptions removed for simplicity and stability
  }

  loadDashboard(): void {
    // Sync identity
    this.username = this.storageService.getUser()?.username || 'User';

    // Conditional fetches based on live permissions from getters
    if (this.canManageUsers) this.fetchUserCount();
    if (this.canManageRoles) this.fetchRoleCount();
    if (this.canViewCategories) this.fetchCategoryCount();
    if (this.storageService.hasPermission('ViewSuppliers')) this.fetchSupplierCount();
    if (this.canViewCustomers) this.fetchCustomerCount();
    if (this.canViewWarehouses) this.fetchWarehouseCount();

    // Force a check if values updated immediately
    this.cd.detectChanges();
  }

  fetchRoleCount(): void {
    this.roleService.getAllRoles().subscribe({
      next: (res) => {
        if (res.status) {
          this.roleCount = res.data?.length || 0;
          this.cd.detectChanges();
        }
      }
    });
  }

  fetchWarehouseCount(): void {
    this.warehouseService.getAllWarehouses().subscribe({
      next: (res: any) => {
        if (res.status) {
          this.warehouseCount = res.data?.length || 0;
          this.cd.detectChanges();
        }
      }
    });
  }

  fetchUserCount(): void {
    this.userService.getAllUsers().subscribe({
      next: (res: any) => {
        if (res.status) {
          this.userCount = res.data?.length || 0;
          this.cd.detectChanges();
        }
      }
    });
  }

  fetchCategoryCount(): void {
    this.isLoading = true;
    this.cd.detectChanges();
    this.categoryService.getAllCategories().subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.categoryCount = res.data?.length || 0;
        }
        this.cd.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cd.detectChanges();
      }
    });
  }

  fetchSupplierCount(): void {
    const params = { pageNumber: 1, pageSize: 1 };
    this.supplierService.getAllSuppliers(params).subscribe({
      next: (res: any) => {
        if (res.status) {
          this.supplierCount = res.data?.totalCount || 0;
          this.cd.detectChanges();
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
          this.cd.detectChanges();
        }
      }
    });
  }
}
