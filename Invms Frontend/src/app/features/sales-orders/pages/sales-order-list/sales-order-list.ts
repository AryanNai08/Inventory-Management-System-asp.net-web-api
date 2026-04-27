import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { SalesOrderService, SalesOrder } from '../../services/sales-order.service';
import { CustomerService } from '../../../customers/services/customer.service';
import { WarehouseService } from '../../../warehouses/services/warehouse.service';
import { ProductService } from '../../../products/services/product.service';
import { ToastService } from '../../../../core/services/toast';
import { StorageService } from '../../../../core/services/storage.service';
import { PaginationParams } from '../../../../core/models/api.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-sales-order-list',
  standalone: false,
  templateUrl: './sales-order-list.html',
  styleUrl: './sales-order-list.scss'
})
export class SalesOrderListComponent implements OnInit, OnDestroy {
  orders: SalesOrder[] = [];
  customers: any[] = [];
  warehouses: any[] = [];
  products: any[] = [];
  filteredProducts: any[] = [];
  
  isLoading = false;
  isSaving = false;
  isModalOpen = false;
  selectedOrder: SalesOrder | null = null;
  selectedOrderId: number | null = null;
  
  // Pagination
  pagination: PaginationParams = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  };
  totalCount = 0;

  orderForm: FormGroup;

  // Permissions
  isAdmin = false;
  canManage = false;
  canDelete = false;

  constructor(
    private salesService: SalesOrderService,
    private customerService: CustomerService,
    private warehouseService: WarehouseService,
    private productService: ProductService,
    private fb: FormBuilder,
    private toastService: ToastService,
    private storageService: StorageService,
    private cdr: ChangeDetectorRef
  ) {
    this.orderForm = this.fb.group({
      customerId: ['', [Validators.required]],
      warehouseId: ['', [Validators.required]],
      notes: [''],
      items: this.fb.array([], [Validators.minLength(1)])
    });

    this.isAdmin = this.storageService.hasRole('Admin') || this.storageService.hasRole('SuperAdmin');
    this.canManage = this.isAdmin || this.storageService.hasPermission('ManageSalesOrders');
    this.canDelete = this.isAdmin || this.storageService.hasPermission('DeleteSalesOrders');
  }

  get pendingCount(): number {
    return this.orders.filter(o => 
      !['delivered', 'cancelled'].includes(o.statusName.toLowerCase())
    ).length;
  }

  goBack() {
    window.history.back();
  }

  warehouseStockMap: { [key: number]: number } = {};

  ngOnInit() {
    this.fetchOrders();
    this.loadDropdowns();

    // Listen for warehouse changes to fetch specific stock
    this.orderForm.get('warehouseId')?.valueChanges.subscribe(warehouseId => {
      if (warehouseId) {
        this.fetchWarehouseStock(Number(warehouseId));
      } else {
        this.warehouseStockMap = {};
      }
    });
  }

  fetchWarehouseStock(warehouseId: number) {
    this.warehouseService.getWarehouseStock(warehouseId).subscribe(res => {
      if (res.status) {
        // Map stock by ProductId for easy lookup
        this.warehouseStockMap = res.data.reduce((acc: any, item: any) => {
          acc[item.productId] = item.quantity;
          return acc;
        }, {});
        this.cdr.detectChanges();
      }
    });
  }

  ngOnDestroy() {
    // Cleanup
  }

  get items() {
    return this.orderForm.get('items') as FormArray;
  }

  addItem() {
    const itemGroup = this.fb.group({
      productId: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0.01)]]
    });

    // When product changes, auto-set price
    itemGroup.get('productId')?.valueChanges.subscribe(id => {
      const product = this.products.find(p => p.id === Number(id));
      if (product) {
        itemGroup.patchValue({ unitPrice: product.salePrice || 0 }, { emitEvent: false });
      }
    });

    this.items.push(itemGroup);
  }

  removeItem(index: number) {
    this.items.removeAt(index);
  }

  loadDropdowns() {
    this.customerService.getAllCustomers({ pageNumber: 1, pageSize: 100 }).subscribe(res => {
      if (res.status) this.customers = res.data.items;
    });

    this.warehouseService.getAllWarehouses().subscribe(res => {
      if (res.status) this.warehouses = res.data;
    });

    this.productService.getAllProducts({ pageNumber: 1, pageSize: 1000 }).subscribe(res => {
      if (res.status) {
        this.products = res.data.items;
        this.filteredProducts = [...this.products];
      }
    });
  }

  fetchOrders() {
    this.isLoading = true;
    this.salesService.getAllOrders(this.pagination).subscribe({
      next: (res) => {
        if (res.status) {
          this.orders = res.data.items;
          this.totalCount = res.data.totalCount;
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.toastService.error('Error', 'Failed to load sales orders');
      }
    });
  }

  openCreateModal() {
    this.selectedOrderId = null;
    this.orderForm.reset({
      customerId: '',
      warehouseId: '',
      notes: ''
    });
    while (this.items.length !== 0) this.items.removeAt(0);
    this.addItem();
    this.isModalOpen = true;

    // Trigger stock fetch if warehouse is already set (e.g. from previous session)
    const wid = this.orderForm.get('warehouseId')?.value;
    if (wid) this.fetchWarehouseStock(Number(wid));
  }

  openEditModal(order: SalesOrder) {
    this.closeDetail(); // Close sidebar first
    this.selectedOrderId = order.id;
    this.orderForm.patchValue({
      customerId: order.customerId,
      warehouseId: order.warehouseId,
      notes: order.notes
    });

    while (this.items.length !== 0) this.items.removeAt(0);

    order.items.forEach(item => {
      const itemGroup = this.fb.group({
        productId: [item.productId, [Validators.required]],
        quantity: [item.quantity, [Validators.required, Validators.min(1)]],
        unitPrice: [item.unitPrice, [Validators.required, Validators.min(0.01)]]
      });
      this.items.push(itemGroup);
    });

    this.isModalOpen = true;
    
    // Fetch stock for the warehouse in the order
    if (order.warehouseId) this.fetchWarehouseStock(order.warehouseId);
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedOrderId = null;
  }

  openDetail(order: SalesOrder) {
    this.isLoading = true;
    this.salesService.getOrderById(order.id).subscribe({
      next: (res) => {
        if (res.status) {
          this.selectedOrder = res.data;
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.toastService.error('Error', 'Failed to load order details');
      }
    });
  }

  closeDetail() {
    this.selectedOrder = null;
  }

  onSubmit() {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    const formRaw = this.orderForm.value;
    
    // 1. Construct Request DTO with exact Backend casing
    const requestDto = {
      CustomerId: Number(formRaw.customerId),
      WarehouseId: Number(formRaw.warehouseId),
      Notes: formRaw.notes || '',
      Items: formRaw.items.map((item: any) => ({
        ProductId: Number(item.productId),
        Quantity: Number(item.quantity),
        UnitPrice: Number(item.unitPrice)
      }))
    };
    
    // 2. Client-side Stock Validation (WAREHOUSE SPECIFIC)
    for (const item of requestDto.Items) {
      const available = this.warehouseStockMap[item.ProductId] || 0;
      const product = this.products.find(p => p.id == item.ProductId);
      
      if (item.Quantity > available) {
        this.toastService.warning('Stock Alert', `Insufficient stock for ${product?.name || 'Product'}. Available in this warehouse: ${available}`);
        this.isSaving = false;
        return; 
      }
    }

    this.isSaving = true;
    
    const request = this.selectedOrderId 
      ? this.salesService.updateOrder(this.selectedOrderId, requestDto)
      : this.salesService.createOrder(requestDto);

    request.subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.status) {
          this.toastService.success('Success', `Sales Order ${this.selectedOrderId ? 'updated' : 'created'} successfully`);
          if (res.data?.warnings?.length > 0) {
            res.data.warnings.forEach(w => this.toastService.warning('Order Warning', w));
          }
          this.closeModal();
          this.fetchOrders();
        } else {
          this.toastService.error('Error', res.message || 'Operation failed');
        }
      },
      error: (err) => {
        this.isSaving = false;
        console.error('Submission error:', err);
        
        // Improved error parsing
        let errMsg = 'Operation failed. Please check your inputs.';
        if (err.error) {
          if (typeof err.error === 'string') errMsg = err.error;
          else if (err.error.message) errMsg = err.error.message;
          else if (err.error.Message) errMsg = err.error.Message;
          else if (err.error.errors) {
            // Handle ASP.NET Validation errors object
            const errors = err.error.errors;
            const firstKey = Object.keys(errors)[0];
            errMsg = errors[firstKey][0];
          }
        }
        
        this.toastService.error('Error', errMsg);
      }
    });
  }

  onConfirm(id: number) {
    this.salesService.confirmOrder(id).subscribe({
      next: () => {
        this.toastService.success('Confirmed', 'Stock has been deducted and order is confirmed');
        this.fetchOrders();
        this.closeDetail();
      },
      error: (err) => this.toastService.error('Confirmation Failed', err.error?.message || 'Error')
    });
  }

  onShip(id: number) {
    this.salesService.shipOrder(id).subscribe({
      next: () => {
        this.toastService.success('Shipped', 'Order status updated to Shipped');
        this.fetchOrders();
        this.closeDetail();
      },
      error: (err) => this.toastService.error('Shipping Failed', err.error?.message || 'Error')
    });
  }

  onDeliver(id: number) {
    this.salesService.deliverOrder(id).subscribe({
      next: () => {
        this.toastService.success('Delivered', 'Order marked as Delivered');
        this.fetchOrders();
        this.closeDetail();
      },
      error: (err) => this.toastService.error('Delivery Failed', err.error?.message || 'Error')
    });
  }

  onCancel(id: number) {
    if (confirm('Are you sure you want to cancel this order? Stock will be restored if previously deducted.')) {
      this.salesService.cancelOrder(id).subscribe({
        next: () => {
          this.toastService.success('Cancelled', 'Order cancelled successfully');
          this.fetchOrders();
          this.closeDetail();
        },
        error: (err) => this.toastService.error('Cancel Failed', err.error?.message || 'Error')
      });
    }
  }

  calculateTotal() {
    return this.items.controls.reduce((acc, ctrl) => {
      const q = ctrl.get('quantity')?.value || 0;
      const p = ctrl.get('unitPrice')?.value || 0;
      return acc + (q * p);
    }, 0);
  }

  getStatusClass(status: string) {
    const s = status.toLowerCase();
    if (s === 'pending') return 'bg-amber-100 text-amber-700 border-amber-200';
    if (s === 'confirmed') return 'bg-blue-100 text-blue-700 border-blue-200';
    if (s === 'shipped') return 'bg-indigo-100 text-indigo-700 border-indigo-200';
    if (s === 'delivered') return 'bg-emerald-100 text-emerald-700 border-emerald-200';
    if (s === 'cancelled') return 'bg-rose-100 text-rose-700 border-rose-200';
    return 'bg-slate-100 text-slate-700 border-slate-200';
  }

  onPageChange(page: number) {
    this.pagination.pageNumber = page;
    this.fetchOrders();
  }
}
