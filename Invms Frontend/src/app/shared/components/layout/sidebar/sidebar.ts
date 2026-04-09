import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../../features/auth/services/auth';
import { StorageService } from '../../../../core/services/storage.service';

@Component({
  selector: 'app-sidebar',
  standalone: false,
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar implements OnInit {
  username: string = '';
  role: string = '';

  constructor(
    private authService: AuthService,
    private storageService: StorageService
  ) {}

  ngOnInit(): void {
    const user = this.storageService.getUser();
    if (user) {
      this.username = user.username;
      // Show first role or Staff as default
      this.role = user.roles && user.roles.length > 0 ? user.roles[0] : 'Staff';
    }
  }

  onLogout(): void {
    this.authService.logout();
  }
}
