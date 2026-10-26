#.NET Microservices Project From Actual Code to Complete Business Flow
┌────────────────────────────────────────────────────────────────────────┐
│                        CLIENT (Postman / React / Mobile)               │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ All requests hit ONE port (7000)
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                        API GATEWAY (Ocelot) :7000                      │
│                                                                        │
│  • Validates JWT Signature at the Edge                                 │
│  • Routes /auth/*       ➔ Port 7001                                    │
│  • Routes /employees/*  ➔ Port 7002                                    │
│  • Routes /documents/*  ➔ Port 7003                                    │
│  • Routes /leaves/*     ➔ Port 7004                                    │
└─────────┬──────────────────┬──────────────────┬──────────────────┬─────┘
          │                  │                  │                  │
          ▼                  ▼                  ▼                  ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│  AUTH SERVICE   │ │ EMPLOYEE SERVICE│ │ DOCUMENT SERVICE│ │  LEAVE SERVICE  │
│    (:7001)      │ │    (:7002)      │ │    (:7003)      │ │    (:7004)      │
├─────────────────┤ ├─────────────────┤ ├─────────────────┤ ├─────────────────┤
│ • Login         │ │ • Create Emp    │ │ • Upload PDF    │ │ • Apply Leave   │
│ • Register      │ │ • Get Profile   │ │ • Download PDF  │ │ • Approve Leave │
│ • Issue JWT     │ │ • Link User     │ │ • Delete PDF    │ │ • Check Balance │
└────────┬────────┘ └────────┬────────┘ └────────┬────────┘ └────────┬────────┘
         │                   │                   │                   │
         ▼                   ▼                   ▼                   ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│    AuthDB       │ │   EmployeeDB    │ │   DocumentDB    │ │    LeaveDB      │
│    (MySQL)      │ │    (MySQL)      │ │    (MySQL)      │ │    (MySQL)      │
└─────────────────┘ └─────────────────┘ └────────┬────────┘ └─────────────────┘
                                                 │
                                                 ▼
                                        ┌─────────────────┐
                                        │  FILE STORAGE   │
                                        │ (C:\Uploads\)   │
                                        └─────────────────┘


# Setup Instructions

## Prerequisites
- .NET 8 SDK
- MySQL Server (running on port 3306)
- Visual Studio 2022

## Step 1: Create Databases
Open MySQL Workbench and run:
CREATE DATABASE IF NOT EXISTS AuthDB;
CREATE DATABASE IF NOT EXISTS EmployeeDB;
CREATE DATABASE IF NOT EXISTS DocumentDB;
CREATE DATABASE IF NOT EXISTS LeaveDB;

## Step 2: Update Connection Strings
Check the `appsettings.json` in each API project. Update the password to match your local MySQL password:
"Password=YOUR_LOCAL_PASSWORD;"

## Step 3: Run Migrations
Open the Package Manager Console in Visual Studio.
Set "Default project" to `AuthService.Infrastructure` and run:
`Update-Database -StartupProject AuthService.API`

Repeat for Employee, Document, and Leave. (Or run `dotnet ef database update` in the terminal for each).

## Step 4: Create Initial Admin User
Once the services are running, use Postman to call:
POST https://localhost:7000/auth/register
Body: { "username": "admin", "password": "Admin@123", "role": "Admin" }

## Step 5: Run
Press F5. All services will start.
