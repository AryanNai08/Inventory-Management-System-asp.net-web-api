import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../../../core/services/toast';

@Component({
  selector: 'app-login-page',
  standalone: false,
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.status) {
          this.toastService.success('Login Successful', 'Welcome back to InvMS!');
          this.router.navigate(['/dashboard']);
        } else {
          this.toastService.error('Login Failed', response.message || 'Invalid username or password');
        }
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.message || err.error?.error || 'Invalid username or password';
        this.toastService.error('Login Failed', msg);
      }
    });
  }
}
