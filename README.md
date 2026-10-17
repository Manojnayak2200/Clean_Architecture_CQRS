# Prompt: Explain My .NET Microservices Project From Actual Code to Complete Business Flow
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



Act as a Senior .NET Microservices Architect and my personal technical mentor.

I have an Employee Management System built using:

- ASP.NET Core Web API
- .NET Clean Architecture
- Microservices Architecture
- Ocelot API Gateway
- JWT Authentication and Authorization
- CQRS and MediatR
- Entity Framework Core
- MySQL
- FluentValidation
- Serilog
- Global Exception Middleware
- Swagger
- Local file storage

My services run on these ports:

- API Gateway: 7000
- Auth Service: 7001
- Employee Service: 7002
- Document Service: 7003
- Leave Service: 7004

Each service has its own database and follows Clean Architecture.

**My main problem:** I cannot understand the actual code execution flow or how all the modules and their use cases work together. Do not give me only definitions or general architecture explanations. Teach me through real business scenarios and actual code execution.

## Part 1: Explain the Complete Business Application

First, explain what this Employee Management System does in a real company.

Describe the responsibilities of:
1. Admin
2. HR or Manager
3. Employee

Explain all the important use cases, including:

- User registration and login
- JWT token generation
- Creating an employee
- Linking an employee record with a login account
- Viewing employee details
- Updating employee details
- Uploading employee documents
- Downloading documents
- Applying for leave
- Checking leave balance
- Viewing leave history
- Approving or rejecting leave
- Updating leave balance after approval
- Viewing employee records based on user roles

For each use case, explain:
- Who performs the action?
- Why is the action required?
- What data does the user enter?
- Which API is called?
- Which microservice handles the request?
- Which database tables are read or updated?
- What business rules are checked?
- What response does the user receive?
- What happens if the operation fails?

If any feature is not implemented in my actual source code, identify it as a proposed feature.

## Part 2: Teach Me the Actual Code Flow

This is the most important section.

For every use case, trace the complete request from beginning to end.

Use this exact structure:

**Step 1 — Client**

Show the Postman request, URL, HTTP method, headers, and JSON body.

**Step 2 — API Gateway**

Explain which Ocelot route matches the request, how the downstream URL is selected, and whether JWT authentication is configured for that route.

**Step 3 — Destination Microservice**

Explain how the request reaches the correct service and port.

**Step 4 — Middleware**

Explain what authentication, authorization, exception handling, logging, and other relevant middleware do, and in what order they execute in my actual application.

**Step 5 — Controller**

Show the actual controller method and explain every important line.

**Step 6 — MediatR**

Explain what happens when the controller calls:

`await _mediator.Send(command, cancellationToken);`

Explain how MediatR finds the correct handler and passes the request to it.

**Step 7 — Validation**

Show where FluentValidation executes, if configured. Explain what happens when validation fails.

**Step 8 — Handler**

Explain the handler line by line. Show where business rules are applied and where database operations begin.

**Step 9 — Infrastructure**

Explain how the handler uses an injected interface, repository, or DbContext and how the implementation is resolved through dependency injection.

**Step 10 — Entity Framework Core**

Show how the C# operation is translated into a database operation. Explain when a query is executed, when changes are tracked, and when `SaveChangesAsync()` sends changes to the database.

**Step 11 — MySQL or File Storage**

Show what data is inserted, selected, updated, or deleted. For document operations, explain how the file and its metadata are handled.

**Step 12 — Response**

Trace the response from the handler to the controller, back through the gateway, and finally to the client.

For every step, show:
- The filename
- The method or class name
- The relevant code
- What the code receives
- What the code does
- What it returns
- Which component executes next

Do not skip intermediate steps.

If you cannot see my source code, first ask me to upload the relevant files. Do not invent my actual method names, routes, database tables, or code.

## Part 3: Use One Complete Example First

Start with this example:

An employee logs in, receives a JWT, and then applies for leave.

Trace these requests separately.

### Request A: Login

Show the flow:

Client → API Gateway → Auth Controller → Authentication logic → User database → JWT generation → Response.

Explain how the password is verified and how the token is generated.

### Request B: Apply for Leave

Show the flow:

Client with JWT → API Gateway → Leave Controller → MediatR → ApplyLeaveCommandHandler → Leave balance validation → EF Core → MySQL → Response.

Use a simple example request:

`POST /leaves/ApplyLeave`

Example JSON:

```json
{
  "employeeId": 101,
  "leaveType": "Casual",
  "startDate": "2026-11-02",
  "endDate": "2026-11-03",
  "reason": "Personal work"
}
```

Treat this as an illustrative example and verify the real route and request model against my code.

Explain exactly how the employee ID is associated with the authenticated user, whether the employee can apply for leave for another employee, how leave balance is checked, and how duplicate or overlapping requests are prevented if these rules exist.

Also explain the difference between:
- A valid JWT
- An authenticated user
- An authorized user
- A request that passes business validation

## Part 4: Explain Every Module's Complete Use Case

Explain each module independently and then show how it connects with other modules.

### Auth Service
Registration, login, password verification, JWT generation, user roles, and token validation.

### Employee Service
Employee creation, profile retrieval, employee updates, account linking, and role-based access.

### Document Service
PDF upload, file validation, filename generation, metadata storage, download, deletion, and ownership checks.

### Leave Service
Leave application, leave balance checks, leave history, manager approval, rejection, and balance updates.

For each module, provide:

1. Business purpose
2. Real-world example
3. API endpoint
4. Request JSON
5. Response JSON
6. Controller code flow
7. Command or Query
8. Handler code flow
9. Database operation
10. Success scenario
11. Failure scenario
12. Authorization rules
13. Connection with other modules

Then explain how the complete employee lifecycle works across these modules.

Example: HR creates an employee → the employee gets an account → the employee logs in → uploads documents → applies for leave → manager approves the leave → the employee checks the updated leave balance.

Explain which actions require communication between services and which can happen independently.

## Part 5: Explain How Files Connect to Each Other

Help me understand the relationship between files in Clean Architecture.

For example, trace:

`LeaveController.cs`
→ `ApplyLeaveCommand.cs`
→ `ApplyLeaveCommandHandler.cs`
→ `ILeaveDbContext` or repository interface
→ `LeaveDbContext.cs`
→ MySQL.

Use the actual file names and paths from my solution.

Explain:
- Why each file exists
- Which file calls or references it
- How dependency injection connects interfaces to implementations
- How MediatR connects requests to handlers
- How DTOs differ from entities
- Where business rules belong
- Where SQL/database access belongs
- Why the controller should not contain all the business logic

Do not assume that every project uses a repository pattern or an interface for DbContext. Explain the pattern actually implemented in my source code.

## Part 6: Explain the Architecture Diagram Through Execution

Explain this request flow visually:

```text
Postman / React / Mobile
          |
          v
API Gateway :7000
          |
          v
Leave Service :7004
          |
          v
LeaveController
          |
          v
MediatR
          |
          v
ApplyLeaveCommandHandler
          |
          v
Validation and Business Rules
          |
          v
EF Core / Repository
          |
          v
MySQL LeaveDB
          |
          v
Handler Result
          |
          v
Controller HTTP Response
          |
          v
API Gateway
          |
          v
Client
```

For every arrow, explain what is passed between the two components and how the next component is selected.

Then explain the error flow for invalid JWT, validation failure, insufficient leave balance, database failure, and unavailable downstream services.

## Part 7: Teach Me With a File-by-File Learning Plan

Do not explain all files at once.

Follow this order:

1. Solution structure and project references
2. API Gateway and `ocelot.json`
3. Auth Service login and JWT generation
4. Employee Service create and retrieve operations
5. Document Service upload and download
6. Leave Service application and approval
7. Database schema and relationships
8. Cross-service communication
9. Middleware, logging, security, and exception handling
10. Complete end-to-end business workflow

At each stage:

- Explain the actual code.
- Show the flow in a simple diagram.
- Explain every important line.
- Give a practical example.
- Explain how to test the flow in Postman.
- Ask me three short questions to check my understanding.
- Wait for my response before moving to the next stage.

**Begin with Stage 1, but inspect my actual solution files before making claims about their implementation.**
