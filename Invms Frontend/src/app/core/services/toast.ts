import { Injectable, NgZone } from '@angular/core';
import { Subject } from 'rxjs';

export interface ToastMessage {
  id?: number;
  type: 'success' | 'error' | 'info' | 'warning';
  title: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastSubject = new Subject<ToastMessage>();
  toasts$ = this.toastSubject.asObservable();
  private counter = 0;

  constructor(private ngZone: NgZone) {}

  show(type: 'success' | 'error' | 'info' | 'warning', title: string, message: string) {
    console.log(`[ToastService] New Toast: ${type} - ${title}: ${message}`);
    
    // Use setTimeout to push to the next tick, avoiding ExpressionChangedAfterItHasBeenCheckedError
    setTimeout(() => {
      this.ngZone.run(() => {
        this.toastSubject.next({
          id: ++this.counter,
          type,
          title,
          message
        });
      });
    });
  }

  success(title: string, message: string) {
    this.show('success', title, message);
  }

  error(title: string, message: string) {
    this.show('error', title, message);
  }

  info(title: string, message: string) {
    this.show('info', title, message);
  }

  warning(title: string, message: string) {
    this.show('warning', title, message);
  }
}
