import { Component, OnInit } from '@angular/core';
import { SupplierService } from '../../services/supplier.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PaginationParams } from '../../../../core/models/api.model';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-supplier-list',
  standalone: false,
  templateUrl: './supplier-list.html',
  styleUrl: './supplier-list.scss'
})
export class SupplierList implements OnInit {
  suppliers: any[] = [];
  isLoading = false;
  
  // PBAC Flags
  canViewSuppliers = false;
  canManage = false;
  canDelete = false;

  // Pagination State
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;
  hasNextPage = false;
  hasPreviousPage = false;

  // Modal State
  isModalOpen = false;
  supplierForm: FormGroup;
  selectedSupplierId: number | null = null;
  isSaving = false;

  constructor(
    private supplierService: SupplierService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.supplierForm = this.fb.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^[0-9]{10}$')]],
      contactPerson: [''],
      address: [''],
      city: ['']
    });
  }

  ngOnInit(): void {
    this.fetchSuppliers();
  }

  checkPermissions(): void {
    this.canViewSuppliers = this.storageService.hasPermission('ViewSuppliers');
    this.canManage = this.storageService.hasPermission('ManageSuppliers');
    this.canDelete = this.storageService.hasPermission('DeleteSuppliers');
  }

  fetchSuppliers(): void {
    if (!this.canViewSuppliers) return;
    this.isLoading = true;
    const params: PaginationParams = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: 'Name',
      sortOrder: 'asc'
    };

    this.supplierService.getAllSuppliers(params).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status && res.data) {
          const paginated = res.data;
          this.suppliers = paginated.items || [];
          this.totalCount = paginated.totalCount;
          this.totalPages = paginated.totalPages;
          this.hasNextPage = paginated.hasNextPage;
          this.hasPreviousPage = paginated.hasPreviousPage;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch suppliers';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchSuppliers();
  }

  openCreateModal(): void {
    this.selectedSupplierId = null;
    this.supplierForm.reset();
    this.isModalOpen = true;
  }

  openEditModal(supplier: any): void {
    this.selectedSupplierId = supplier.id;
    this.supplierForm.patchValue({
      name: supplier.name,
      email: supplier.email,
      phone: supplier.phone,
      contactPerson: supplier.contactPerson,
      address: supplier.address,
      city: supplier.city
    });
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedSupplierId = null;
    this.supplierForm.reset();
  }

  onSubmit(): void {
    if (this.supplierForm.invalid) {
      this.supplierForm.markAllAsTouched();
       return;
    }

    this.isSaving = true;
    const request = this.selectedSupplierId 
      ? this.supplierService.updateSupplier(this.selectedSupplierId, this.supplierForm.value)
      : this.supplierService.createSupplier(this.supplierForm.value);

    request.subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', `Supplier ${this.selectedSupplierId ? 'updated' : 'created'} successfully`);
          this.closeModal();
          this.fetchSuppliers();
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
    if (!confirm('Are you sure you want to delete this supplier? This action cannot be undone.')) {
      return;
    }

    this.supplierService.deleteSupplier(id).subscribe({
      next: (res) => {
        if (res.status) {
          this.toastService.success('Deleted', 'Supplier removed successfully');
          this.fetchSuppliers();
        } else {
          this.toastService.error('Error', res.error || 'Failed to delete supplier');
        }
      },
      error: (err) => {
        const msg = err.error?.Error || err.error?.message || 'Delete operation failed';
        this.toastService.error('Error', msg);
      }
    });
  }
}
