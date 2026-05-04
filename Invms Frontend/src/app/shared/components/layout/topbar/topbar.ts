import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';
import { AuthService } from '../../../../features/auth/services/auth';
import { LayoutService } from '../../../../core/services/layout';

@Component({
  selector: 'app-topbar',
  standalone: false,
  templateUrl: './topbar.html',
  styleUrl: './topbar.scss'
})
export class Topbar implements OnInit {
  username: string = 'User';
  role: string = 'Staff';
  showMenu: boolean = false;
  showNavMenu: boolean = false;

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
  canManageRoles = false;

  constructor(
    private storageService: StorageService,
    private authService: AuthService,
    private layoutService: LayoutService
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
    this.canManageRoles = this.storageService.hasPermission('ManageRoles');
  }

  toggleMenu(): void {
    this.showMenu = !this.showMenu;
  }

  toggleSidebar(): void {
    this.layoutService.toggleSidebar();
  }

  onLogout(): void {
    this.authService.logout();
  }
}
