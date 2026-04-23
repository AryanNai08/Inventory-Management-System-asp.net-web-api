import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastService, ToastMessage } from '../../../../core/services/toast';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast-container',
  standalone: false,
  templateUrl: './toast-container.html',
  styleUrl: './toast-container.scss'
})
export class ToastContainer implements OnInit, OnDestroy {
  toasts: ToastMessage[] = [];
  private subscription?: Subscription;

  constructor(
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    console.log('[ToastContainer] Initialized');
    this.subscription = this.toastService.toasts$.subscribe(toast => {
      console.log('[ToastContainer] Received new toast:', toast);
      // Use array spread to ensure reference change for Change Detection
      this.toasts = [...this.toasts, toast];
      
      // Auto-remove after 5 seconds
      setTimeout(() => this.remove(toast), 5000);
    });
  }

  remove(toast: ToastMessage): void {
    this.toasts = this.toasts.filter(t => t.id !== toast.id);
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
