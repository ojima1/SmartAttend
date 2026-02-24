# SmartAttend 🕒

A professional hybrid attendance system built with **.NET 10** and **React**. It uses AI-driven biometrics and geofencing to track office and remote work cycles.

## 🏗️ The Architecture
Built using **Clean Architecture** and **Domain-Driven Design (DDD)**.
- **Domain**: Core logic and business rules.
- **Application**: Use cases and commands (CQRS).
- **Infrastructure**: Database (EF Core) and AI integrations (Gemini).
- **API**: Secure entry point with JWT authentication.

## ⚖️ Business Rules
- **Role-Based Deadlines**: Interns must check in by **08:00 WAT**, Staff by **09:00 WAT**.
- **Hybrid Work**: Supports 3 office days and 2 remote days per week.
- **Geofencing**: Office check-ins require being within 200m of coordinates.
- **Security**: Automatic VPN detection and AI-powered liveness checks.

## 🛠️ Tech Stack
- **Backend**: .NET 10 & SQL Server.
- **AI**: Google Gemini 1.5 Flash (Face matching & Liveness).
- **Frontend**: React + TypeScript.
- **Testing**: xUnit for domain and integration tests.