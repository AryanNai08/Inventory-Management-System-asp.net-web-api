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
  isAdmin: boolean = false;

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
      this.isAdmin = this.storageService.isAdmin();
    }
  }

  onLogout(): void {
    this.authService.logout();
  }
}
