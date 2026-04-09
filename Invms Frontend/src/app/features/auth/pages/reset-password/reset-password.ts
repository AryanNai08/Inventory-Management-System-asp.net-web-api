import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../../../core/services/toast';

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss'
})
export class ResetPassword implements OnInit {
  resetForm!: FormGroup;
  isLoading = false;
  email: string = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams['email'] || '';
    
    this.resetForm = this.fb.group({
      email: [this.email, [Validators.required, Validators.email]],
      otp: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { 'mismatch': true };
  }

  onSubmit(): void {
    if (this.resetForm.invalid) return;

    this.isLoading = true;
    const dto = {
      email: this.resetForm.value.email,
      otp: this.resetForm.value.otp,
      newPassword: this.resetForm.value.newPassword
    };

    this.authService.resetPassword(dto).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.toastService.success('Success', 'Your password has been reset successfully. Please login.');
          this.router.navigate(['/auth/login']);
        } else {
          this.toastService.error('Reset Failed', res.message || 'Invalid OTP or session expired.');
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.Message || err.error?.message || 'Failed to reset password. Please try again.';
        this.toastService.error('Error', msg);
      }
    });
  }
}
