import { Component, OnInit } from '@angular/core';
import { LayoutService } from '../../../../core/services/layout';
import { Observable } from 'rxjs';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-app-layout',
  standalone: false,
  templateUrl: './app-layout.html',
  styleUrl: './app-layout.scss',
})
export class AppLayout implements OnInit {
  isSidebarOpen$: Observable<boolean>;

  constructor(
    private layoutService: LayoutService,
    private router: Router
  ) {
    this.isSidebarOpen$ = this.layoutService.sidebarOpen$;
  }

  ngOnInit(): void {
    // Close sidebar on navigation if we are on a small screen
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      if (window.innerWidth < 768) { // md breakpoint is 768px
        this.layoutService.setSidebarState(false);
      }
    });
  }

  toggleSidebar(): void {
    this.layoutService.toggleSidebar();
  }
}
