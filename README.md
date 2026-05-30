# ShopNest E-Commerce API 🛍️

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen?style=flat-square)](#)

ShopNest is a robust, scalable, and modern E-Commerce REST API built with **.NET 10**. Designed with best practices in mind, it utilizes **Clean Architecture** and **CQRS** (Command Query Responsibility Segregation) patterns to ensure high maintainability, separation of concerns, and scalability.

---

## 🚀 Features

* **Identity & Security**: Full JWT-based authentication (Login, Registration, Refresh Tokens, Password Reset, Email Confirmation) using ASP.NET Core Identity.
* **Product Catalog**: Manage products, categories, inventory stock, and product images (with slugs and primary images).
* **Shopping Experience**: Functional Cart and Wishlist management.
* **Order Processing**: Complete checkout flow, order placement, and coupon/discount application.
* **Payments Integration**: Secure payment processing utilizing **Stripe.net** (Payment Intents, Webhooks, Refunds).
* **User Engagement**: Product reviews and a built-in notification system.

## 🛠️ Tech Stack

* **Framework**: .NET 10.0 (ASP.NET Core Web API)
* **Architecture**: Clean Architecture & CQRS (MediatR)
* **Database**: SQL Server (Entity Framework Core)
* **Caching**: Redis (StackExchange.Redis)
* **Validation**: FluentValidation
* **Object Mapping**: AutoMapper
* **Logging**: Serilog
* **Payment Gateway**: Stripe SDK
* **Emails**: SendGrid
* **API Documentation**: Scalar

## 🏗️ Architecture

The solution follows the **Clean Architecture** (Onion Architecture) principles, dividing the application into four distinct layers:

1. **ShopNest.Domain**: Contains core entities, enums, exceptions, and domain events. (No external dependencies).
2. **ShopNest.Application**: Business logic layer containing MediatR Commands/Queries, DTOs, interfaces, and mapping profiles.
3. **ShopNest.Infrastructure**: Implementation of data access (`AppDbContext`), Identity, and external services (Stripe, SendGrid, LocalFileService).
4. **ShopNest.API**: The presentation layer serving as the entry point, containing Controllers, middlewares, and Dependency Injection wiring.

## 💻 Getting Started

### Prerequisites

Ensure you have the following installed on your machine:
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB, Developer Edition, or Docker container)
* Redis Server (Local or Docker container: `docker run -d -p 6379:6379 redis`)
* Stripe Account (for payment integration keys)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/ShopNest.git
   cd ShopNest
   ```

2. **Configure AppSettings**
   Navigate to `src/ShopNest.API/appsettings.Development.json` (or set up User Secrets) and configure your connection strings and external keys:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ShopNestDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
       "Redis": "localhost:6379"
     },
     "JwtSettings": {
       "SecretKey": "YOUR_SUPER_SECRET_JWT_KEY_HERE_MAKE_IT_LONG",
       "Issuer": "ShopNestAPI",
       "Audience": "ShopNestClients"
     },
     "Stripe": {
       "PublishableKey": "pk_test_...",
       "SecretKey": "sk_test_...",
       "WebhookSecret": "whsec_..."
     },
     "EmailSettings": {
       "SendGridKey": "SG.your_sendgrid_key",
       "FromEmail": "noreply@shopnest.com",
       "FromName": "ShopNest"
     }
   }
   ```

3. **Apply Database Migrations**
   Run the following command from the root of the repository to create the database schema:
   ```bash
   dotnet ef database update --project src/ShopNest.Infrastructure --startup-project src/ShopNest.API
   ```

4. **Run the Application**
   Start the API project:
   ```bash
   dotnet run --project src/ShopNest.API
   ```

5. **Explore the API**
   Once the application is running, open your browser and navigate to the Scalar documentation UI to explore and test the endpoints:
   ```
   https://localhost:<port>/scalar/v1
   ```

## 📁 Project Structure

```text
ShopNest/
├── src/
│   ├── ShopNest.API/            # Controllers, Middleware, API configurations
│   ├── ShopNest.Application/    # CQRS Handlers, DTOs, Interfaces, Validators
│   ├── ShopNest.Domain/         # Core Entities, Value Objects, Enums
│   └── ShopNest.Infrastructure/ # EF Core DbContext, Identity, Stripe Service
└── tests/                       # Unit and Integration Tests
```

## 🤝 Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

Distributed under the MIT License. See the `LICENSE` file for more information.
