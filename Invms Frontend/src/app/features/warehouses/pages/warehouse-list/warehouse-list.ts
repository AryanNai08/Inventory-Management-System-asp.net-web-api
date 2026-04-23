import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WarehouseService } from '../../services/warehouse.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-warehouse-list',
  standalone: false,
  templateUrl: './warehouse-list.html',
  styleUrl: './warehouse-list.scss'
})
export class WarehouseList implements OnInit {
  warehouses: any[] = [];
  isLoading = false;
  
  // Permission Flags
  canView = false;
  canManage = false;
  canDelete = false;

  // Modal
  isModalOpen = false;
  warehouseForm: FormGroup;
  selectedWarehouseId: number | null = null;
  isSaving = false;

  constructor(
    private warehouseService: WarehouseService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.warehouseForm = this.fb.group({
      name: ['', [Validators.required]],
      location: ['', [Validators.required]],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.fetchWarehouses();
  }

  checkPermissions(): void {
    this.canView = this.storageService.hasPermission('ViewWarehouses');
    this.canManage = this.storageService.hasPermission('ManageWarehouses');
    this.canDelete = this.storageService.hasPermission('DeleteWarehouses');
  }

  fetchWarehouses(): void {
    if (!this.canView) return;
    this.isLoading = true;
    this.warehouseService.getAllWarehouses().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status && res.data) {
          this.warehouses = res.data;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch warehouses';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  openCreateModal(): void {
    this.selectedWarehouseId = null;
    this.warehouseForm.reset();
    this.isModalOpen = true;
  }

  openEditModal(warehouse: any): void {
    this.selectedWarehouseId = warehouse.id;
    this.warehouseForm.patchValue({
      name: warehouse.name,
      location: warehouse.location,
      description: warehouse.description
    });
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedWarehouseId = null;
    this.warehouseForm.reset();
  }

  onSubmit(): void {
    if (this.warehouseForm.invalid) {
      this.warehouseForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const request = this.selectedWarehouseId 
      ? this.warehouseService.updateWarehouse(this.selectedWarehouseId, this.warehouseForm.value)
      : this.warehouseService.createWarehouse(this.warehouseForm.value);

    request.subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', `Warehouse ${this.selectedWarehouseId ? 'updated' : 'created'} successfully`);
          this.closeModal();
          this.fetchWarehouses();
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
    if (!confirm('Are you sure you want to delete this warehouse? This action cannot be undone.')) {
      return;
    }

    this.warehouseService.deleteWarehouse(id).subscribe({
      next: (res) => {
        if (res.status) {
          this.toastService.success('Deleted', 'Warehouse removed successfully');
          this.fetchWarehouses();
        } else {
          this.toastService.error('Error', res.error || 'Failed to delete warehouse');
        }
      },
      error: (err) => {
        const msg = err.error?.Error || err.error?.message || 'Delete operation failed';
        this.toastService.error('Error', msg);
      }
    });
  }
}
