import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../../../core/services/storage.service';

@Component({
  selector: 'app-topbar',
  standalone: false,
  templateUrl: './topbar.html',
  styleUrl: './topbar.scss',
})
export class Topbar implements OnInit {
  username: string = '';
  role: string = '';

  constructor(private storageService: StorageService) {}

  ngOnInit(): void {
    const user = this.storageService.getUser();
    if (user) {
      this.username = user.username;
      this.role = user.roles && user.roles.length > 0 ? user.roles[0] : 'Staff';
    }
  }
}
