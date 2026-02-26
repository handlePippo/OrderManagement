<img width="1208" height="416" alt="image" src="https://github.com/user-attachments/assets/bc351c88-e4f5-454d-9c31-536efa4c2f46" />

# 📦 Order Management Microservices (.NET)

Minimal order management system built with a **microservices architecture in .NET**, designed for PhotoSì.

The system supports:

- User authentication and address book
- Product categories and catalog
- Order creation and management
- API Gateway as single entry point

Each domain is implemented as an **independent microservice** with its own DbContext and persistence boundary.  
Database relationships are enforced at the MySQL level while keeping services isolated at code level.

---

## 🏗 Architecture

The solution consists of:

- **Provisioner API** — users, authentication and addresses  
- **Category API** — product categories  
- **Product API** — product catalog  
- **Order API** — order lifecycle and order item snapshotting  
- **Gateway API** — unified REST entry point  

### Key characteristics

- ✔ Service isolation (no project references between services)  
- ✔ Entity Framework Core per service  
- ✔ HTTP communication between services  
- ✔ Snapshot-based order model (product and shipping data preserved over time)  
- ✔ JWT authentication  
- ✔ Docker-based local orchestration  
- ✔ Database-level referential integrity across services

---

## 🧰 Tech stack

- **Language:** C# (.NET 8)
- **API:** ASP.NET Core Web API (REST)
- **Persistence:** Entity Framework Core
- **Database:** MySQL 8
- **Authentication:** JWT Bearer
- **API Documentation:** Swagger / OpenAPI
- **Containerization & Orchestration:** Docker + Docker Compose
- **Testing:** xUnit (unit tests)

> Note: each microservice owns its own DbContext and persistence layer, even though all services share the same MySQL instance.

---

## 🧪 Unit Tests

The solution includes unit tests covering:

- Application services
- Infrastructure repositories
- Factories and normalizers
- Calculation logic
- Validation rules

### ▶ Run tests locally

From the solution root:

```bash
dotnet test
```

## 🐳 Run tests inside Docker

Tests can also be executed inside a container:

```bash
docker build -t photosi-tests -f Dockerfile.tests .
docker run --rm photosi-tests
```

The test container runs dotnet test and exits with the corresponding exit code.

---

## 🔐 Authentication & Authorization

- JWT authentication handled by **Provisioner API**
- Public read access for catalog endpoints
- Protected write operations
- Role-based authorization support (Admin role for privileged operations)

---

## 🐳 Running the project

### Requirements

- Docker
- Docker Compose

### Start the system

```bash
docker compose up --build
```

The system will automatically:

- Start MySQL
- Apply EF migrations
- Create database constraints
- Seed demo data
- Start all microservices
- Start the API Gateway

---

## 🌐 Service endpoints

| Service     | URL                                                            |
| ----------- | -------------------------------------------------------------- |
| Gateway     | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
| Provisioner | [http://localhost:5001/swagger](http://localhost:5001/swagger) |
| Category    | [http://localhost:5002/swagger](http://localhost:5002/swagger) |
| Product     | [http://localhost:5003/swagger](http://localhost:5003/swagger) |
| Order       | [http://localhost:5004/swagger](http://localhost:5004/swagger) |

---

## 👤 Demo credentials

* Admin
email: admin@demo.it
password: admin

* User
email: user@demo.it
password: user

---

## 🧾 How to make an Order

- Authenticate and obtain JWT
- Browse categories
- Browser products of the choosen category
- Create an order with selected items and shipping details
- Update order while status is pending
- Retrieve order details
- Orders store snapshots of product information and shipping data to guarantee historical consistency even if catalog data changes.

---

## 🎯 Project goals

This project demonstrates:

- Microservice boundaries and independence
- Cross-service database integrity
- Realistic order lifecycle handling
- Transaction consistency within a service
- Clean domain modeling
- Containerized development workflow

---

## ⚙️ Technical decisions

### Microservice isolation

Each domain is implemented as an independent microservice with:

- its own DbContext
- no direct project references to other services
- communication exclusively through HTTP

This ensures loose coupling and allows services to evolve independently.

---

### Shared database with bounded contexts

Although all services use the same MySQL instance, each microservice owns its schema and persistence logic.

Cross-service relationships are enforced at the database level using foreign keys defined via initialization scripts.  
This approach guarantees referential integrity while preserving application-level isolation.

---

### Snapshot-based order model

Orders store a snapshot of:

- product name
- product price
- shipping address

This prevents historical inconsistencies when catalog data changes after an order has been created.

The Order service does not rely on real-time product data when retrieving historical orders.

---

### HTTP-based inter-service communication

Services communicate using HTTP clients instead of direct dependencies.

This reflects real-world distributed architectures and enables:

- independent deployment
- resilience strategies
- easier scaling

---

### API Gateway as single entry point

The Gateway aggregates all services and provides:

- centralized routing
- authentication propagation
- simplified client interaction

Clients never invoke microservices directly.

---

### JWT authentication and propagation

Authentication is handled by the Gateway service (JWT validation)

The Gateway forwards the Authorization header to downstream services, allowing each service to validate tokens independently without tight coupling.

---

### Database initialization strategy

The system uses:

- EF Core migrations for schema creation
- a dedicated initialization container for foreign keys and seed data

This guarantees reproducible environments and consistent startup behavior.

---

### Transaction boundaries

Each service maintains transactional consistency within its own boundary.

Distributed transactions are intentionally avoided.  
The Order service ensures internal consistency while relying on external services only during command execution.

---

### Docker-first development

The entire system is designed to run through Docker Compose, providing:

- deterministic local environments
- simplified onboarding
- infrastructure reproducibility

---

## ⚠️ Limitations / Trade-offs

This project intentionally focuses on clarity and microservice isolation rather than production-grade completeness.

- **No distributed transactions**: each service guarantees consistency only within its own boundary. Cross-service operations rely on synchronous HTTP calls and local transactions.
- **Shared database instance**: all services use the same MySQL instance for simplicity. This is a pragmatic choice for the exercise; in a production scenario each service would typically own its own database to maximize autonomy.
- **Database-level relationships across services**: foreign keys are enforced at DB level via init scripts to guarantee referential integrity while keeping services decoupled at code level. This introduces operational coupling to the shared schema.
- **Simplified authorization model**: role assignment (Admin/User) is intentionally minimal and designed for demo purposes, not a complete IAM solution.
- **No async messaging / eventual consistency**: the system uses synchronous REST calls only; no event-driven patterns (Kafka/RabbitMQ) or outbox/inbox mechanisms are implemented.
- **Observability is minimal**: no distributed tracing/centralized logging/metrics stack (OpenTelemetry, Prometheus, etc.) is included.
- **Health checks are basic**: the solution relies on Docker health checks; richer `/health` endpoints could be added per service.