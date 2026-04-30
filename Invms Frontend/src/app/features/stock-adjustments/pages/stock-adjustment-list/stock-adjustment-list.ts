import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StockAdjustmentService, StockAdjustment, AdjustmentType } from '../../services/stock-adjustment.service';
import { ProductService, Product } from '../../../products/services/product.service';
import { WarehouseService } from '../../../warehouses/services/warehouse.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { PaginationParams } from '../../../../core/models/api.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-stock-adjustment-list',
  standalone: false,
  templateUrl: './stock-adjustment-list.html',
  styleUrl: './stock-adjustment-list.scss'
})
export class StockAdjustmentList implements OnInit, OnDestroy {
  adjustments: StockAdjustment[] = [];
  adjustmentTypes: AdjustmentType[] = [];
  products: Product[] = [];
  warehouses: any[] = [];
  isLoading = false;
  isSaving = false;
  isAdmin = false;
  searchTerm: string = '';

  // RBAC
  canView = false;
  canManage = false;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;
  hasNextPage = false;
  hasPreviousPage = false;

  // Modal/Drawer
  isModalOpen = false;
  isDetailOpen = false;
  adjustmentForm: FormGroup;
  selectedAdjustmentDetail: StockAdjustment | null = null;
  
  // Context Aware
  currentStock: number | null = null;
  isLoadingStock = false;

  private destroy$ = new Subject<void>();

  constructor(
    private stockService: StockAdjustmentService,
    private productService: ProductService,
    private warehouseService: WarehouseService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.adjustmentForm = this.fb.group({
      productId: ['', [Validators.required]],
      warehouseId: ['', [Validators.required]],
      adjustmentTypeId: ['', [Validators.required]],
      direction: ['increase', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      reason: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.fetchAdjustments();
    this.loadDropdowns();
    this.setupFormSubscribers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    document.body.classList.remove('no-scroll');
  }

  checkPermissions(): void {
    this.canView = this.storageService.hasPermission('ViewStockAdjustments');
    this.canManage = this.storageService.hasPermission('ManageStockAdjustments');
    this.isAdmin = this.storageService.isAdmin();
  }

  loadDropdowns(): void {
    this.productService.searchProducts().subscribe(res => {
      if (res.status) this.products = res.data || [];
    });
    this.warehouseService.getAllWarehouses().subscribe(res => {
      if (res.status) this.warehouses = res.data || [];
    });
    this.stockService.getAdjustmentTypes().subscribe(res => {
      if (res.status) this.adjustmentTypes = res.data || [];
    });
  }

  setupFormSubscribers(): void {
    // Monitor Product + Warehouse to show current stock
    this.adjustmentForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(vals => {
        const { productId, warehouseId } = vals;
        if (productId && warehouseId) {
          this.fetchCurrentStock(productId, warehouseId);
        } else {
          this.currentStock = null;
        }
      });
  }

  fetchCurrentStock(productId: number, warehouseId: number): void {
    this.isLoadingStock = true;
    this.warehouseService.getWarehouseStock(warehouseId).subscribe({
      next: (res) => {
        this.isLoadingStock = false;
        if (res.status && res.data) {
          const productStock = res.data.find((s: any) => s.productId == productId);
          this.currentStock = productStock ? productStock.quantity : 0;
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingStock = false;
        this.currentStock = 0;
        this.cdr.detectChanges();
      }
    });
  }

  fetchAdjustments(): void {
    if (!this.canView) return;
    this.isLoading = true;
    const params: PaginationParams = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: 'Id',
      sortOrder: 'desc',
      searchTerm: this.searchTerm
    };

    this.stockService.getAllAdjustments(params).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status && res.data) {
          this.adjustments = res.data.items || [];
          this.totalCount = res.data.totalCount;
          this.totalPages = res.data.totalPages;
          this.hasNextPage = res.data.hasNextPage;
          this.hasPreviousPage = res.data.hasPreviousPage;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.toastService.error('Error', 'Failed to fetch stock adjustments');
        this.cdr.detectChanges();
      }
    });
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    this.currentPage = 1;
    this.fetchAdjustments();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchAdjustments();
  }

  openCreateModal(): void {
    this.adjustmentForm.reset({ direction: 'increase', quantity: 1 });
    this.currentStock = null;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  openDetail(adjustment: StockAdjustment): void {
    this.selectedAdjustmentDetail = adjustment;
    this.isDetailOpen = true;
    document.body.classList.add('no-scroll');
  }

  closeDetail(): void {
    this.isDetailOpen = false;
    this.selectedAdjustmentDetail = null;
    document.body.classList.remove('no-scroll');
  }

  onSubmit(): void {
    if (this.adjustmentForm.invalid) {
      this.adjustmentForm.markAllAsTouched();
      return;
    }

    const { direction, quantity, productId, warehouseId, adjustmentTypeId, reason } = this.adjustmentForm.value;
    const signedQuantity = direction === 'increase' ? quantity : -quantity;

    // Prevention of negative stock
    if (this.currentStock !== null && (this.currentStock + signedQuantity < 0)) {
      this.toastService.error('Invalid Adjustment', 'Resulting stock cannot be negative.');
      return;
    }

    // Confirmation for large adjustments
    if (quantity > 50) {
      if (!confirm(`Are you sure you want to ${direction} stock by ${quantity} units? This is a large adjustment.`)) {
        return;
      }
    }

    this.isSaving = true;
    const dto = {
      productId,
      warehouseId,
      adjustmentTypeId,
      quantityChange: signedQuantity,
      reason
    };

    this.stockService.createAdjustment(dto).subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', 'Stock adjustment recorded successfully');
          this.closeModal();
          this.fetchAdjustments();
        }
      },
      error: (err) => {
        this.isSaving = false;
        const msg = err.error?.message || 'Failed to create adjustment';
        this.toastService.error('Error', msg);
      }
    });
  }

  getQtyClass(qty: number): string {
    return qty > 0 ? 'text-emerald-600 font-black' : 'text-red-600 font-black';
  }
}
