import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-category-list',
  standalone: false,
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss'
})
export class CategoryList implements OnInit {
  categories: any[] = [];
  filteredCategories: any[] = [];
  isLoading = false;
  searchTerm: string = '';
  
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
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.fetchCategories();
  }



  checkPermissions(): void {
    this.canCreate = this.storageService.hasPermission('CreateCategory');
    this.canEdit = this.storageService.hasPermission('UpdateCategory');
    this.canDelete = this.storageService.hasPermission('DeleteCategory');
  }

  fetchCategories(): void {
    this.isLoading = true;
    this.categoryService.getAllCategories().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status) {
          this.categories = res.data || [];
          this.applyFilter();
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch categories';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
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
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isSaving = false;
        const msg = err.error?.Error || err.error?.message || 'Operation failed';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
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


  onSearch(term: string): void {
    this.searchTerm = term;
    this.applyFilter();
  }

  applyFilter(): void {
    if (!this.searchTerm) {
      this.filteredCategories = [...this.categories];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredCategories = this.categories.filter(c => 
        c.name.toLowerCase().includes(term) || 
        (c.description && c.description.toLowerCase().includes(term))
      );
    }
  }
}
