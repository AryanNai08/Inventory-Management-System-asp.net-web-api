import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth';
import { ToastService } from '../../../../core/services/toast';

@Component({
  selector: 'app-change-password',
  standalone: false,
  templateUrl: './change-password.html',
  styleUrl: './change-password.scss'
})
export class ChangePassword implements OnInit {
  changePassForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.changePassForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { 'mismatch': true };
  }

  onSubmit(): void {
    if (this.changePassForm.invalid) return;

    this.isLoading = true;
    const dto = {
      currentPassword: this.changePassForm.value.currentPassword,
      newPassword: this.changePassForm.value.newPassword
    };

    this.authService.changePassword(dto).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.toastService.success('Success', 'Password updated successfully');
          this.router.navigate(['/dashboard']);
        } else {
          this.toastService.error('Failed', res.message || 'Incorrect current password.');
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.Message || err.error?.message || 'Something went wrong.';
        this.toastService.error('Error', msg);
      }
    });
  }
}
