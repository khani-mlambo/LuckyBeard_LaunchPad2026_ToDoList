# LuckyBeard Launchpad 2026 – Todo API

This repository contains a production-ready ASP.NET Core 8 Web API developed for the LuckyBeard Launchpad 2026 program. The API provides user authentication, JWT-based authorization, Todo management, and bulk data upload functionality. The solution follows clean architecture practices with a service-based domain layer, SQL Server database integration, and full Swagger documentation to support testing.

---

## Table of Contents

1. Technologies Used
2. Steps to Get a Local Environment Running
3. Environment Variables
4. Running the Application
5. API Documentation
6. SQL Dump
7. Repository Link

---

## 1. Technologies Used

* ASP.NET Core 8 Web API
* Entity Framework Core
* SQL Server LocalDB
* BCrypt.Net
* JWT Bearer Authentication
* Swagger / Swashbuckle
* .NET Dependency Injection

---

## 2. Steps to Get a Local Environment Running

### Step 1: Install Prerequisites

Ensure the following are installed:

* .NET 8 SDK
* Visual Studio 2022 or Visual Studio Code

### Step 2: Clone the Repository

```bash
git clone https://github.com/<your-username>/LuckyBeardAPI_Launchpad2026.git
cd LuckyBeardAPI_Launchpad2026
```

### Step 3: Create Environment File

Create a .env file with the contents of: .en.example
Update .env with a secure JWT key.

### Step 4: Set Up the Database

complete a migration and update the database if necessary

### Step 5: Run the Application

```bash
dotnet run
```

The API will start at:

```
https://localhost:7008
```

---

## 3. Environment Variables

A `.env.example` file is included to assist developers with the required environment configuration.

```
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://localhost:7008

ConnectionStrings__DefaultConnection=Server=(localdb)\\mssqllocaldb;Database=luckybeardApiDb;Trusted_Connection=True;TrustServerCertificate=True

Jwt__Key=CHANGE_ME_TO_A_LONG_RANDOM_SECRET_KEY
Jwt__Issuer=https://localhost:7008/
Jwt__Audience=https://localhost:7008/
Jwt__TokenVialidityMins=30
```

---

## 4. Running the Application

The application can be launched through Visual Studio or via the command line:

```bash
dotnet run
```

Swagger UI for API exploration is available at:

```
https://localhost:7008/swagger
```

---

## 5. API Documentation

Swagger offers full interactive documentation, including:

* Endpoint definitions
* Request and response bodies
* Authentication flow
* Error responses

The Swagger JSON specification is available at:

```
https://localhost:7008/swagger/v1/swagger.json
```
---

## 6. SQL Dump

The SQL schema and data dump required to restore the project database can be found at:

```
/db/luckybeardApiDb.sql
```

---

## 7. Repository Link

Your public GitHub repository should be included here:

```
https://github.com/<your-username>/LuckyBeardAPI_Launchpad2026
```


