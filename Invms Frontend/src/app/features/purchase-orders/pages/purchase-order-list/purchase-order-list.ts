import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { PurchaseOrderService, PurchaseOrder } from '../../services/purchase-order.service';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { WarehouseService } from '../../../warehouses/services/warehouse.service';
import { ProductService } from '../../../products/services/product.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { PaginationParams } from '../../../../core/models/api.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-purchase-order-list',
  standalone: false,
  templateUrl: './purchase-order-list.html',
  styleUrl: './purchase-order-list.scss'
})
export class PurchaseOrderList implements OnInit, OnDestroy {
  orders: PurchaseOrder[] = [];
  suppliers: any[] = [];
  warehouses: any[] = [];
  products: any[] = [];
  isLoading = false;
  isSaving = false;
  isAdmin = false;

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
  orderForm: FormGroup;
  selectedOrderId: number | null = null;

  // Detail View State
  isDetailOpen = false;
  selectedOrderForDetail: PurchaseOrder | null = null;

  constructor(
    private poService: PurchaseOrderService,
    private supplierService: SupplierService,
    private warehouseService: WarehouseService,
    private productService: ProductService,
    private toastService: ToastService,
    private storageService: StorageService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.checkPermissions();
    this.orderForm = this.fb.group({
      supplierId: ['', [Validators.required]],
      warehouseId: ['', [Validators.required]],
      expectedDeliveryDate: [''],
      notes: [''],
      items: this.fb.array([], [Validators.required, Validators.minLength(1)])
    });
  }

  ngOnInit(): void {
    this.fetchOrders();
    this.loadDropdowns();
  }

  ngOnDestroy(): void {
    document.body.classList.remove('no-scroll');
  }

  checkPermissions(): void {
    this.canView = this.storageService.hasPermission('ViewPurchaseOrders');
    this.canManage = this.storageService.hasPermission('ManagePurchaseOrders');
    this.canDelete = this.storageService.hasPermission('DeletePurchaseOrders');
    this.isAdmin = this.storageService.isAdmin();
  }

  getFilteredProducts(): any[] {
    const supplierId = this.orderForm.get('supplierId')?.value;
    if (!supplierId) return [];
    return this.products.filter(p => p.supplierId == supplierId);
  }

  loadDropdowns(): void {
    this.supplierService.getAllSuppliers({ pageNumber: 1, pageSize: 100, sortColumn: 'Name', sortOrder: 'asc' }).subscribe(res => {
      if (res.status) this.suppliers = res.data?.items || [];
    });
    this.warehouseService.getAllWarehouses().subscribe(res => {
      if (res.status) this.warehouses = res.data || [];
    });
    this.productService.searchProducts().subscribe(res => {
      if (res.status) this.products = res.data || [];
    });

    // Clear items if supplier changes to prevent mismatch
    this.orderForm.get('supplierId')?.valueChanges.subscribe(() => {
      while (this.items.length !== 0) {
        this.items.removeAt(0);
      }
      this.addItem();
    });
  }

  fetchOrders(): void {
    if (!this.canView) return;
    this.isLoading = true;
    const params: PaginationParams = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortColumn: 'Id',
      sortOrder: 'desc'
    };

    this.poService.getAllOrders(params).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.status && res.data) {
          const paginated = res.data;
          this.orders = paginated.items || [];
          this.totalCount = paginated.totalCount;
          this.totalPages = paginated.totalPages;
          this.hasNextPage = paginated.hasNextPage;
          this.hasPreviousPage = paginated.hasPreviousPage;
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.isLoading = false;
        this.toastService.error('Error', 'Failed to fetch purchase orders');
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchOrders();
  }

  get items() {
    return this.orderForm.get('items') as FormArray;
  }

  addItem() {
    const itemGroup = this.fb.group({
      productId: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitCost: [0, [Validators.required, Validators.min(0.01)]]
    });
    this.items.push(itemGroup);
  }

  removeItem(index: number) {
    this.items.removeAt(index);
  }

  onProductSelect(index: number) {
    const productId = this.items.at(index).get('productId')?.value;
    const product = this.products.find(p => p.id == productId);
    if (product) {
      this.items.at(index).patchValue({
        unitCost: product.purchasePrice
      });
    }
  }

  openCreateModal() {
    this.selectedOrderId = null;
    this.orderForm.reset();
    while (this.items.length !== 0) {
      this.items.removeAt(0);
    }
    this.addItem();
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  openDetail(order: PurchaseOrder) {
    this.isLoading = true;
    this.poService.getOrderById(order.id).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.status && res.data) {
          this.selectedOrderForDetail = res.data;
          this.isDetailOpen = true;
          document.body.classList.add('no-scroll');
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.toastService.error('Error', 'Failed to fetch order details');
        this.cdr.detectChanges();
      }
    });
  }

  closeDetail() {
    this.isDetailOpen = false;
    this.selectedOrderForDetail = null;
    document.body.classList.remove('no-scroll');
  }

  getTotal(): number {
    return this.items.controls.reduce((acc, control) => {
      const quantity = control.get('quantity')?.value || 0;
      const unitCost = control.get('unitCost')?.value || 0;
      return acc + (quantity * unitCost);
    }, 0);
  }

  openEditModal(order: PurchaseOrder) {
    this.selectedOrderId = order.id;
    this.orderForm.patchValue({
      supplierId: order.supplierId,
      warehouseId: order.warehouseId,
      expectedDeliveryDate: order.expectedDeliveryDate ? order.expectedDeliveryDate.split('T')[0] : '',
      notes: order.notes
    });

    // Clear existing items
    while (this.items.length !== 0) {
      this.items.removeAt(0);
    }

    // Add items from order
    order.items.forEach(item => {
      const itemGroup = this.fb.group({
        productId: [item.productId, [Validators.required]],
        quantity: [item.quantity, [Validators.required, Validators.min(1)]],
        unitCost: [item.unitCost, [Validators.required, Validators.min(0.01)]]
      });
      this.items.push(itemGroup);
    });

    this.isModalOpen = true;
    this.cdr.detectChanges();
  }

  onSubmit() {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const formData = { ...this.orderForm.value };
    
    if (!formData.expectedDeliveryDate) {
      formData.expectedDeliveryDate = null;
    }

    const request = this.selectedOrderId 
      ? this.poService.updateOrder(this.selectedOrderId, formData)
      : this.poService.createOrder(formData);

    request.subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', `Purchase Order ${this.selectedOrderId ? 'updated' : 'created'} successfully`);
          this.closeModal();
          this.fetchOrders();
        }
      },
      error: (err) => {
        this.isSaving = false;
        const errMsg = err.error?.message || err.error?.title || 'Operation failed';
        this.toastService.error('Error', errMsg);
      }
    });
  }

  onApprove(id: number) {
    if (!confirm('Are you sure you want to approve this order?')) return;
    this.poService.approveOrder(id).subscribe(res => {
      if (res.status) {
        this.toastService.success('Approved', 'Order approved successfully');
        this.fetchOrders();
        if (this.isDetailOpen) this.closeDetail();
      }
    });
  }

  onReceive(id: number) {
    if (!confirm('Mark this order as received? This will update stock levels.')) return;
    this.poService.receiveOrder(id).subscribe(res => {
      if (res.status) {
        this.toastService.success('Received', 'Stock levels updated successfully');
        this.fetchOrders();
        if (this.isDetailOpen) this.closeDetail();
      }
    });
  }

  onCancel(id: number) {
    if (!confirm('Are you sure you want to cancel this order?')) return;
    this.poService.cancelOrder(id).subscribe(res => {
      if (res.status) {
        this.toastService.success('Cancelled', 'Order cancelled');
        this.fetchOrders();
        if (this.isDetailOpen) this.closeDetail();
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'draft': return 'bg-slate-100 text-slate-600';
      case 'pending': return 'bg-amber-100 text-amber-600';
      case 'approved': return 'bg-blue-100 text-blue-600';
      case 'received': return 'bg-emerald-100 text-emerald-600';
      case 'cancelled': return 'bg-red-100 text-red-600';
      default: return 'bg-slate-100 text-slate-600';
    }
  }
}
