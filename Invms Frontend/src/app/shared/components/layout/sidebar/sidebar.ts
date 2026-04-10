import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../features/auth/services/auth';
import { StorageService } from '../../../../core/services/storage.service';

@Component({
  selector: 'app-sidebar',
  standalone: false,
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar implements OnInit {
  username: string = 'User';
  role: string = 'Staff';
  
  // Permission Flags
  canManageUsers = false;
  canRegisterUsers = false;
  canViewCategories = false;
  canViewProducts = false;
  canViewSuppliers = false;
  canViewCustomers = false;
  canViewWarehouses = false;
  canViewPurchaseOrders = false;
  canViewSalesOrders = false;
  canViewStockAdjustments = false;
  canViewReports = false;

  constructor(
    private authService: AuthService,
    private storageService: StorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const user = this.storageService.getUser();
    if (user) {
      this.username = user.fullName || user.username || 'User';
      this.role = user.roles?.[0] || 'Staff';
      this.checkPermissions();
    }
  }

  private checkPermissions(): void {
    this.canManageUsers = this.storageService.hasPermission('ManageUsers');
    this.canRegisterUsers = this.storageService.hasPermission('ManageUserRegistration');
    this.canViewCategories = this.storageService.hasPermission('ViewCategories');
    this.canViewProducts = this.storageService.hasPermission('ViewProducts');
    this.canViewSuppliers = this.storageService.hasPermission('ViewSuppliers');
    this.canViewCustomers = this.storageService.hasPermission('ViewCustomers');
    this.canViewWarehouses = this.storageService.hasPermission('ViewWarehouses');
    this.canViewPurchaseOrders = this.storageService.hasPermission('ViewPurchaseOrders');
    this.canViewSalesOrders = this.storageService.hasPermission('ViewSalesOrders');
    this.canViewStockAdjustments = this.storageService.hasPermission('ViewStockAdjustments');
    this.canViewReports = this.storageService.hasPermission('ViewReports');
  }

  onLogout(): void {
    this.authService.logout();
  }
}
