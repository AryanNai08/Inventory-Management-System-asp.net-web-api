import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';
import { AuthService } from '../../../../features/auth/services/auth';

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

  constructor(
    private storageService: StorageService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const user = this.storageService.getUser();
    if (user) {
      this.username = user.fullName || user.username || 'User';
      this.role = user.roles?.[0] || 'Staff';
    }
  }

  toggleMenu(): void {
    this.showMenu = !this.showMenu;
  }

  onLogout(): void {
    this.authService.logout();
  }
}
