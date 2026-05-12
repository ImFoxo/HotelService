# HotelServiceAPI

## Table of contents
- [General Description](https://github.com/ImFoxo/HotelService#general-description)
- [Key Features](https://github.com/ImFoxo/HotelService#key-features)
- [Technology Stack](https://github.com/ImFoxo/HotelService#technology-stack)
- [Testing Environment](https://github.com/ImFoxo/HotelService#testing-environment)
- [Additional Technical Solutions](https://github.com/ImFoxo/HotelService#additional-technical-solutions)
- [Getting Started](https://github.com/ImFoxo/HotelService#getting-started)
- [Using accounts](https://github.com/ImFoxo/HotelService#using-accounts)
- [Running Tests](https://github.com/ImFoxo/HotelService#running-tests)

## General Description
A backend system for managing hotel resource reservations (rooms, conference halls) built with ASP.NET Core 8. The project emphasizes robust architecture and data integrity.

## Key Features
**Booking System:** Safe reservation system with data validation. Supports multi-resource bookings in a single transaction.

**Advanced Data Modeling:** Implementation of Table-per-Type (TPT) inheritance. Each entity (BookableItem, Resource, Seat) has its own dedicated table, ensuring a clean and normalized database schema.

**Security (Identity):** Role-based access control (RBAC) using ASP.NET Core Identity and JWT (Admin/User roles).

**Transactional Integrity:** Critical database operations are protected with ACID transactions.

**Automatic Seeding:** Data initialization on startup, including users, roles, and default hotel resources.

## Technology Stack
**Framework:** ASP.NET Core Web API

**Database:** SQL Server (Production/Development via Docker), SQLite In-Memory (Testing)

**ORM:** Entity Framework Core

**Security:** Microsoft Identity, JWT Authentication

**Testing:** xUnit, FluentAssertions, WebApplicationFactory

## Testing Environment
**SQLite In-Memory:** Utilizes a full SQL engine in RAM instead of the basic InMemoryDatabase, enabling validation of transactions and foreign key constraints.

**State Isolation:** Each test case starts with a freshly cleared and re-seeded database, ensuring total test independence.

**Custom WebApplicationFactory:** A specialized test factory managing a Singleton SQLite connection.

## Additional Technical Solutions
**LINQ Polymorphism:** Leveraging .OfType<T>() for precise filtering and cast specific resource types within polymorphic collections.

**Clean Code & DTOs:** Strict separation of concerns using Data Transfer Objects to decouple the API layer from the database schema.

**Token Generation:** Hand-coded JWT generation with claim mapping.

## Getting Started
**Prerequisites**
- .NET 8 SDK

- Docker Desktop

**Installation & Execution**
1. Clone the repository and enter directory:
```
git clone https://github.com/ImFoxo/HotelService.git
cd HotelService
```

2. Launch the Database (Docker):
The project uses SQL Server running in a Docker container. Run the following command to start the database service:
```
docker-compose up -d 
```

3. Run the Application:
You don't need to manually apply migrations. The application is configured to automatically migrate the database schema on startup.
```
dotnet run --project HotelServiceAPI --launch-profile https
```

4. Explore the API:
Once the application starts, you can access the Swagger UI at:
```
https://localhost:7075/swagger/index.html
```

## Using accounts
### Standard log in
1. To log in utilize *account/login* endpoint. </br>
To do that use one of the following automatically generated accounts or register your own.
- **Login:** admin@admin.com **Password:** admin123
- **Login:** user1@user.com **Password:** user123

2. Then copy given token, press *Authorize* at the top of the page and paste the token, finally press *Authorize* again.
<p align="center">
  <img width="902" height="230" alt="image" src="https://github.com/user-attachments/assets/2e6a56cc-873f-469a-904e-ae8384292f62" />
  <img width="416" height="186" alt="image" src="https://github.com/user-attachments/assets/05f89965-9c41-44f3-9cf4-fe0cd6acd841" />
</p>

3. Enjoy service as a logged in user:
<p align="center">
  <img width="894" height="133" alt="image" src="https://github.com/user-attachments/assets/08eed257-a622-4f26-9dc6-fface0cdc9f9" />
  <img width="898" height="79" alt="image" src="https://github.com/user-attachments/assets/e0bbbef5-bae9-437c-bd9d-bc3c26b6fd64" />
</p>
</br>

### Google Auth
1. To use Google authentication first acquire your valid googleId and googleSecret, then go to *\HotelServiceAPI* and add them to project using following commands:
```
dotnet user-secrets init
dotnet user-secrets set "Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Google:ClientSecret" "your-google-client-secret"
```

2. While the application is running, go to the link below (don't use Swagger) and choose your account:
```
https://localhost:7075/Account/google-login
```

3. After you obtain your token, paste it as you would in standard log in procedure.
<p align="center">
  <img width="902" height="230" alt="image" src="https://github.com/user-attachments/assets/2e6a56cc-873f-469a-904e-ae8384292f62" />
  <img width="416" height="186" alt="image" src="https://github.com/user-attachments/assets/05f89965-9c41-44f3-9cf4-fe0cd6acd841" />
</p>

## Running Tests
To execute unit tests just run:
```
dotnet test
```
