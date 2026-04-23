import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { PaginationParams } from '../../../../core/models/api.model';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-customer-list',
  standalone: false,
  templateUrl: './customer-list.html',
  styleUrl: './customer-list.scss'
})
export class CustomerList implements OnInit {
  customers: any[] = [];
  isLoading = false;
  
  // PBAC Flags
  canView = false;
  canManage = false;
  canDelete = false;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;
  hasNextPage = false;
  hasPreviousPage = false;

  // Modal
  isModalOpen = false;
  customerForm: FormGroup;
  selectedCustomerId: number | null = null;
  isSaving = false;

  constructor(
    private customerService: CustomerService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.customerForm = this.fb.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^[0-9]{10}$')]],
      address: [''],
      city: ['']
    });
  }

  ngOnInit(): void {
    this.fetchCustomers();
  }

  checkPermissions(): void {
    this.canView = this.storageService.hasPermission('ViewCustomers');
    this.canManage = this.storageService.hasPermission('ManageCustomers');
    this.canDelete = this.storageService.hasPermission('DeleteCustomers');
  }

  fetchCustomers(): void {
    if (!this.canView) return;
    this.isLoading = true;
    const params: PaginationParams = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: 'Name',
      sortOrder: 'asc'
    };

    this.customerService.getAllCustomers(params).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status && res.data) {
          const paginated = res.data;
          this.customers = paginated.items || [];
          this.totalCount = paginated.totalCount;
          this.totalPages = paginated.totalPages;
          this.hasNextPage = paginated.hasNextPage;
          this.hasPreviousPage = paginated.hasPreviousPage;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch customers';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchCustomers();
  }

  openCreateModal(): void {
    this.selectedCustomerId = null;
    this.customerForm.reset();
    this.isModalOpen = true;
  }

  openEditModal(customer: any): void {
    this.selectedCustomerId = customer.id;
    this.customerForm.patchValue({
      name: customer.name,
      email: customer.email,
      phone: customer.phone,
      address: customer.address,
      city: customer.city
    });
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedCustomerId = null;
    this.customerForm.reset();
  }

  onSubmit(): void {
    if (this.customerForm.invalid) {
      this.customerForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const request = this.selectedCustomerId 
      ? this.customerService.updateCustomer(this.selectedCustomerId, this.customerForm.value)
      : this.customerService.createCustomer(this.customerForm.value);

    request.subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', `Customer ${this.selectedCustomerId ? 'updated' : 'created'} successfully`);
          this.closeModal();
          this.fetchCustomers();
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
    if (!confirm('Are you sure you want to delete this customer? This action cannot be undone.')) {
      return;
    }

    this.customerService.deleteCustomer(id).subscribe({
      next: (res) => {
        if (res.status) {
          this.toastService.success('Deleted', 'Customer removed successfully');
          this.fetchCustomers();
        } else {
          this.toastService.error('Error', res.error || 'Failed to delete customer');
        }
      },
      error: (err) => {
        const msg = err.error?.Error || err.error?.message || 'Delete operation failed';
        this.toastService.error('Error', msg);
      }
    });
  }
}
