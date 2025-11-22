# ServiceSystem

A comprehensive ASP.NET Core 8 service management system built with Clean Architecture principles, designed to manage service requests, payments, reviews, and real-time tracking.

## 🏗️ Architecture

This project follows **Clean Architecture** with clear separation of concerns across multiple layers:

```
ServicesProject/
├── src/
│   ├── ServicesSystem.Domain/          # Core business entities and interfaces
│   ├── ServicesSystem.Application/     # Business logic and services
│   ├── ServicesSystem.Shared/          # DTOs and common models
│   ├── ServicesSystem.Infrastructure/  # Data access and external services
│   └── ServicesSystem.API/             # REST API endpoints
└── ServicesSystem.sln
```

## ✨ Features

### 1. **User Management & Authentication**
- JWT-based authentication with refresh tokens
- Role-based access control (Admin, Customer, Technician)
- Secure password hashing
- User registration and login

### 2. **Service Management**
- Service catalog with categories
- Multilingual support (English/Arabic)
- Service pricing and duration estimation
- Active/inactive service status

### 3. **Request Management**
- Service request creation and tracking
- Multiple request statuses (Pending, Accepted, InProgress, Completed, Cancelled, Rejected)
- Emergency request support with priority handling
- Technician assignment
- Customer location tracking

### 4. **Real-Time Tracking**
- GPS-based technician location tracking
- Request status updates with notes
- Tracking history for each request

### 5. **Payment System**
- Multiple payment method support
- Payment gateway integration ready
- Payment status tracking (Pending, Completed, Failed, Refunded)
- Transaction management

### 6. **Review & Rating System**
- Customer reviews for completed services
- 1-5 star rating system
- Public/private review visibility
- Technician performance metrics

### 7. **Dashboard & Analytics**
- **Admin Dashboard**: System-wide statistics, revenue, user counts
- **Technician Dashboard**: Assigned requests, earnings, ratings
- **Customer Dashboard**: Request history, spending, reviews

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Architecture Pattern**: Clean Architecture
- **Design Patterns**: Repository Pattern, Unit of Work
- **API Style**: RESTful

## 📋 Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/moaaz10esmail-cloud/ServiceSystem.git
cd ServiceSystem
```

### 2. Update Connection String
Edit `src/ServicesSystem.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ServicesSystemDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Apply Database Migrations
```bash
cd src/ServicesSystem.API
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run
```

The API will be available at `https://localhost:7xxx` (check console output for exact port).

## 📚 API Documentation

Once running, access Swagger UI at: `https://localhost:7xxx/swagger`

### Main Endpoints

#### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/change-password` - Change password

#### Services
- `GET /api/services` - Get all services
- `GET /api/services/{id}` - Get service by ID
- `GET /api/services/category/{categoryId}` - Get services by category
- `POST /api/services` - Create new service
- `DELETE /api/services/{id}` - Delete service

#### Requests
- `GET /api/requests` - Get all requests
- `GET /api/requests/{id}` - Get request by ID
- `GET /api/requests/customer/{customerId}` - Get customer requests
- `GET /api/requests/technician/{technicianId}` - Get technician requests
- `POST /api/requests` - Create new request
- `PUT /api/requests/{id}/status` - Update request status
- `PUT /api/requests/{id}/assign/{technicianId}` - Assign technician

#### Tracking
- `GET /api/tracking/request/{requestId}` - Get tracking history
- `GET /api/tracking/request/{requestId}/latest` - Get latest tracking
- `POST /api/tracking/request/{requestId}` - Add tracking update

#### Payments
- `POST /api/payments` - Create payment
- `GET /api/payments/{id}` - Get payment by ID
- `GET /api/payments/request/{requestId}` - Get payment by request
- `GET /api/payments/customer/{customerId}` - Get customer payments
- `PUT /api/payments/{id}/status` - Update payment status

#### Reviews
- `POST /api/reviews` - Create review
- `GET /api/reviews/{id}` - Get review by ID
- `GET /api/reviews/request/{requestId}` - Get review by request
- `GET /api/reviews/technician/{technicianId}` - Get technician reviews
- `GET /api/reviews/customer/{customerId}` - Get customer reviews
- `PUT /api/reviews/{id}` - Update review
- `DELETE /api/reviews/{id}` - Delete review

#### Dashboard
- `GET /api/dashboard/admin` - Get admin dashboard
- `GET /api/dashboard/technician/{technicianId}` - Get technician dashboard
- `GET /api/dashboard/customer/{customerId}` - Get customer dashboard

## 🗄️ Database Schema

### Core Entities
- **User**: System users (Admin, Customer, Technician)
- **Service**: Available services
- **ServiceCategory**: Service categorization
- **ServiceRequest**: Customer service requests
- **RequestTracking**: Real-time request tracking
- **Payment**: Payment transactions
- **Review**: Customer reviews and ratings
- **RefreshToken**: JWT refresh tokens
- **Notification**: User notifications
- **Wallet**: User wallet management
- **WalletTransaction**: Wallet transaction history

## 🔐 Security

- JWT-based authentication
- Password hashing using secure algorithms
- Role-based authorization
- Refresh token rotation
- HTTPS enforcement (production)

## 🧪 Testing

```bash
# Build the solution
dotnet build

# Run tests (if available)
dotnet test
```

## 📦 Project Structure

### Domain Layer
Contains core business entities, value objects, enums, and repository interfaces. No dependencies on other layers.

### Application Layer
Contains business logic, DTOs, and service interfaces. Depends only on Domain layer.

### Shared Layer
Contains DTOs, constants, and common utilities shared across layers.

### Infrastructure Layer
Implements data access, external services, and infrastructure concerns. Depends on Domain and Application layers.

### API Layer
REST API controllers and configuration. Depends on all other layers.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👨‍💻 Author

**Moaaz Esmail**
- GitHub: [@moaaz10esmail-cloud](https://github.com/moaaz10esmail-cloud)

## 🙏 Acknowledgments

- Built with ASP.NET Core 8
- Clean Architecture principles by Robert C. Martin
- Entity Framework Core for data access
