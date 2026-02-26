SmartAttend 🕒
A professional hybrid attendance system built with .NET 10 and Blazor WebAssembly. It leverages AI-driven biometrics and geofencing to manage modern office and remote work cycles with high integrity.

🏗️ The Architecture
Built using Clean Architecture and Domain-Driven Design (DDD) principles to ensure scalability and maintainability.

Domain: Pure business logic, Entities, and Value Objects.

Application: Use cases, CQRS pattern (MediatR), and FluentValidation.

Infrastructure: SQL Server (EF Core), JWT Authentication, and Gemini AI integration.

Client (Blazor WASM): A rich, interactive frontend sharing core logic with the backend.

⚖️ Business Rules
Role-Based Deadlines: Dynamic check-in windows (Interns: 08:00 WAT, Staff: 09:00 WAT).

Hybrid Work Model: Intelligent tracking for 3 office days and 2 remote days per week.

Geofencing: Office check-ins validated against a 200m radius of GPS coordinates.

Security: RBAC (Role-Based Access Control), VPN detection, and AI-powered liveness checks.

🛠️ Tech Stack
Framework: .NET 10 (C#)

Database: SQL Server + Entity Framework Core

AI: Google Gemini 1.5 Flash / Nano (Biometric face matching)

Frontend: Blazor WebAssembly

Testing: xUnit + Moq + FluentAssertions (7/7 Core Tests Passing)

🚦 Current Status
[x] Domain & Application Layer Implementation

[x] 100% Test Coverage for Core Use Cases

[ ] Infrastructure & SQL Server Persistence (In Progress)

[ ] Gemini AI Biometric Integration(In Progress)

[ ] Blazor UI Development(in Progress)