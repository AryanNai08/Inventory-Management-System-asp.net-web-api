import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ToastMessage {
  id?: number;
  type: 'success' | 'error' | 'info';
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

  show(type: 'success' | 'error' | 'info', title: string, message: string) {
    console.log(`[ToastService] New Toast: ${type} - ${title}: ${message}`);
    
    // Wrap in setTimeout to avoid NG0100 ExpressionChangedAfterItHasBeenCheckedError
    // when triggered during a component's check cycle (like Login)
    setTimeout(() => {
      this.toastSubject.next({
        id: ++this.counter,
        type,
        title,
        message
      });
    }, 0);
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
}
