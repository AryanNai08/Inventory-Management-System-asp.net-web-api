import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';
import { UserService } from '../../../users/services/user.service';

@Component({
  selector: 'app-dashboard-home',
  standalone: false,
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss'
})
export class DashboardHome implements OnInit {
  isAdmin = false;
  username = '';
  userCount = 0;
  isLoading = false;

  constructor(
    private storageService: StorageService,
    private userService: UserService
  ) {
    this.isAdmin = this.storageService.isAdmin();
    this.username = this.storageService.getUser()?.username || 'User';
  }

  ngOnInit(): void {
    if (this.isAdmin) {
      this.fetchUserCount();
    }
  }

  fetchUserCount(): void {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.userCount = res.data?.length || 0;
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }
}
