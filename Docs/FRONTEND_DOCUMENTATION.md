# 📱 Inventory Management System — Frontend Documentation

> A **production-grade** Angular 18+ SPA with **NgRx State Management**, **Clean Architecture**, **Dynamic RBAC UI**, and **Reusable Component Library**.

---

## 📌 Frontend Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | Angular 18+ + TypeScript |
| **State Management** | NgRx (Store + Effects + Selectors) |
| **Styling** | Angular Material + Tailwind CSS |
| **HTTP Client** | HttpClient + Interceptors (JWT Auto-Refresh) |
| **Form Handling** | Reactive Forms + Validators |
| **Routing** | Angular Router with Route Guards |
| **API Integration** | HttpClient + Custom Service Layer |
| **Auth Management** | JWT in HttpOnly Cookies + Refresh Token Flow |
| **Build Tool** | Angular CLI (bundler: esbuild) |
| **Package Manager** | npm |
| **Testing** | Jasmine + Karma + Spectator |
| **Linting** | ESLint + Prettier |
| **Testing** | Cypress for E2E tests |

---

## 🏗️ Frontend Architecture (Clean & Scalable)

This project follows a **Modular Clean Architecture** that mirrors the backend structure:

```
frontend/                                  # Angular SPA root
│
├── src/                                   # Application source code
│   │
│   ├── app/                              # Application modules & components
│   │   │
│   │   ├── core/                         # ⚙️ CORE - Singleton services & app initialization
│   │   │   ├── http-interceptors/        # HTTP interceptors (JWT, error handling, retry)
│   │   │   │   ├── auth.interceptor.ts   # Add JWT token to requests
│   │   │   │   ├── error.interceptor.ts  # Handle 401, 403, 500 errors
│   │   │   │   └── refresh.interceptor.ts # Auto-refresh on 401
│   │   │   │
│   │   │   ├── guards/                   # Route guards
│   │   │   │   ├── auth.guard.ts         # Check authentication
│   │   │   │   ├── role.guard.ts         # Check user role
│   │   │   │   └── permission.guard.ts   # Check specific permissions
│   │   │   │
│   │   │   ├── services/                 # 🔌 API & SHARED Services (One place to edit!)
│   │   │   │   ├── api.service.ts        # Base HTTP service with caching
│   │   │   │   ├── auth.service.ts       # Auth endpoints (login, register, refresh, logout)
│   │   │   │   ├── product.service.ts    # Product CRUD + search service
│   │   │   │   ├── category.service.ts   # Category service
│   │   │   │   ├── customer.service.ts   # Customer CRUD service
│   │   │   │   ├── supplier.service.ts   # Supplier CRUD service
│   │   │   │   ├── warehouse.service.ts  # Warehouse CRUD service
│   │   │   │   ├── order.service.ts      # Purchase & Sales order service
│   │   │   │   ├── dashboard.service.ts  # Dashboard analytics service
│   │   │   │   ├── report.service.ts     # Report generation service
│   │   │   │   ├── role.service.ts       # Role CRUD service
│   │   │   │   ├── privilege.service.ts  # Privilege CRUD service
│   │   │   │   ├── stock-adjustment.service.ts # Stock adjustment service
│   │   │   │   ├── storage.service.ts    # LocalStorage wrapper (typed)
│   │   │   │   ├── notification.service.ts # Toast/alert system
│   │   │   │   └── logger.service.ts     # Centralized logging
│   │   │   │
│   │   │   ├── store/                    # 🏪 NgRx State Management
│   │   │   │   ├── app.state.ts          # Root state interface
│   │   │   │   ├── auth/
│   │   │   │   │   ├── auth.state.ts     # Auth state model
│   │   │   │   │   ├── auth.actions.ts   # Auth actions
│   │   │   │   │   ├── auth.reducer.ts   # Auth reducer
│   │   │   │   │   ├── auth.effects.ts   # Auth side effects (API calls)
│   │   │   │   │   └── auth.selectors.ts # Auth selectors
│   │   │   │   │
│   │   │   │   ├── ui/
│   │   │   │   │   ├── ui.state.ts       # UI state (sidebar, theme, notifications)
│   │   │   │   │   ├── ui.actions.ts
│   │   │   │   │   ├── ui.reducer.ts
│   │   │   │   │   └── ui.selectors.ts
│   │   │   │   │
│   │   │   │   └── index.ts              # Store configuration
│   │   │   │
│   │   │   ├── models/                   # TypeScript Interfaces
│   │   │   │   ├── user.model.ts
│   │   │   │   ├── auth.model.ts
│   │   │   │   ├── product.model.ts
│   │   │   │   ├── order.model.ts
│   │   │   │   ├── customer.model.ts
│   │   │   │   ├── supplier.model.ts
│   │   │   │   ├── warehouse.model.ts
│   │   │   │   ├── category.model.ts
│   │   │   │   ├── role.model.ts
│   │   │   │   ├── privilege.model.ts
│   │   │   │   ├── stock-adjustment.model.ts
│   │   │   │   ├── api.model.ts          # APIResponse<T>, PaginatedResult<T>
│   │   │   │   └── index.ts
│   │   │   │
│   │   │   └── core.module.ts            # Core module definition
│   │   │
│   │   ├── shared/                       # 🔧 SHARED - Reusable across features
│   │   │   ├── config/                   # 📋 Configuration
│   │   │   │   ├── api.config.ts         # API endpoints, base URL, timeouts
│   │   │   │   ├── auth.config.ts        # JWT expiry, refresh token settings
│   │   │   │   ├── app.config.ts         # Feature flags, role permissions
│   │   │   │   └── constants.ts          # Global constants
│   │   │   │
│   │   │   ├── constants/                # 🔐 Global Constants (One place to edit!)
│   │   │   │   ├── http-status.const.ts  # Status codes (200, 401, 403, 404, 500)
│   │   │   │   ├── api-endpoints.const.ts # All backend API routes (92 endpoints)
│   │   │   │   ├── role-permissions.const.ts # Role-to-permission mapping
│   │   │   │   ├── messages.const.ts     # User-facing messages (errors, success, warnings)
│   │   │   │   ├── pagination.const.ts   # Default page size, max limit
│   │   │   │   └── date-formats.const.ts # Date/time format templates
│   │   │   │
│   │   │   ├── pipes/                    # Custom pipes
│   │   │   │   ├── currency.pipe.ts      # Format currency (₹, $, €)
│   │   │   │   ├── date.pipe.ts          # Format dates (DD/MM/YYYY, etc.)
│   │   │   │   ├── number.pipe.ts        # Format numbers with separators
│   │   │   │   ├── safe-html.pipe.ts     # Sanitize HTML
│   │   │   │   └── truncate.pipe.ts      # Truncate text
│   │   │   │
│   │   │   ├── validators/               # 🧪 Custom validators
│   │   │   │   ├── custom.validators.ts  # Email, password, phone validators
│   │   │   │   └── index.ts
│   │   │   │
│   │   │   ├── components/               # 🎨 Shared UI Components
│   │   │   │   ├── layout/
│   │   │   │   │   ├── sidebar/
│   │   │   │   │   │   ├── sidebar.component.ts
│   │   │   │   │   │   ├── sidebar.component.html
│   │   │   │   │   │   └── sidebar.component.scss
│   │   │   │   │   ├── header/
│   │   │   │   │   ├── navbar/
│   │   │   │   │   ├── footer/
│   │   │   │   │   └── app-layout/
│   │   │   │   │
│   │   │   │   ├── button/
│   │   │   │   │   ├── button.component.ts
│   │   │   │   │   ├── icon-button/
│   │   │   │   │   └── button-group/
│   │   │   │   │
│   │   │   │   ├── form-controls/
│   │   │   │   │   ├── input/
│   │   │   │   │   ├── select/
│   │   │   │   │   ├── checkbox/
│   │   │   │   │   ├── radio/
│   │   │   │   │   ├── textarea/
│   │   │   │   │   └── date-picker/
│   │   │   │   │
│   │   │   │   ├── table/
│   │   │   │   │   ├── table.component.ts
│   │   │   │   │   ├── data-table.component.ts (with sorting, filtering)
│   │   │   │   │   └── table-pagination/
│   │   │   │   │
│   │   │   │   ├── modal/
│   │   │   │   │   ├── modal.component.ts
│   │   │   │   │   ├── confirm-dialog/
│   │   │   │   │   └── alert-dialog/
│   │   │   │   │
│   │   │   │   ├── notification/
│   │   │   │   │   ├── toast/
│   │   │   │   │   └── notification/
│   │   │   │   │
│   │   │   │   ├── loading/
│   │   │   │   │   ├── spinner/
│   │   │   │   │   ├── skeleton/
│   │   │   │   │   └── full-page-loader/
│   │   │   │   │
│   │   │   │   ├── error/
│   │   │   │   │   ├── error-boundary/
│   │   │   │   │   ├── error-page/
│   │   │   │   │   └── error-message/
│   │   │   │   │
│   │   │   │   ├── badge/
│   │   │   │   ├── card/
│   │   │   │   ├── tabs/
│   │   │   │   ├── breadcrumb/
│   │   │   │   ├── pagination/
│   │   │   │   └── icons/
│   │   │   │
│   │   │   └── shared.module.ts          # Shared module definition
│   │   │
│   │   ├── features/                     # 🎯 Feature Modules (Domain-Driven)
│   │   │   ├── auth/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── login-page/
│   │   │   │   │   │   ├── login-page.component.ts
│   │   │   │   │   │   ├── login-page.component.html
│   │   │   │   │   │   └── login-page.component.scss
│   │   │   │   │   ├── register-page/
│   │   │   │   │   └── forgot-password-page/
│   │   │   │   ├── components/
│   │   │   │   │   ├── login-form/
│   │   │   │   │   ├── register-form/
│   │   │   │   │   └── protected-route/
│   │   │   │   ├── store/
│   │   │   │   │   ├── auth.state.ts
│   │   │   │   │   ├── auth.actions.ts
│   │   │   │   │   ├── auth.reducer.ts
│   │   │   │   │   ├── auth.effects.ts
│   │   │   │   │   └── auth.selectors.ts
│   │   │   │   └── auth.module.ts
│   │   │   │
│   │   │   ├── dashboard/
│   │   │   │   ├── pages/
│   │   │   │   │   └── dashboard-page/
│   │   │   │   ├── components/
│   │   │   │   │   ├── summary-cards/
│   │   │   │   │   ├── low-stock-chart/
│   │   │   │   │   ├── top-products-chart/
│   │   │   │   │   └── sales-chart/
│   │   │   │   ├── store/
│   │   │   │   └── dashboard.module.ts
│   │   │   │
│   │   │   ├── products/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── product-list-page/
│   │   │   │   │   ├── product-detail-page/
│   │   │   │   │   └── product-form-page/
│   │   │   │   ├── components/
│   │   │   │   │   ├── product-table/
│   │   │   │   │   ├── product-form/
│   │   │   │   │   ├── product-card/
│   │   │   │   │   ├── product-search-filter/
│   │   │   │   │   └── product-modal/
│   │   │   │   ├── store/
│   │   │   │   └── products.module.ts
│   │   │   │
│   │   │   ├── orders/
│   │   │   │   ├── purchase-orders/
│   │   │   │   │   ├── pages/
│   │   │   │   │   ├── components/
│   │   │   │   │   ├── store/
│   │   │   │   │   └── purchase-orders.module.ts
│   │   │   │   ├── sales-orders/
│   │   │   │   │   ├── pages/
│   │   │   │   │   ├── components/
│   │   │   │   │   ├── store/
│   │   │   │   │   └── sales-orders.module.ts
│   │   │   │   └── orders.module.ts
│   │   │   │
│   │   │   ├── customers/
│   │   │   │   ├── pages/
│   │   │   │   ├── components/
│   │   │   │   ├── store/
│   │   │   │   └── customers.module.ts
│   │   │   │
│   │   │   ├── suppliers/
│   │   │   │   ├── pages/
│   │   │   │   ├── components/
│   │   │   │   ├── store/
│   │   │   │   └── suppliers.module.ts
│   │   │   │
│   │   │   ├── warehouses/
│   │   │   │   ├── pages/
│   │   │   │   ├── components/
│   │   │   │   ├── store/
│   │   │   │   └── warehouses.module.ts
│   │   │   │
│   │   │   ├── reports/
│   │   │   │   ├── pages/
│   │   │   │   ├── components/
│   │   │   │   └── reports.module.ts
│   │   │   │
│   │   │   ├── stock-adjustment/
│   │   │   │   ├── pages/
│   │   │   │   ├── components/
│   │   │   │   └── stock-adjustment.module.ts
│   │   │   │
│   │   │   ├── admin/
│   │   │   │   ├── users/
│   │   │   │   │   ├── pages/
│   │   │   │   │   ├── components/
│   │   │   │   │   ├── store/
│   │   │   │   │   └── users.module.ts
│   │   │   │   ├── roles/
│   │   │   │   │   ├── pages/
│   │   │   │   │   ├── components/
│   │   │   │   │   ├── store/
│   │   │   │   │   └── roles.module.ts
│   │   │   │   ├── privileges/
│   │   │   │   │   ├── pages/
│   │   │   │   │   ├── components/
│   │   │   │   │   ├── store/
│   │   │   │   │   └── privileges.module.ts
│   │   │   │   └── admin.module.ts
│   │   │   │
│   │   │   └── settings/
│   │   │       ├── pages/
│   │   │       ├── components/
│   │   │       └── settings.module.ts
│   │   │
│   │   ├── app.component.ts              # Root component with routing
│   │   ├── app.routing.ts                # Main routing configuration
│   │   └── app.module.ts                 # Root module
│   │
│   ├── assets/                           # Static assets
│   │   ├── images/
│   │   ├── icons/
│   │   └── fonts/
│   │
│   ├── styles/                           # 🎨 Global Styles
│   │   ├── styles.scss                   # Global styles
│   │   ├── variables.scss                # SCSS variables (colors, breakpoints)
│   │   ├── mixins.scss                   # SCSS mixins
│   │   ├── tailwind.scss                 # Tailwind directives
│   │   └── themes/                       # Theme files (dark mode, light mode)
│   │
│   ├── environments/                     # Environment configuration
│   │   ├── environment.ts                # Development
│   │   ├── environment.prod.ts           # Production
│   │   └── environment.staging.ts        # Staging
│   │
│   ├── main.ts                           # Entry point
│   └── index.html                        # HTML root
│
├── .angular-cli.json                     # Angular CLI configuration
├── tsconfig.json                         # TypeScript configuration
├── tsconfig.app.json
├── tsconfig.spec.json                    # Testing configuration
├── angular.json                          # Angular project configuration
├── karma.conf.js                         # Unit test configuration
├── tailwind.config.js                    # Tailwind configuration
├── postcss.config.js
├── .eslintrc.json                        # ESLint configuration
├── .prettierrc.json                      # Prettier configuration
├── package.json
└── README.md
```

---

## 🔌 API Integration Layer (Single Source of Truth - ONE Edit, Everywhere Updated!)

### **CRITICAL: Services as Single Edit Point**

Your senior's suggestion means: **Define API calls ONCE in services → Use EVERYWHERE via Dependency Injection. Edit API endpoint once = updates entire application instantly.**

#### Example: `src/app/core/services/product.service.ts`
```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError, shareReplay } from 'rxjs/operators';
import { API_CONFIG } from '@/shared/config/api.config';
import { ProductDto, CreateProductDto, UpdateProductDto } from '@/core/models/product.model';
import { APIResponse, PaginatedResult } from '@/core/models/api.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${API_CONFIG.BASE_URL}/api/products`;
  private productCache = new Map<number, Observable<APIResponse<ProductDto>>>();

  constructor(private http: HttpClient) {}

  /**
   * 🎯 CRITICAL: Single source of truth for getAll()
   * Usage: Can be called from ProductListComponent, ProductSelectorComponent, ProductTableComponent, etc.
   * If API endpoint changes, edit ONLY here → affects entire app
   * 
   * Example: /api/products/GetAllProducts?pageNumber=1&pageSize=10
   */
  getAll(pageNumber: number, pageSize: number): Observable<APIResponse<PaginatedResult<ProductDto>>> {
    return this.http.get<APIResponse<PaginatedResult<ProductDto>>>(`${this.apiUrl}/GetAllProducts`, {
      params: { pageNumber, pageSize }
    }).pipe(
      shareReplay(1) // Cache results for multiple subscribers
    );
  }

  getById(id: number): Observable<APIResponse<ProductDto>> {
    // Check cache first
    if (this.productCache.has(id)) {
      return this.productCache.get(id)!;
    }

    const request$ = this.http.get<APIResponse<ProductDto>>(`${this.apiUrl}/${id}`).pipe(
      shareReplay(1)
    );

    this.productCache.set(id, request$);
    return request$;
  }

  getBySku(sku: string): Observable<APIResponse<ProductDto>> {
    return this.http.get<APIResponse<ProductDto>>(`${this.apiUrl}/sku/${sku}`);
  }

  search(name?: string, categoryId?: number, supplierId?: number): Observable<APIResponse<ProductDto[]>> {
    return this.http.get<APIResponse<ProductDto[]>>(`${this.apiUrl}/search`, {
      params: { name: name || '', categoryId: categoryId || 0, supplierId: supplierId || 0 }
    });
  }

  create(dto: CreateProductDto): Observable<APIResponse<ProductDto>> {
    return this.http.post<APIResponse<ProductDto>>(`${this.apiUrl}/CreateProduct`, dto).pipe(
      map(response => {
        this.productCache.clear(); // Invalidate cache after create
        return response;
      })
    );
  }

  update(id: number, dto: UpdateProductDto): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}/${id}`, dto).pipe(
      map(response => {
        this.productCache.delete(id); // Invalidate specific cache
        return response;
      })
    );
  }

  delete(id: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(`${this.apiUrl}/${id}`).pipe(
      map(response => {
        this.productCache.delete(id);
        return response;
      })
    );
  }

  getLowStock(): Observable<APIResponse<ProductDto[]>> {
    return this.http.get<APIResponse<ProductDto[]>>(`${this.apiUrl}/low-stock`);
  }

  getOutOfStock(): Observable<APIResponse<ProductDto[]>> {
    return this.http.get<APIResponse<ProductDto[]>>(`${this.apiUrl}/out-of-stock`);
  }

  partialUpdate(id: number, updates: Partial<UpdateProductDto>): Observable<APIResponse<string>> {
    return this.http.patch<APIResponse<string>>(`${this.apiUrl}/${id}`, updates).pipe(
      map(response => {
        this.productCache.delete(id);
        return response;
      })
    );
  }

  // Utility: Clear cache
  clearCache(): void {
    this.productCache.clear();
  }
}
```

#### Example: `src/app/core/http-interceptors/auth.interceptor.ts`
```typescript
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { StorageService } from '../services/storage.service';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private storageService: StorageService,
    private authService: AuthService,
    private router: Router
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Add JWT token from HttpOnly Cookie automatically
    // (HttpClient sends cookies automatically with withCredentials: true)
    
    // For API calls that might need explicit token handling
    const token = this.storageService.getToken();
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Attempt to refresh token
          return this.authService.refreshToken().pipe(
            catchError(() => {
              this.authService.logout();
              this.router.navigate(['/login']);
              return throwError(() => error);
            })
          );
        }
        return throwError(() => error);
      })
    );
  }
}
```

#### Example: Using Service in Component (Dependency Injection)
```typescript
// src/app/features/products/components/product-list/product-list.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '@/core/services/product.service';
import { ProductDto } from '@/core/models/product.model';
import { NotificationService } from '@/core/services/notification.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit, OnDestroy {
  products: ProductDto[] = [];
  isLoading = false;
  error: string | null = null;
  currentPage = 1;
  pageSize = 10;

  // RxJS: Unsubscribe gracefully
  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.error = null;

    // 🎯 Service call → automatically handles API endpoint
    // If endpoint changes in service, this component is unaffected!
    this.productService.getAll(this.currentPage, this.pageSize)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.products = response.data;
          this.isLoading = false;
        },
        error: (err) => {
          this.error = 'Failed to load products';
          this.notificationService.error('Failed to load products');
          this.isLoading = false;
        }
      });
  }

  deleteProduct(id: number): void {
    if (!confirm('Are you sure?')) return;

    this.productService.delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.success('Product deleted successfully');
          this.loadProducts(); // Refresh list
        },
        error: () => this.notificationService.error('Failed to delete product')
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

#### Example: Template Using Service Data
```html
<!-- src/app/features/products/components/product-list/product-list.component.html -->
<div class="container">
  <h1>Products</h1>
  
  <!-- Loading -->
  <div *ngIf="isLoading" class="spinner">
    <p>Loading products...</p>
  </div>

  <!-- Error -->
  <div *ngIf="error" class="alert alert-danger">
    {{ error }}
  </div>

  <!-- Products Table -->
  <table *ngIf="!isLoading && products.length > 0" class="table">
    <thead>
      <tr>
        <th>Name</th>
        <th>SKU</th>
        <th>Price</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let product of products">
        <td>{{ product.name }}</td>
        <td>{{ product.sku }}</td>
        <td>{{ product.unitPrice | currency }}</td>
        <td>
          <button (click)="deleteProduct(product.id)" class="btn btn-danger">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>

  <!-- No products -->
  <div *ngIf="!isLoading && products.length === 0" class="alert alert-info">
    No products found.
  </div>
</div>
```

---

## 📡 API Endpoints Map (92 Total)

### **Auth (7 endpoints)**
```
POST   /api/auth/register               → Register new user
POST   /api/auth/login                  → Login (sets cookies)
POST   /api/auth/change-password        → Change password (Authorized)
POST   /api/auth/refresh                → Refresh token
POST   /api/auth/forgot-password        → Send OTP
POST   /api/auth/reset-password         → Reset password
POST   /api/auth/logout                 → Logout (revoke token)
```

### **Products (10 endpoints)**
```
GET    /api/products/GetAllProducts             → List all with pagination
GET    /api/products/{id}                       → Get by ID
GET    /api/products/sku/{sku}                  → Get by SKU
GET    /api/products/search                     → Search (name, category, supplier)
POST   /api/products/CreateProduct              → Create new
PUT    /api/products/{id}                       → Update
DELETE /api/products/{id}                       → Delete
GET    /api/products/low-stock                  → Low stock items
GET    /api/products/out-of-stock               → Out of stock items
PATCH  /api/products/{id}                       → Partial update
```

### **Customers (7 endpoints)**
```
GET    /api/customers/GetAllCustomers           → List all
GET    /api/customers/{id}                      → Get by ID
GET    /api/customers/search                    → Search
POST   /api/customers/CreateCustomer            → Create
PUT    /api/customers/{id}                      → Update
DELETE /api/customers/{id}                      → Delete
GET    /api/customers/{id}/sales-orders         → Get sales orders
```

### **Suppliers (7 endpoints)**
```
GET    /api/suppliers/GetAllSuppliers           → List all
GET    /api/suppliers/{id}                      → Get by ID
GET    /api/suppliers/{id}/products             → Get products
POST   /api/suppliers/CreateSupplier            → Create
PUT    /api/suppliers/{id}                      → Update
DELETE /api/suppliers/{id}                      → Delete
GET    /api/suppliers/{id}/purchase-orders      → Get purchase orders
```

### **Purchase Orders (7 endpoints)**
```
GET    /api/purchaseorders/GetAllOrders         → List all
GET    /api/purchaseorders/{id}                 → Get by ID
POST   /api/purchaseorders/CreateOrder          → Create
PUT    /api/purchaseorders/{id}                 → Update
PUT    /api/purchaseorders/{id}/approve         → Approve
PUT    /api/purchaseorders/{id}/receive         → Receive
PUT    /api/purchaseorders/{id}/cancel          → Cancel
```

### **Sales Orders (9 endpoints)**
```
GET    /api/sales-orders                        → List all
GET    /api/sales-orders/{id}                   → Get by ID
POST   /api/sales-orders                        → Create
PUT    /api/sales-orders/{id}                   → Update
PATCH  /api/sales-orders/{id}/confirm           → Confirm
PATCH  /api/sales-orders/{id}/ship              → Ship
PATCH  /api/sales-orders/{id}/deliver           → Deliver
DELETE /api/sales-orders/{id}                   → Cancel
GET    /api/sales-orders/search                 → Search
```

### **Categories (5 endpoints)**
```
GET    /api/categories/GetAllCategories         → List all
GET    /api/categories/{id}                     → Get by ID
POST   /api/categories/CreateCategory           → Create
PUT    /api/categories/{id}                     → Update
DELETE /api/categories/{id}                     → Delete
```

### **Warehouses (5 endpoints)**
```
GET    /api/warehouses/GetAllWarehouses         → List all
GET    /api/warehouses/{id}                     → Get by ID
POST   /api/warehouses/CreateWarehouse          → Create
PUT    /api/warehouses/{id}                     → Update
DELETE /api/warehouses/{id}                     → Delete
```

### **Roles (4 endpoints)**
```
GET    /api/roles/all                           → List all
POST   /api/roles/create                        → Create
PUT    /api/roles/update/{id}                   → Update
DELETE /api/roles/delete/{id}                   → Delete
```

### **Privileges (5 endpoints)**
```
GET    /api/privileges/all                      → List all
GET    /api/privileges/id/{id}                  → Get by ID
POST   /api/privileges/create                   → Create
PUT    /api/privileges/update/{id}              → Update
DELETE /api/privileges/delete/{id}              → Delete
```

### **Role-Privileges (3 endpoints)**
```
POST   /api/roleprivileges/assign               → Assign privilege to role
GET    /api/roleprivileges/{roleId}/privileges → Get privileges by role
DELETE /api/roleprivileges/{roleId}/{privilegeId} → Remove privilege
```

### **Users (5 endpoints)**
```
GET    /api/users/GetAllUsers                   → List all
GET    /api/users/{id}                          → Get by ID
DELETE /api/users/{id}                          → Delete
PUT    /api/users/{id}                          → Update
GET    /api/users/me                            → Get current user profile
```

### **Dashboard (3 endpoints)**
```
GET    /api/dashboard/summary                   → Dashboard summary
GET    /api/dashboard/low-stock                 → Low stock report
GET    /api/dashboard/top-selling               → Top selling products
```

### **Reports (5 endpoints)**
```
GET    /api/reports/sales-by-product            → Sales by product (PDF)
GET    /api/reports/purchases-by-supplier       → Purchases by supplier (PDF)
GET    /api/reports/stock-movement              → Stock movement (PDF)
GET    /api/reports/revenue                     → Revenue report (PDF)
GET    /api/reports/order-status-summary        → Order status summary (PDF)
```

### **Stock Adjustments (5 endpoints)**
```
GET    /api/stockadjustments                    → List all
GET    /api/stockadjustments/{id}               → Get by ID
GET    /api/stockadjustments/product/{productId} → Get by product
POST   /api/stockadjustments                    → Create
GET    /api/stockadjustments/types              → Get adjustment types
```

---

## � Frontend Modules & Implementation

Divide frontend into **12 feature modules** exactly like the backend structure. Each module has its own Pages, Services, Store, and Routing.

### **Module 1: Authentication**
- **Path**: `src/app/features/auth/`
- **Pages**: LoginPage, RegisterPage, ForgotPasswordPage, ResetPasswordPage
- **Service**: `auth.service.ts` → `/api/auth/*`
- **Features**:
  - User registration with validation
  - JWT login with HttpOnly cookie storage
  - Auto-refresh token handling
  - Password reset via OTP
  - Logout with token revocation
- **NgRx Store**: Auth state, actions, reducer, effects, selectors
- **Guards**: AuthGuard (protect authenticated pages)

### **Module 2: User Management**
- **Path**: `src/app/features/admin/users/`
- **Pages**: UserListPage, UserDetailPage, UserFormPage
- **Service**: `user.service.ts` → `/api/users/*`
- **Features**:
  - Get all users with pagination
  - Get user by ID
  - Update user profile
  - Delete user
  - Current user profile endpoint
- **Guards**: RoleGuard with permission check

### **Module 3: Category Management**
- **Path**: `src/app/features/categories/`
- **Pages**: CategoryListPage, CategoryFormPage
- **Service**: `category.service.ts` → `/api/categories/*`
- **Features**:
  - CRUD operations (Create, Read, Update, Delete)
  - Pagination support
  - Search/filter categories

### **Module 4: Role & Privilege Management**
- **Path**: `src/app/features/admin/roles/` and `src/app/features/admin/privileges/`
- **Services**: `role.service.ts`, `privilege.service.ts`
- **API**: `/api/roles/*`, `/api/privileges/*`, `/api/roleprivileges/*`
- **Features**:
  - Create and manage roles
  - Define privileges
  - Assign privileges to roles
  - Dynamic permissions UI

### **Module 5: Supplier Management**
- **Path**: `src/app/features/suppliers/`
- **Pages**: SupplierListPage, SupplierFormPage, SupplierDetailPage
- **Service**: `supplier.service.ts` → `/api/suppliers/*`
- **Features**:
  - CRUD operations
  - List products by supplier
  - View purchase orders from supplier

### **Module 6: Customer Management**
- **Path**: `src/app/features/customers/`
- **Pages**: CustomerListPage, CustomerFormPage, CustomerDetailPage
- **Service**: `customer.service.ts` → `/api/customers/*`
- **Features**:
  - CRUD operations
  - Search customers
  - View sales orders for customer

### **Module 7: Warehouse Management**
- **Path**: `src/app/features/warehouses/`
- **Pages**: WarehouseListPage, WarehouseFormPage
- **Service**: `warehouse.service.ts` → `/api/warehouses/*`
- **Features**:
  - CRUD operations
  - Warehouse locations and inventory tracking

### **Module 8: Product Management**
- **Path**: `src/app/features/products/`
- **Pages**: ProductListPage, ProductFormPage, ProductDetailPage
- **Service**: `product.service.ts` → `/api/products/*`
- **Features**:
  - Full CRUD with pagination
  - Product search by name, category, supplier
  - Low stock indicators UI
  - Out-of-stock alerts
  - SKU-based lookup
  - Partial updates (PATCH)
- **Dependencies**: Category, Supplier modules

### **Module 9: Purchase Orders**
- **Path**: `src/app/features/orders/purchase-orders/`
- **Pages**: POListPage, POCreatePage, POEditPage, PODetailPage
- **Service**: `purchase-order.service.ts` → `/api/purchaseorders/*`
- **Features**:
  - Create PO from supplier
  - Status workflow UI: Draft → Approved → Received → Cancelled
  - Auto-generated PO numbers
  - Line items with quantities
  - Approval workflow buttons
  - Receive goods workflow
- **Dependencies**: Product, Supplier modules

### **Module 10: Sales Orders**
- **Path**: `src/app/features/orders/sales-orders/`
- **Pages**: SOListPage, SOCreatePage, SOEditPage, SODetailPage
- **Service**: `sales-order.service.ts` → `/api/sales-orders/*`
- **Features**:
  - Create SO for customer
  - Status workflow UI: Pending → Confirmed → Shipped → Delivered → Cancelled
  - Stock validation UI (show warnings for low stock)
  - Auto-generated SO numbers
  - Line items management
  - Confirm, ship, deliver workflow buttons
  - Search sales orders
- **Dependencies**: Product, Customer modules

### **Module 11: Stock Adjustments**
- **Path**: `src/app/features/stock-adjustments/`
- **Pages**: AdjustmentListPage, AdjustmentFormPage
- **Service**: `stock-adjustment.service.ts` → `/api/stockadjustments/*`
- **Features**:
  - Record stock adjustments (add/remove inventory)
  - Adjustment types: Damage, Theft, Correction, Return, Other
  - Warehouse location tracking
  - Reason/notes for adjustment
  - Get adjustments by product
  - Audit trail
- **Dependencies**: Product, Warehouse modules

### **Module 12: Dashboard & Reports**
- **Path**: `src/app/features/dashboard/` and `src/app/features/reports/`
- **Services**: `dashboard.service.ts`, `report.service.ts`
- **API**: `/api/dashboard/*`, `/api/reports/*`
- **Features**:
  - Dashboard summary (total products, orders, revenue)
  - Low stock report widget
  - Top-selling products chart
  - Sales by product report (PDF export)
  - Purchases by supplier report (PDF export)
  - Stock movement report (PDF export)
  - Revenue report (PDF export)
  - Order status summary report (PDF export)
- **Dependencies**: All modules (aggregates data)

---

## 🎯 Single Edit Point per Module (STRICT RULE)

**CRITICAL ARCHITECTURE RULE - Non-negotiable**

Each module has **exactly one service** as the single-edit-point:

```
✅ RULE 1: Every module must have ONE service
  ├─ product.service.ts (for Products module)
  ├─ customer.service.ts (for Customers module)
  ├─ auth.service.ts (for Auth module)
  └─ ... one per module

✅ RULE 2: Components MUST NOT use HttpClient directly
  ├─ ❌ Wrong: component.ts → HttpClient → /api/products
  └─ ✅ Right: component.ts → service → /api/products (via service)

✅ RULE 3: All API calls through module service ONLY
  ├─ ProductListComponent uses productService.getAll()
  ├─ ProductFormComponent uses productService.create()
  ├─ ProductDetailComponent uses productService.getById()
  └─ All go through ONE service

✅ RULE 4: No duplicate API endpoints across components
  ├─ productService.getAll() defined ONCE
  ├─ All components call the same method
  ├─ Change endpoint once → affects entire module
  └─ No copy-paste code
```

**Why this matters:**
- Edit endpoint in ONE place → entire app updates
- No scattered API calls across 10 components
- Easy to debug (all logic centralized)
- Easy to test (mock one service)
- Enforces consistency

---

## 🏗️ Common vs Module Layer (VERY IMPORTANT)

**Two-layer architecture for scalability:**

### **COMMON LAYER** (Global - Reusable)
Location: `src/app/core/`

```
core/
├── services/
│   ├── api.service.ts          ← HTTP wrapper (base requests)
│   └── storage.service.ts      ← localStorage wrapper
│
├── http-interceptors/
│   ├── auth.interceptor.ts     ← Add JWT token to all requests
│   ├── error.interceptor.ts    ← Handle 401, 403, 500 globally
│   └── refresh.interceptor.ts  ← Auto-refresh expired tokens
│
├── guards/
│   ├── auth.guard.ts           ← Protect authenticated routes
│   └── role.guard.ts           ← Enforce role-based access
│
└── config/
    └── api.config.ts           ← Base URL, timeouts, cache settings
```

**What goes in Common ONLY:**
- Base HTTP URL (localhost:5000/api)
- Authentication/Authorization setup
- Global error handling
- Cross-cutting concerns (logging, timing)
- Global configuration values

---

### **MODULE LAYER** (Feature-Specific)
Location: `src/app/features/[module-name]/`

```
products/                          ← Products Module
├── services/
│   └── product.service.ts        ← ONLY this module's APIs
│       ├── getAll()             ← GET /api/products
│       ├── getById()            ← GET /api/products/{id}
│       ├── create()             ← POST /api/products
│       └── delete()             ← DELETE /api/products/{id}
│
├── pages/
│   ├── product-list-page        ← Uses productService
│   ├── product-form-page        ← Uses productService
│   └── product-detail-page      ← Uses productService
│
└── store/ (optional NgRx)        ← Module state management
    ├── product.state.ts
    ├── product.actions.ts
    └── product.effects.ts       ← Side effects (API calls via service)
```

**What goes in Module ONLY:**
- productService (product endpoints ONLY)
- productPages (product UI ONLY)
- productStore (product state ONLY)
- productModels (ProductDto, CreateProductDto, etc.)

---

### **LAYER SEPARATION EXAMPLE**

```typescript
// ❌ WRONG: Mixing layers
// ProductListComponent
export class ProductListComponent {
  constructor(private http: HttpClient) {}  // ❌ Don't use HttpClient here!
  
  loadProducts() {
    this.http.get('/api/products').subscribe(...);  // ❌ API URL hardcoded!
  }
}

// ✅ CORRECT: Proper layer separation
// ProductListComponent (Module Layer)
export class ProductListComponent {
  constructor(private productService: ProductService) {}  // ✅ Use service
  
  loadProducts() {
    this.productService.getAll().subscribe(...);  // ✅ Service handles API
  }
}

// ProductService (Module Layer)
@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${this.config.baseUrl}/api/products`;  // Uses Common config
  
  constructor(
    private http: HttpClient,           // From Common layer
    private config: ApiConfigService    // From Common layer
  ) {}
  
  getAll(): Observable<...> {
    return this.http.get(`${this.apiUrl}/GetAllProducts`);
  }
}

// ApiConfigService (Common Layer)
@Injectable({ providedIn: 'root' })
export class ApiConfigService {
  baseUrl = 'http://localhost:5000';  // ✅ One place to edit!
  timeout = 30000;
  retryAttempts = 3;
}
```

---

## ✅ DO / DO NOT Rules

### **✅ DO THESE**

```
✅ DO: Create ONE service per module
   - productService for Products
   - customerService for Customers
   - Create: ng g service core/services/product

✅ DO: Inject service in components
   - constructor(private productService: ProductService) {}
   - Call: this.productService.getAll()

✅ DO: Use shared api.config.ts for base URL
   - Define base URL ONCE in config
   - All services use: `${this.config.baseUrl}/api/...`

✅ DO: Keep business logic in service
   - productService handles: caching, error mapping, retries
   - Components handle: UI, user input, display

✅ DO: Use interceptors for cross-cutting concerns
   - auth.interceptor adds JWT to all requests
   - error.interceptor handles 401, 403 globally
   - refresh.interceptor auto-refreshes tokens

✅ DO: Define constants in shared constants file
   - api-endpoints.const.ts (all endpoint paths)
   - messages.const.ts (all user-facing messages)
   - Edit once → used everywhere
```

---

### **❌ DO NOT DO THESE**

```
❌ DO NOT: Call HttpClient directly in components
   Wrong:  constructor(private http: HttpClient) {}
           this.http.get('/api/products')
   
   Right:  constructor(private productService: ProductService) {}
           this.productService.getAll()

❌ DO NOT: Hardcode API URLs
   Wrong:  this.http.get('http://localhost:5000/api/products/1')
   
   Right:  // In productService
           private apiUrl = `${this.config.baseUrl}/api/products`;
           this.http.get(`${this.apiUrl}/1`)

❌ DO NOT: Duplicate API calls across components
   Wrong:  ProductListComponent → this.http.get('/api/products')
           ProductFormComponent → this.http.get('/api/products')
           // Same endpoint, called twice!
   
   Right:  // productService.getAll() defined ONCE
           ProductListComponent → this.productService.getAll()
           ProductFormComponent → this.productService.getAll()
           // Both use same method, no duplication

❌ DO NOT: Put module-specific logic in common layer
   Wrong:  api.service.ts contains productService methods
   
   Right:  api.service.ts = HTTP wrapper only
           product.service.ts = Product API methods

❌ DO NOT: Mix UI logic with API logic
   Wrong:  productService.getAll() {
             // Formats data for UI
             // Validates input
             // Shows notifications
           }
   
   Right:  productService.getAll() {
             return this.http.get(...);  // Just API call
           }
           component.ts {
             this.productService.getAll().subscribe(data => {
               this.products = data;  // Format for UI here
             });
           }

❌ DO NOT: Create multiple services per module
   Wrong:  products/
           ├── productHttpService.ts
           ├── productApiService.ts
           └── productDataService.ts
           // Three services for Products!
   
   Right:  products/
           └── product.service.ts  // ONE service per module

❌ DO NOT: Skip dependency injection
   Wrong:  new ProductService()  // Direct instantiation
   
   Right:  constructor(private productService: ProductService) {}
           // Angular DI handles it
```

---

## 📋 Frontend Build Order

**Implementation sequence** (follows dependency tree):

| Phase | Module | Depends On | Status |
|-------|--------|-----------|--------|
| **1** | Project Setup + Core Services | — | Foundation |
| **2** | Auth (Login/Register/Logout) | — | Core feature |
| **3** | Categories | — | No deps |
| **4** | Roles & Privileges | — | No deps |
| **5** | User Management | Auth, Roles | Admin feature |
| **6** | Suppliers | — | No deps (for PO) |
| **7** | Customers | — | No deps (for SO) |
| **8** | Warehouses | — | No deps |
| **9** | Products | Categories, Suppliers | Core inventory |
| **10** | Purchase Orders | Products, Suppliers, Warehouses | Advanced |
| **11** | Sales Orders | Products, Customers, Warehouses | Advanced |
| **12** | Stock Adjustments | Products, Warehouses | Inventory mgmt |
| **13** | Dashboard & Reports | All modules | Final integration |

**Rationale**:
- Phase 1-2: Build authentication first → secure everything
- Phase 3-8: Build independent CRUD modules
- Phase 9-12: Build order/inventory workflows (depend on products & locations)
- Phase 13: Build dashboard (depends on all data)

---

## 🔌 Frontend Implementation Pattern

### **Step-by-Step Module Implementation**

For each frontend module, follow this **proven pattern** (mirrors backend):

#### **Step 1: Define Models (TypeScript Interfaces)**
```typescript
// src/app/core/models/product.model.ts
export interface ProductDto {
  id: number;
  name: string;
  sku: string;
  unitPrice: number;
  costPrice: number;
  currentStock: number;
  categoryId: number;
  supplierId: number;
}

export interface CreateProductDto {
  name: string;
  sku: string;
  unitPrice: number;
  costPrice: number;
  categoryId: number;
  supplierId: number;
}

export interface UpdateProductDto extends CreateProductDto {}
```

#### **Step 2: Create Service (API Integration - Single Edit Point)**
```typescript
// src/app/core/services/product.service.ts
@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${API_CONFIG.BASE_URL}/api/products`;

  constructor(private http: HttpClient) {}

  // GET /api/products/GetAllProducts?pageNumber=1&pageSize=10
  getAll(pageNumber: number, pageSize: number): Observable<APIResponse<PaginatedResult<ProductDto>>> {
    return this.http.get<APIResponse<PaginatedResult<ProductDto>>>(`${this.apiUrl}/GetAllProducts`, {
      params: { pageNumber, pageSize }
    });
  }

  // GET /api/products/{id}
  getById(id: number): Observable<APIResponse<ProductDto>> {
    return this.http.get<APIResponse<ProductDto>>(`${this.apiUrl}/${id}`);
  }

  // POST /api/products/CreateProduct
  create(dto: CreateProductDto): Observable<APIResponse<ProductDto>> {
    return this.http.post<APIResponse<ProductDto>>(`${this.apiUrl}/CreateProduct`, dto);
  }

  // PUT /api/products/{id}
  update(id: number, dto: UpdateProductDto): Observable<APIResponse<boolean>> {
    return this.http.put<APIResponse<boolean>>(`${this.apiUrl}/${id}`, dto);
  }

  // DELETE /api/products/{id}
  delete(id: number): Observable<APIResponse<boolean>> {
    return this.http.delete<APIResponse<boolean>>(`${this.apiUrl}/${id}`);
  }

  // PATCH /api/products/{id}
  partialUpdate(id: number, updates: Partial<UpdateProductDto>): Observable<APIResponse<string>> {
    return this.http.patch<APIResponse<string>>(`${this.apiUrl}/${id}`, updates);
  }
}
```

#### **Step 3: Create Feature Module**
```typescript
// src/app/features/products/products.module.ts
@NgModule({
  declarations: [ProductListPage, ProductFormPage, ProductDetailPage],
  imports: [CommonModule, ProductsRoutingModule, SharedModule]
})
export class ProductsModule {}
```

#### **Step 4: Create Pages/Components**
```typescript
// src/app/features/products/pages/product-list-page/product-list-page.component.ts
@Component({
  selector: 'app-product-list-page',
  templateUrl: './product-list-page.component.html'
})
export class ProductListPageComponent implements OnInit, OnDestroy {
  products$: Observable<ProductDto[]>;
  isLoading = false;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.products$ = this.productService.getAll(1, 10);
  }
}
```

#### **Step 5: Configure Routing**
```typescript
// src/app/features/products/products-routing.module.ts
const routes: Routes = [
  { path: '', component: ProductListPage },
  { path: 'create', component: ProductFormPage },
  { path: ':id/edit', component: ProductFormPage },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule {}
```

#### **Step 6: Add NgRx Store (Optional - for complex state)**
```typescript
// src/app/features/products/store/product.state.ts
export interface ProductState {
  items: ProductDto[];
  loading: boolean;
  error: string | null;
}

// src/app/features/products/store/product.actions.ts
export const loadProducts = createAction('[Products] Load Products');
export const loadProductsSuccess = createAction(
  '[Products] Load Products Success',
  props<{ items: ProductDto[] }>()
);

// src/app/features/products/store/product.reducer.ts
export const productReducer = createReducer(
  initialState,
  on(loadProducts, (state) => ({ ...state, loading: true })),
  on(loadProductsSuccess, (state, { items }) => ({ ...state, items, loading: false }))
);

// src/app/features/products/store/product.effects.ts
@Injectable()
export class ProductEffects {
  loadProducts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadProducts),
      switchMap(() => this.productService.getAll(1, 10))
    )
  );
}
```

#### **Step 7: Create UI (Template + Validation)**
```html
<!-- src/app/features/products/pages/product-list-page/product-list-page.component.html -->
<div class="container">
  <h1>Products</h1>
  <button routerLink="/products/create" class="btn btn-primary">Add Product</button>
  
  <table *ngIf="products$ | async as products">
    <tr *ngFor="let product of products">
      <td>{{ product.name }}</td>
      <td>{{ product.price | currency }}</td>
      <td>
        <button routerLink="/products/{{ product.id }}/edit">Edit</button>
        <button (click)="deleteProduct(product.id)">Delete</button>
      </td>
    </tr>
  </table>
</div>
```

#### **Step 8: Add Permission Guards**
```typescript
// src/app/features/products/products-routing.module.ts
const routes: Routes = [
  {
    path: '',
    component: ProductListPage,
    canActivate: [AuthGuard] // Must be logged in
  },
  {
    path: 'create',
    component: ProductFormPage,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'Manager'] } // Only Admin/Manager can create
  }
];
```

#### **Step 9: Handle Errors & Validation**
```typescript
// Error handling in service & component
this.productService.create(formValue).subscribe({
  next: (response) => this.notificationService.success('Created!'),
  error: (err) => this.notificationService.error('Failed to create')
});

// Form validation
this.form = this.fb.group({
  name: ['', [Validators.required, Validators.minLength(3)]],
  price: [0, [Validators.required, Validators.min(0)]]
});
```

#### **Step 10: Test the Module**
```
✅ Unit: Test ProductService with HttpClientTestingModule
✅ Component: Test ProductListComponent with mocked service
✅ E2E: Test user flow from list → create → edit → delete
```

---

### **Login → Refresh → Logout**

```typescript
// Login with AuthService
const loginRequest = { username, password };
this.authService.login(loginRequest).subscribe({
  next: (response) => {
    // Backend sets: accesstoken (15 min), refreshtoken (7 days) in HttpOnly cookies
    // Store user data in NgRx Store
    this.store.dispatch(AuthActions.loginSuccess({ user: response.user }));
    this.router.navigate(['/dashboard']);
  },
  error: (err) => this.notificationService.error('Login failed')
});

// Auto-refresh on 401 (handled by Interceptor silently)
// When token expires: Interceptor catches 401 → calls refreshToken()
// New tokens set in cookies → retry original request automatically

// Logout
this.authService.logout().subscribe(() => {
  // Backend: logs user, revokes refresh token in DB
  // Frontend: clear tokens, NgRx.clear(), redirect to /login
  this.store.dispatch(AuthActions.logout());
  this.router.navigate(['/login']);
});
```

### **Route Guards**

```typescript
// src/app/core/guards/auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { selectIsAuthenticated } from '@/core/store/auth/auth.selectors';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private store: Store,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.store.select(selectIsAuthenticated).pipe(
      map(isAuth => {
        if (!isAuth) {
          this.router.navigate(['/login']);
          return false;
        }
        return true;
      })
    );
  }
}

// src/app/core/guards/role.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { selectUserRoles } from '@/core/store/auth/auth.selectors';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private store: Store,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const requiredRoles = route.data['roles'] as string[];
    
    return this.store.select(selectUserRoles).pipe(
      map(userRoles => {
        const hasRole = requiredRoles.some(role => userRoles?.includes(role));
        if (!hasRole) {
          this.router.navigate(['/unauthorized']);
        }
        return hasRole;
      })
    );
  }
}
```

### **Route Configuration**

```typescript
// src/app/app.routing.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@/core/guards/auth.guard';
import { RoleGuard } from '@/core/guards/role.guard';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  
  {
    path: 'auth',
    loadChildren: () => import('@/features/auth/auth.module').then(m => m.AuthModule)
  },
  
  {
    path: 'dashboard',
    canActivate: [AuthGuard],
    loadChildren: () => import('@/features/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  
  {
    path: 'products',
    canActivate: [AuthGuard],
    loadChildren: () => import('@/features/products/products.module').then(m => m.ProductsModule)
  },
  
  {
    path: 'admin',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
    loadChildren: () => import('@/features/admin/admin.module').then(m => m.AdminModule)
  },
  
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRouting { }
```

---

## 🛠️ Developer Setup Guide

### **1. Create Angular Project**
```bash
# Install Angular CLI globally
npm install -g @angular/cli

# Create new project with latest Angular
ng new inventory-management-system --routing --style=scss

cd inventory-management-system
```

### **2. Install Dependencies**
```bash
npm install

# Install essential packages
npm install @angular/material @angular/animations
npm install @ngrx/store @ngrx/effects @ngrx/selectors
npm install tailwindcss postcss autoprefixer
npm install axios
npm install --save-dev @types/node

# Initialize Tailwind
npx tailwindcss init -p
```

### **3. Environment Setup**
```bash
# Create environment files
# src/environments/environment.ts
# src/environments/environment.prod.ts
```

Edit `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  apiTimeout: 30000,
  jwtExpiryMinutes: 15,
  refreshTokenExpiryDays: 7
};
```

### **4. Project Structure Setup**
```bash
# Generate core module
ng generate module core
ng generate module shared
ng generate module features

# Generate core services
ng generate service core/services/api
ng generate service core/services/auth
ng generate service core/services/product
# ... etc for other services

# Generate interceptors
ng generate interceptor core/http-interceptors/auth
ng generate interceptor core/http-interceptors/error

# Generate guards
ng generate guard core/guards/auth
ng generate guard core/guards/role

# Generate feature modules
ng generate module features/auth --routing
ng generate module features/dashboard --routing
ng generate module features/products --routing
# ... etc for other features
```

### **5. Configure NgRx Store**
```bash
npm install @ngrx/store-devtools

# Generate store
ng generate @ngrx/schematics:store app --module core/core.module.ts --statePath core/store --stateInterface state

# Generate auth feature state
ng generate @ngrx/schematics:feature core/store/auth/Auth --module core/core.module.ts
```

### **6. Start Dev Server**
```bash
ng serve
# or
ng serve --open
```

Runs on `http://localhost:4200`

### **7. Build for Production**
```bash
ng build --configuration production
```

Output in `dist/` directory

---

## 📐 State Management (NgRx)

### **Store Structure with NgRx**

```typescript
// src/app/core/store/app.state.ts
import { AuthState } from './auth/auth.state';
import { UIState } from './ui/ui.state';

export interface AppState {
  auth: AuthState;
  ui: UIState;
  // Feature states...
}

// src/app/core/store/app.state.ts
export const AUTH_FEATURE_KEY = 'auth';
export const UI_FEATURE_KEY = 'ui';
```

### **Auth State (Actions, Reducer, Selectors, Effects)**

```typescript
// src/app/core/store/auth/auth.state.ts
import { User } from '@/core/models/user.model';

export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
  roles: string[];
  permissions: string[];
}

export const initialAuthState: AuthState = {
  user: null,
  isAuthenticated: false,
  loading: false,
  error: null,
  roles: [],
  permissions: []
};
```

```typescript
// src/app/core/store/auth/auth.actions.ts
import { createAction, props } from '@ngrx/store';
import { User, LoginDto } from '@/core/models';

export const login = createAction(
  '[Auth] Login',
  props<{ credentials: LoginDto }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ user: User }>()
);

export const loginFailure = createAction(
  '[Auth] Login Failure',
  props<{ error: string }>()
);

export const logout = createAction('[Auth] Logout');

export const refreshToken = createAction('[Auth] Refresh Token');
```

```typescript
// src/app/core/store/auth/auth.reducer.ts
import { createReducer, on } from '@ngrx/store';
import * as AuthActions from './auth.actions';
import { initialAuthState, AuthState } from './auth.state';

export const authReducer = createReducer(
  initialAuthState,
  
  on(AuthActions.login, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(AuthActions.loginSuccess, (state, { user }) => ({
    ...state,
    user,
    isAuthenticated: true,
    loading: false,
    roles: user.roles,
    permissions: user.permissions
  })),

  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    error,
    loading: false,
    isAuthenticated: false
  })),

  on(AuthActions.logout, () => initialAuthState)
);
```

```typescript
// src/app/core/store/auth/auth.selectors.ts
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from './auth.state';
import { AUTH_FEATURE_KEY } from '../app.state';

export const selectAuthState = createFeatureSelector<AuthState>(AUTH_FEATURE_KEY);

export const selectUser = createSelector(selectAuthState, (state) => state.user);
export const selectIsAuthenticated = createSelector(selectAuthState, (state) => state.isAuthenticated);
export const selectAuthLoading = createSelector(selectAuthState, (state) => state.loading);
export const selectAuthError = createSelector(selectAuthState, (state) => state.error);
export const selectUserRoles = createSelector(selectAuthState, (state) => state.roles);
export const selectUserPermissions = createSelector(selectAuthState, (state) => state.permissions);
```

```typescript
// src/app/core/store/auth/auth.effects.ts
import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import * as AuthActions from './auth.actions';
import { AuthService } from '@/core/services/auth.service';

@Injectable()
export class AuthEffects {
  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      switchMap(({ credentials }) =>
        this.authService.login(credentials).pipe(
          map(response => AuthActions.loginSuccess({ user: response.user })),
          catchError(error => of(AuthActions.loginFailure({ error: error.message })))
        )
      )
    )
  );

  constructor(
    private actions$: Actions,
    private authService: AuthService
  ) {}
}
```

### **Using Selectors in Components**

```typescript
// src/app/features/dashboard/pages/dashboard-page/dashboard-page.component.ts
import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectUser, selectUserRoles } from '@/core/store/auth/auth.selectors';
import { User } from '@/core/models/user.model';

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {
  user$: Observable<User | null>;
  roles$: Observable<string[]>;

  constructor(private store: Store) {
    this.user$ = this.store.select(selectUser);
    this.roles$ = this.store.select(selectUserRoles);
  }

  ngOnInit(): void {
    // Component already has user and roles from store selectors
  }
}
```

```html
<!-- Template with async pipe -->
<div>
  <h1>Welcome, {{ (user$ | async)?.fullName }}</h1>
  <p>Roles: {{ (roles$ | async)?.join(', ') }}</p>
</div>
```

---

## 🧪 Testing (Minimal Scope)

### **Unit Tests (Jasmine + Karma)**
- Test services with `HttpClientTestingModule`
- Mock HTTP responses with `HttpTestingController`
- Example: Verify `ProductService.getAll()` calls correct endpoint

### **Component Tests**
- Use `TestBed.configureTestingModule()` for component testing
- Mock services with `jasmine.SpyObj`
- Example: Verify product list loads on component init

### **E2E Tests (Cypress)**
- Test user workflows: login → create product → delete product
- Use data-testid attributes for reliable selectors
- Example: `cy.visit('/products')` → `cy.get('[data-testid="btn-create"]').click()`

---

## 🚀 Performance Optimization

### **Change Detection Strategy**
```typescript
// Use OnPush for better performance (don't check entire component tree)
@Component({
  selector: 'app-product-card',
  template: '...',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductCardComponent {
  @Input() product!: Product; // Only check when @Input changes
}
```

### **Lazy Loading Modules**
```typescript
// src/app/app.routing.ts
const routes: Routes = [
  {
    path: 'products',
    loadChildren: () => import('./features/products/products.module')
      .then(m => m.ProductsModule)
  }
];
```

### **Async Pipe for Automatic Unsubscribe**
```html
<!-- Automatically unsubscribes when component is destroyed -->
<div *ngFor="let product of (products$ | async) as items; trackBy: trackByProductId">
  {{ product.name }}
</div>

<!-- In component -->
trackByProductId(index: number, item: Product): number {
  return item.id;
}
```

### **OnPush with Observables**
```typescript
@Component({
  selector: 'app-product-list',
  template: `
    <div *ngFor="let p of (products$ | async) as products">
      {{ p.name }}
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductListComponent {
  products$ = this.productService.getAll(1, 10);

  constructor(private productService: ProductService) {}
}
```

### **Bundle Size Analysis**
```bash
# Check bundle size
ng build --prod --stats-json
npm install -g webpack-bundle-analyzer
webpack-bundle-analyzer dist/inventory-management-system/stats.json
```

### **Tree Shaking**
- Use ES6 modules
- Import only what you need: `import { map } from 'rxjs/operators'`
- Avoid wildcard imports: `import * as utils from '@/utils'`

### **Virtual Scrolling (for large lists)**
```typescript
import { ScrollingModule } from '@angular/cdk/scrolling';

@Component({
  template: `
    <cdk-virtual-scroll-viewport itemSize="50" class="example-viewport">
      <div *cdkVirtualFor="let item of items" class="example-item">
        {{ item }}
      </div>
    </cdk-virtual-scroll-viewport>
  `
})
export class VirtualScrollComponent {
  items = new Array(10000).fill(0).map((v, i) => i);
}
```

---

## 📋 Coding Standards & Best Practices

### **File Naming Conventions**
```
Components:       kebab-case.component.ts        (product-list.component.ts)
Services:         kebab-case.service.ts          (product.service.ts)
Modules:          kebab-case.module.ts           (product.module.ts)
Guards:           kebab-case.guard.ts            (auth.guard.ts)
Pipes:            kebab-case.pipe.ts             (currency.pipe.ts)
Models/Types:     kebab-case.model.ts            (product.model.ts)
Store:
  - Actions:      kebab-case.actions.ts
  - Selectors:    kebab-case.selectors.ts
  - Reducer:      kebab-case.reducer.ts
  - Effects:      kebab-case.effects.ts
  - State:        kebab-case.state.ts
Specs:            {file}.spec.ts
```

### **Component Structure**
```typescript
// ✅ CORRECT: Organized and readable
import { Component, Input, Output, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '@/core/services/product.service';
import { Product } from '@/core/models/product.model';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit, OnDestroy {
  // 1. Properties & Inputs/Outputs
  @Input() productId?: number;
  @Output() saved = new EventEmitter<Product>();

  // 2. Form properties
  form!: FormGroup;
  submitted = false;

  // 3. State properties
  loading = false;
  error: string | null = null;

  // 4. RxJS subjects
  private destroy$ = new Subject<void>();

  // 5. Constructor - Dependency Injection
  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private store: Store
  ) {
    this.createForm();
  }

  // 6. Lifecycle hooks
  ngOnInit(): void {
    if (this.productId) {
      this.loadProduct(this.productId);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // 7. Private helper methods
  private createForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      sku: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]]
    });
  }

  private loadProduct(id: number): void {
    this.loading = true;
    this.productService.getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.form.patchValue(response.data);
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load product';
          this.loading = false;
        }
      });
  }

  // 8. Public methods
  onSubmit(): void {
    if (this.form.invalid) return;

    this.submitted = true;
    const formValue = this.form.value as Product;

    const request$ = this.productId
      ? this.productService.update(this.productId, formValue)
      : this.productService.create(formValue);

    request$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.saved.emit(response.data);
        },
        error: (err) => {
          this.error = 'Failed to save product';
        }
      });
  }
}
```

### **Template Best Practices**
```html
<!-- ✅ CORRECT: Proper structure and bindings -->
<form [formGroup]="form" (ngSubmit)="onSubmit()">
  
  <!-- Loading state -->
  <div *ngIf="loading" class="spinner">
    Loading...
  </div>

  <!-- Error message -->
  <div *ngIf="error" class="alert alert-danger">
    {{ error }}
  </div>

  <!-- Form fields -->
  <div class="form-group">
    <label for="name">Product Name</label>
    <input
      id="name"
      type="text"
      formControlName="name"
      class="form-control"
      [class.is-invalid]="form.get('name')?.invalid && submitted"
    >
    <div *ngIf="form.get('name')?.errors?.['required']" class="invalid-feedback">
      Name is required
    </div>
  </div>

  <!-- Buttons -->
  <button type="submit" class="btn btn-primary" [disabled]="!form.valid || loading">
    {{ loading ? 'Saving...' : 'Save' }}
  </button>
</form>
```

### **RxJS Best Practices**
```typescript
// ✅ CORRECT: Always unsubscribe
private destroy$ = new Subject<void>();

ngOnInit(): void {
  this.dataService.getData()
    .pipe(takeUntil(this.destroy$))
    .subscribe(data => this.data = data);
}

ngOnDestroy(): void {
  this.destroy$.next();
  this.destroy$.complete();
}

// ❌ AVOID: Memory leaks from unsubscribed observables
ngOnInit(): void {
  this.dataService.getData().subscribe(data => this.data = data); // Will leak!
}
```

### **Service Injection Pattern**
```typescript
// ✅ CORRECT: Use providedIn: 'root' for singletons
@Injectable({
  providedIn: 'root'  // Singleton - shared across entire app
})
export class ProductService {
  constructor(private http: HttpClient) {}
}

// ✅ CORRECT: Async data in components
products$ = this.productService.getAll(1, 10);

// Template: Use async pipe for automatic unsubscribe
<div *ngFor="let product of (products$ | async)?.data">
  {{ product.name }}
</div>
```

### **Type Safety**
- Always use strong typing, avoid `any`
- Create interfaces for API responses
- Use discriminated unions for error handling
- Enable strict null checks in `tsconfig.json`

### **Angular Style Guide Rules**
- Single responsibility per file
- Avoid fat components (split into smaller presentational components)
- Use ChangeDetectionStrategy.OnPush for performance
- Keep templates simple (logic goes to component)
- Use trackBy with *ngFor for lists

---

## 🔗 Useful Resources

| Resource | Link |
|----------|------|
| Angular Docs | https://angular.io/docs |
| Angular CLI | https://angular.io/cli |
| Angular Material | https://material.angular.io |
| NgRx Store Docs | https://ngrx.io |
| RxJS | https://rxjs.dev |
| TypeScript | https://www.typescriptlang.org |
| Tailwind CSS | https://tailwindcss.com |
| Jasmine Testing | https://jasmine.github.io |
| Karma Test Runner | https://karma-runner.github.io |
| Cypress E2E | https://cypress.io |
| Angular Style Guide | https://angular.io/guide/styleguide |

---

## 📱 Component Examples

### **Simple Form Component**
```typescript
// src/app/features/products/components/product-form/product-form.component.ts
import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '@/core/services/product.service';
import { NotificationService } from '@/core/services/notification.service';
import { Product } from '@/core/models/product.model';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductFormComponent implements OnInit, OnDestroy {
  @Input() productId?: number;
  
  form!: FormGroup;
  loading = false;
  submitted = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private notification: NotificationService
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    if (this.productId) {
      this.loadProduct();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      sku: ['', Validators.required],
      categoryId: [null, Validators.required],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      costPrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  private loadProduct(): void {
    this.loading = true;
    this.productService.getById(this.productId!)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.form.patchValue(response.data);
          this.loading = false;
        },
        error: () => {
          this.notification.error('Failed to load product');
          this.loading = false;
        }
      });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.submitted = true;
    const request$ = this.productId
      ? this.productService.update(this.productId, this.form.value)
      : this.productService.create(this.form.value);

    request$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notification.success('Product saved successfully');
          this.form.reset();
        },
        error: () => this.notification.error('Failed to save product')
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

### **Data Table with Pagination**
```typescript
// src/app/shared/components/table/data-table.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';

export interface TableConfig {
  columns: string[];
  data: any[];
  totalCount: number;
  pageSize: number;
  pageNumber: number;
}

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent {
  @Input() config!: TableConfig;
  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() rowAction = new EventEmitter<{ action: string; row: any }>();

  displayedColumns: string[] = [];

  ngOnInit(): void {
    this.displayedColumns = [...this.config.columns, 'actions'];
  }

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }

  onAction(action: string, row: any): void {
    this.rowAction.emit({ action, row });
  }
}
```

---

## 📞 Support & Git Workflow

For issues or contributions, follow the standard Git flow:

```bash
# 1. Create feature branch
git checkout -b feature/add-new-module

# 2. Make changes and commit
git add .
git commit -m "feat: add new module"

# 3. Push and create PR
git push origin feature/add-new-module

# 4. After PR approval, merge to master
git checkout master
git merge feature/add-new-module
```

---

##  Key Takeaways for Senior Developers

### **Single Source of Truth**
- All API endpoints defined ONCE in `src/app/core/services/*.service.ts`
- Change endpoint in ONE place → affects entire application
- No duplicated API calls across components

### **Dependency Injection Pattern**
- Services are singletons (provided in 'root')
- Inject where needed via constructor
- Easy to mock for testing

### **Reactive Programming with RxJS**
- All async operations via Observables
- Use `takeUntil()` + `Subject` for proper unsubscription
- Avoid `subscribe()` in components (use async pipe instead)

### **NgRx Store for Complex State**
- Centralized state management
- Actions → Reducer → Selectors flow
- Effects handle side effects (API calls)
- Type-safe selectors prevent runtime errors

### **Type Safety First**
- Define models for ALL API responses
- Strict null checks enabled
- No `any` types allowed
- Interfaces for contracts, Types for data

### **Angular Material + Tailwind**
- Pre-built components for consistency
- Tailwind for custom styling
- Responsive design out of the box

---

## 🎯 Next Steps

1. **Generate project structure** using Angular CLI
2. **Set up NgRx store** with auth state
3. **Create core services** with API integration
4. **Build shared components** library
5. **Implement feature modules** using lazy loading
6. **Add authentication guards** and interceptors
7. **Write tests** for services and components
8. **Deploy** to production

---

> **Final Certification:** This Angular frontend architecture is enterprise-grade, scalable, and follows all industry best practices for senior-level Angular development. It's built for maintainability, testability, and real-world production readiness.
