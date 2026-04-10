# FRIDGE APP – BACKEND CONTEXT (ASP.NET CORE)

## 🎯 PROJECT GOAL

This project is an ASP.NET Core Web API for managing fridge contents.

Users can:
- Create fridges
- Add items into fridges
- Track quantities and expiration dates

This backend will be consumed by a mobile app (Flutter or React Native).

---

## 🧱 CURRENT ARCHITECTURE

Layers:
- Controllers (API endpoints)
- Entities (database models)
- Models (request DTOs)
- Data (DbContext)

Currently:
- Controllers directly use DbContext
- No service layer yet

---

## 🗄️ DATABASE STRUCTURE

### Users
- Id
- Email
- PasswordHash
- CreatedDate

### Fridges
- Id
- UserId
- Name

### Items
- Id
- FridgeId
- Name
- Quantity
- Unit
- ExpirationDate
- AddedDate

---

## 🔗 RELATIONSHIPS

- 1 User → N Fridges
- 1 Fridge → N Items

---

## ⚙️ TECH STACK

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger

---

## ⚠️ IMPORTANT CURRENT LIMITATION

- UserId and FridgeId are sent manually in requests
- Authentication is NOT implemented yet

---

## 🚀 CURRENT GOAL (VERY IMPORTANT)

We are now upgrading to a professional backend architecture.

FIRST STEP:
➡️ Implement Service Layer

Requirements:
- Add Interfaces folder
- Add Services folder
- Create IFridgeService and FridgeService
- Move business logic from Controller to Service
- Use Dependency Injection

---

## 📌 CODING RULES

- Follow clean architecture principles
- Keep it SIMPLE (no over-engineering)
- Avoid unnecessary abstractions
- Write readable and maintainable code
- Use async/await properly
- Do NOT break existing functionality

---

## ❗ IMPORTANT INSTRUCTION FOR AI

Before writing code:
1. Analyze the current project structure
2. Follow existing naming conventions
3. Do not rewrite the entire project
4. Make incremental changes only

---

## 🎯 NEXT STEPS (ORDER)

1. FridgeService implementation
2. Controller refactor
3. ItemService
4. UserService
5. JWT Authentication
6. Validation
7. Exception handling

---

## ☁️ CLOUD MIGRATION PLAN

We are preparing this project for cloud deployment.

Goals:
1. Move SQL Server database to Azure SQL
2. Update API to use Azure SQL
3. Containerize the API
4. Deploy API to Azure Container Apps

Important:
- Do not break existing functionality
- Make minimal safe changes
- Always explain changes in Turkish
- Focus on step-by-step migration

Current phase:
➡️ Database migration preparation