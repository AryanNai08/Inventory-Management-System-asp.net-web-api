# 📦 Inventory Management System — Final Project Documentation

> A **production-grade** ASP.NET Core Web API built with **Pure Clean Architecture**, **Dynamic RBAC**, and **High-Performance Stateless/Stateful Auth**.

---

## 📌 Tech Stack

| Layer            | Technology                                 |
| ---------------- | ------------------------------------------ |
| **Framework**    | ASP.NET Core (.NET 10) Web API             |
| **Architecture** | **100% Pure Clean Architecture** (Decoupled Infrastructure) |
| **Database**     | SQL Server — `InventoryManagementDB`       |
| **Auth**         | JWT + HttpOnly Cookies + **Stateful Logout** |
| **RBAC**         | **Dynamic Permission-Based** (Policy-Driven) |
| **Reporting**    | RDLC PDF Engine (Enterprise Analytics)     |
| **Mapping**      | AutoMapper (Entity ↔ DTO)                  |
| **Email**        | MailKit (SMTP) with direct string config   |
| **Security**     | BCrypt.Net (Hashing) + Git-Ignored Secrets |

---

## 🏗️ Pure Clean Architecture

This project follows a strict **Pure Clean Architecture** pattern. The layers are decoupled to ensure that the core business logic (Domain) remains independent of any technical implementation details.

### **Dependency Rules**
1.  **Domain**: 0 dependencies. Contains Entities, Exceptions, and **Technical Strategy Interfaces** (e.g., `ICurrentUserService`, `IEmailService`).
2.  **Application**: Depends ONLY on **Domain**. Contains DTOs, Business Use Cases (Services), and Mappings.
3.  **Infrastructure**: Depends ONLY on **Domain**. Implements database context, repositories, and technical services. **Critically, it has 0 reference to the Application layer.**
4.  **Web API**: The Composition Root. References all layers to wire up dependencies.

### **Project Structure**
```
InventoryManagementSystem/
│
├── Docs/                             # Project documentation
│
└── InvMS/                            # .NET Solution root
    │
    ├── Domain/                       # The "Heart": No outer dependencies
    │   ├── Entities/                 # Core Data Models
    │   ├── Interfaces/               # ALL technical & repository contracts
    │   └── Common/                   # PaginatedResult.cs, APIResponse.cs
    │
    ├── Application/                  # Business Logic & DTOs
    │   ├── DTOs/                     # Strongly-typed data transfers
    │   ├── Services/                 # Business use-case logic
    │   ├── Mappings/                 # AutoMapper Profile
    │   └── Security/                 # Dynamic RBAC engine
    │
    ├── Infrastructure/               # External Implementation (Decoupled)
    │   ├── Data/                     # DbContext & UnitOfWork
    │   ├── Repositories/             # EF Core Implementations
    │   └── ThirdPartyServices/       # Email & PDF generation logic
    │
    └── InvMS/ (Web API Layer)        # Entry point & Config
        ├── Controller/               # API Endpoints
        ├── Middleware/               # Global Error Handling
        ├── Program.cs                # Configuration & DI
        └── appsettings.json          # [UNTRACKED] - Local secret storage
```

---

## 🔐 Advanced Security Features

### **1. Stateful Logout (Revocation)**
Unlike standard stateless JWTs, this system implements a **Stateful Logout**. When a user logs out:
- The server clears the `accesstoken` and `refreshtoken` cookies.
- The server revokes the internal `RefreshToken` in the database.
- Even if a JWT was stolen, the session cannot be restored because the refresh path is dead.

### **2. High-Performance Configuration**
The system uses the **"Direct Configuration"** pattern for secrets:
- Secrets (JWT Key, SMTP Password) are stored directly in `appsettings.json`.
- This eliminates slow, blocking `File.ReadAllText` calls on every authentication request.
- Performance is optimized for sub-millisecond token generation.

### **3. Git-Ignored Configuration**
For security, `appsettings.json` is **Git-Ignored**. This prevents sensitive database strings and API keys from leaking into version control.

---

## 🛠️ Developer Setup Guide

Because `appsettings.json` is protected and not tracked in Git, new developers must follow these steps to get the project running:

1.  **Clone the Repo**: `git clone <repo-url>`
2.  **Create appsettings.json**: Create the file in `InvMS/InvMS/appsettings.json`.
3.  **Populate Configuration**:
    ```json
    {
      "ConnectionStrings": {
        "InventoryDb": "Your-SQL-Server-Connection-String"
      },
      "Jwt": {
        "SecretKey": "Your-Minimum-32-Char-Key",
        "Issuer": "InvMS",
        "Audience": "InvMS-Users",
        "ExpiryInMinutes": 15
      },
      "EmailSettings": {
        "SenderEmail": "Your-Mail-Address",
        "Password": "Your-App-Specific-Password"
      }
    }
    ```
4.  **Install RDLC Support**: Ensure the system has the Microsoft RDLC extension or report viewer installed for analytics.

---

## 📡 API Modules & Capabilities (78 Endpoints)

The system supports 12 full modules, including:
- **Auth**: Stateless auth + Stateful revocation.
- **Inventory Control**: Multi-warehouse stock tracking.
- **Transactions**: Atomic Purchase & Sales orders with DB Transaction safety.
- **RBAC**: Dynamically mapping permissions to granular controller policies.

---

## 📈 Industry-Grade Principles Found in This Project
- **Optimistic Concurrency**: Uses `RowVersion` to prevent race conditions during stock updates.
- **Repository Pattern**: Abstracted data access allows for easier testing and DB switching.
- **Atomic Workflows**: Uses `IUnitOfWork` to ensure complex orders either save completely or not at all.
- **Global Error Handling**: Standardized `APIResponse<T>` ensures the frontend never sees raw server errors.

---

> **Final Certification:** This backend is architected for stability, speed, and real-world scalability.
