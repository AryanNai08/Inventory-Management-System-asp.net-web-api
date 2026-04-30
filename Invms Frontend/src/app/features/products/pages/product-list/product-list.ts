import { Component, OnInit, ChangeDetectorRef, HostListener, OnDestroy } from '@angular/core';
import { ProductService, Product } from '../../services/product.service';
import { CategoryService } from '../../../categories/services/category.service';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PaginationParams } from '../../../../core/models/api.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductList implements OnInit, OnDestroy {
  products: Product[] = [];
  categories: any[] = [];
  suppliers: any[] = [];
  isLoading = false;
  
  // RBAC Flags
  canView = false;
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
  productForm: FormGroup;
  selectedProductId: number | null = null;
  isSaving = false;
  currentRowVersion: any = null;

  // Search State
  searchTerm: string = '';

  // Detail View State
  selectedProductForDetail: Product | null = null;
  isDetailOpen = false;
  isAdmin = false;

  // Breakdown State
  stockBreakdown: any = null;
  isLoadingBreakdown = false;
  lastBreakdownProductId: number | null = null;

  @HostListener('window:keydown.escape')
  onEscapePressed() {
    this.closeDetail();
  }

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private supplierService: SupplierService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.productForm = this.fb.group({
      name: ['', [Validators.required]],
      sku: ['', [Validators.required]],
      description: [''],
      purchasePrice: [0, [Validators.required, Validators.min(0)]],
      salePrice: [0, [Validators.required, Validators.min(0)]],
      reorderLevel: [10, [Validators.required, Validators.min(0)]],
      categoryId: ['', [Validators.required]],
      supplierId: ['', [Validators.required]]
    }, { validators: this.priceValidator });
  }

  priceValidator(group: FormGroup): any {
    const purchase = group.get('purchasePrice')?.value;
    const sale = group.get('salePrice')?.value;
    return sale > purchase ? null : { priceInvalid: true };
  }

  ngOnInit(): void {
    this.fetchProducts();
    this.loadDropdowns();
  }

  ngOnDestroy(): void {
    document.body.classList.remove('no-scroll');
  }

  checkPermissions(): void {
    this.canView = this.storageService.hasPermission('ViewProducts');
    this.canManage = this.storageService.hasPermission('ManageProducts');
    this.canDelete = this.storageService.hasPermission('DeleteProducts');
    this.isAdmin = this.storageService.isAdmin();
  }

  loadDropdowns(): void {
    this.categoryService.getAllCategories().subscribe((res: any) => {
      if (res.status) this.categories = res.data || [];
    });
    this.supplierService.getAllSuppliers({ pageNumber: 1, pageSize: 100, sortColumn: 'Name', sortOrder: 'asc' }).subscribe((res: any) => {
      if (res.status) this.suppliers = res.data?.items || [];
    });
  }

  fetchProducts(): void {
    if (!this.canView) return;
    setTimeout(() => this.isLoading = true);
    const params: PaginationParams = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: 'Id',
      sortOrder: 'desc',
      searchTerm: this.searchTerm
    };

    this.productService.getAllProducts(params).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status && res.data) {
          const paginated = res.data;
          this.products = paginated.items || [];
          this.totalCount = paginated.totalCount;
          this.totalPages = paginated.totalPages;
          this.hasNextPage = paginated.hasNextPage;
          this.hasPreviousPage = paginated.hasPreviousPage;
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        const msg = err.error?.Error || err.error?.message || 'Failed to fetch products';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    this.currentPage = 1;
    this.fetchProducts();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchProducts();
  }

  openCreateModal(): void {
    this.selectedProductId = null;
    this.productForm.reset({ purchasePrice: 0, salePrice: 0, reorderLevel: 10 });
    this.isModalOpen = true;
  }

  openEditModal(product: Product): void {
    console.log('Loading product into form:', product);
    // Close detail drawer first if it's open to prevent modal being blocked
    this.closeDetail();
    
    this.selectedProductId = product.id;
    this.currentRowVersion = product.rowVersion;
    this.productForm.patchValue({
      name: product.name,
      sku: product.sku,
      description: product.description,
      purchasePrice: product.purchasePrice,
      salePrice: product.salePrice,
      reorderLevel: product.reorderLevel,
      categoryId: product.categoryId,
      supplierId: product.supplierId
    });
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedProductId = null;
    this.productForm.reset();
  }

  // Detail View Logic
  openDetail(product: Product): void {
    this.selectedProductForDetail = product;
    this.isDetailOpen = true;
    document.body.classList.add('no-scroll');
    this.fetchStockBreakdown(product.id);
    this.cdr.detectChanges();
  }

  fetchStockBreakdown(productId: number): void {
    if (this.lastBreakdownProductId === productId && this.stockBreakdown) {
      return; // Use cached
    }

    this.isLoadingBreakdown = true;
    this.stockBreakdown = null;
    this.productService.getProductStockBreakdown(productId).subscribe({
      next: (res) => {
        this.isLoadingBreakdown = false;
        if (res.status) {
          this.stockBreakdown = res.data;
          this.lastBreakdownProductId = productId;
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoadingBreakdown = false;
        this.cdr.detectChanges();
      }
    });
  }

  closeDetail(): void {
    this.isDetailOpen = false;
    this.selectedProductForDetail = null;
    this.stockBreakdown = null;
    this.lastBreakdownProductId = null;
    document.body.classList.remove('no-scroll');
    this.cdr.detectChanges();
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const formData = { ...this.productForm.value };
    
    if (this.selectedProductId) {
      formData.rowVersion = this.currentRowVersion;
    }

    const request: Observable<any> = this.selectedProductId 
      ? this.productService.updateProduct(this.selectedProductId, formData)
      : this.productService.createProduct(formData);

    request.subscribe({
      next: (res: any) => {
        setTimeout(() => this.isSaving = false);
        if (res.status) {
          this.toastService.success('Success', `Product ${this.selectedProductId ? 'updated' : 'created'} successfully`);
          this.closeModal();
          this.fetchProducts();
        } else {
          this.toastService.error('Failed', res.error || 'Operation failed');
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        setTimeout(() => this.isSaving = false);
        const msg = err.error?.Error || err.error?.message || 'Operation failed';
        this.toastService.error('Error', msg);
        this.cdr.detectChanges();
      }
    });
  }

  onDelete(id: number): void {
    if (!confirm('Are you sure you want to delete this product?')) {
      return;
    }

    this.productService.deleteProduct(id).subscribe({
      next: (res: any) => {
        if (res.status) {
          this.toastService.success('Deleted', 'Product removed successfully');
          this.fetchProducts();
        } else {
          this.toastService.error('Error', res.error || 'Failed to delete product');
        }
      },
      error: (err: any) => {
        const msg = err.error?.Error || err.error?.message || 'Delete failed';
        this.toastService.error('Error', msg);
      }
    });
  }
}
