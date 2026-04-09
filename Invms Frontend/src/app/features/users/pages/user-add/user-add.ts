import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth';
import { ToastService } from '../../../../core/services/toast';

@Component({
  selector: 'app-user-add',
  standalone: false,
  templateUrl: './user-add.html',
  styleUrl: './user-add.scss'
})
export class UserAdd implements OnInit {
  userForm!: FormGroup;
  isLoading = false;
  roles = [
    { id: 4, name: 'Admin' },
    { id: 5, name: 'Manager' },
    { id: 6, name: 'Staff' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required, Validators.minLength(4)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      roleId: [3, Validators.required] // Default to Staff
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.authService.register(this.userForm.value).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status) {
          this.toastService.success('Success', 'User created successfully');
          this.router.navigate(['/dashboard']);
        } else {
          this.toastService.error('Error', res.message || 'Failed to create user');
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        // Strictly use backend Error/Message or a clean default. 
        // Avoid err.message which contains technical "Http failure response" strings.
        const msg = err.error?.Error || err.error?.Message || err.error?.message || 'Failed to create user. Please check your data.';
        this.toastService.error('Registration Failed', msg);
      }
    });
  }
}
