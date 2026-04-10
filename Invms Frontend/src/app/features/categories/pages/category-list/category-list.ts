import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { UserService } from '../../../users/services/user.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-category-list',
  standalone: false,
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss'
})
export class CategoryList implements OnInit {
  categories: any[] = [];
  users: any[] = [];
  isLoading = false;
  
  // Role Access
  canCreate = false;
  canEdit = false;
  canDelete = false;

  // Modal State
  isModalOpen = false;
  categoryForm: FormGroup;
  selectedCategoryId: number | null = null;
  isSaving = false;

  constructor(
    private categoryService: CategoryService,
    private userService: UserService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder
  ) {
    this.checkPermissions();
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.fetchCategories();
    // Only fetch users if current user has access (Admin only)
    if (this.storageService.isAdmin()) {
      this.fetchUsers();
    }
  }

  fetchUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        if (res.status) {
          this.users = res.data || [];
        }
      },
      error: () => {
        // Silently ignore - role lookup is a nice-to-have
      }
    });
  }

  checkPermissions(): void {
    const roles = this.storageService.getRoles().map(r => r.toUpperCase());
    this.canCreate = roles.includes('ADMIN') || roles.includes('MANAGER');
    this.canEdit = roles.includes('ADMIN') || roles.includes('MANAGER');
    this.canDelete = roles.includes('ADMIN');
  }

  fetchCategories(): void {
    this.isLoading = true;
    this.categoryService.getAllCategories().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status) {
          this.categories = res.data || [];
        }
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch categories';
        this.toastService.error('Error', msg);
      }
    });
  }

  openCreateModal(): void {
    this.selectedCategoryId = null;
    this.categoryForm.reset();
    this.isModalOpen = true;
  }

  openEditModal(category: any): void {
    this.selectedCategoryId = category.id;
    this.categoryForm.patchValue({
      name: category.name,
      description: category.description
    });
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedCategoryId = null;
    this.categoryForm.reset();
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) return;

    this.isSaving = true;
    const request = this.selectedCategoryId 
      ? this.categoryService.updateCategory(this.selectedCategoryId, this.categoryForm.value)
      : this.categoryService.createCategory(this.categoryForm.value);

    request.subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', `Category ${this.selectedCategoryId ? 'updated' : 'created'} successfully`);
          this.closeModal();
          this.fetchCategories();
        } else {
          this.toastService.error('Failed', res.error || 'Operation failed');
        }
      },
      error: (err) => {
        this.isSaving = false;
        const msg = err.error?.Error || err.error?.message || 'Operation failed';
        this.toastService.error('Error', msg);
      }
    });
  }

  onDelete(id: number): void {
    if (!confirm('Are you sure you want to delete this category? This will fail if products are linked.')) {
      return;
    }

    this.categoryService.deleteCategory(id).subscribe({
      next: (res) => {
        if (res.status) {
          this.toastService.success('Deleted', 'Category removed successfully');
          this.fetchCategories();
        } else {
          this.toastService.error('Action Restricted', res.error || 'Linked products detected');
        }
      },
      error: (err) => {
        const msg = err.error?.Error || err.error?.message || 'Delete operation failed';
        this.toastService.error('Error', msg);
      }
    });
  }

  getUserWithRole(username: string | null): string {
    if (!username) return '-';
    const user = this.users.find(u => u.username?.toLowerCase() === username.toLowerCase());
    if (user && user.roles?.length > 0) {
      return `${username}(${user.roles[0]})`;
    }
    return username;
  }
}
