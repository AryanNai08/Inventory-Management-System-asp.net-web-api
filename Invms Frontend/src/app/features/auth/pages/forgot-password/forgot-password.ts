import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../../../core/services/toast';

@Component({
  selector: 'app-forgot-password',
  standalone: false,
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPassword implements OnInit {
  forgotForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotForm.invalid) return;

    this.isLoading = true;
    this.authService.forgotPassword(this.forgotForm.value.email).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.toastService.success('OTP Sent', 'An OTP has been sent to your email address.');
          this.router.navigate(['/auth/reset-password'], { queryParams: { email: this.forgotForm.value.email } });
        } else {
          this.toastService.error('Request Failed', res.message || 'Unable to send OTP.');
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.Message || err.error?.message || 'Something went wrong. Please try again.';
        this.toastService.error('Error', msg);
      }
    });
  }
}
