# AnkiSync - Clean Architecture Implementation

## Overview

AnkiSync follows Clean Architecture (Hexagonal Architecture) principles to ensure maintainability, testability, and separation of concerns. The architecture is organized into four main layers, each with specific responsibilities and dependencies.

## Architecture Layers

### 1. Domain Layer (`AnkiSync.Domain.Core`)

**Purpose**: Contains the core business logic and domain entities that are independent of any external frameworks or technologies.

**Contents**:
- **Entities**: `Flashcard`, `SyncResult`, `SyncStatus`, etc.
- **Domain Services**: Business logic that doesn't naturally fit in entities
- **Port Interfaces**: Define what the application needs from external systems
- **Value Objects**: Immutable objects representing domain concepts
- **Domain Events**: Represent business events within the domain

**Key Characteristics**:
- No dependencies on external frameworks
- Pure .NET Standard library
- Contains business rules and validations
- Defines the ubiquitous language of the domain

### 2. Application Layer (`AnkiSync.Application`)

**Purpose**: Contains application-specific business logic and defines the use cases of the system.

**Contents**:
- **Application Services**: Orchestrate domain objects to fulfill use cases
- **Port Interfaces**: Define contracts for external dependencies
- **DTOs**: Data Transfer Objects for cross-layer communication
- **Commands/Queries**: Represent user intentions and data requests

**Key Characteristics**:
- Depends only on the Domain layer
- Defines what the application can do (use cases)
- Contains no business logic, only orchestration
- Technology-agnostic

### 3. Infrastructure Layer (`AnkiSync.Adapter.*`)

**Purpose**: Contains adapters that connect the application to external systems and frameworks.

**Contents**:
- **Adapters**: Implement port interfaces defined in Application layer
- **External API Clients**: AnkiConnect HTTP client, file system access, etc.
- **Repository Implementations**: Database access, file storage, etc.
- **Framework Integrations**: Logging, configuration, dependency injection

**Key Characteristics**:
- Implements interfaces defined in Application layer
- Contains external system integrations
- Can depend on any technology or framework
- Easy to replace or mock for testing

### 4. Presentation Layer (`AnkiSync.Presentation.*`)

**Purpose**: Contains the user interface and external interfaces of the system.

**Contents**:
- **Console Application**: Command-line interface
- **API Controllers**: REST API endpoints (future)
- **UI Components**: Web interface (future)
- **Configuration**: App settings and startup logic

**Key Characteristics**:
- Depends on Application layer
- Contains no business logic
- Can be easily replaced (console → web → API)
- Handles user input/output

## Dependency Flow

```
┌─────────────────┐
│   Presentation  │ ← Depends on Application
└─────────────────┘
        │
        ▼
┌─────────────────┐
│   Application   │ ← Depends on Domain
└─────────────────┘
        │
        ▼
┌─────────────────┐
│     Domain      │ ← No dependencies
└─────────────────┘
        │
        ▼
┌─────────────────┐
│ Infrastructure  │ ← Depends on Application (implements ports)
└─────────────────┘
```

## Port & Adapter Pattern

The architecture uses the Ports & Adapters pattern (Hexagonal Architecture) where:

- **Ports** are interfaces defined in the Application layer that represent what the application needs
- **Adapters** are implementations in the Infrastructure layer that fulfill those interfaces

### Example Ports

```csharp
// Application Layer - Defines what we need
public interface IAnkiPort
{
    Task<bool> TestConnectionAsync();
    Task<IEnumerable<string>> GetDecksAsync();
    Task<long> AddNoteAsync(AnkiNote note);
}

public interface IFileSystemPort
{
    bool FileExists(string path);
    Task<string> ReadAllTextAsync(string path);
}
```

### Example Adapters

```csharp
// Infrastructure Layer - Implements the ports
public class AnkiConnectAdapter : IAnkiPort
{
    // Implementation using AnkiConnect HTTP API
}

public class SystemFileAdapter : IFileSystemPort
{
    // Implementation using System.IO
}
```

## Benefits

### Maintainability
- Clear separation of concerns
- Easy to modify or replace components
- Changes in one layer don't affect others

### Testability
- Each layer can be tested in isolation
- Easy mocking of external dependencies
- Fast unit tests without external systems

### Flexibility
- Can change UI technology without affecting business logic
- Can switch infrastructure (database, external APIs) easily
- Technology choices are isolated to specific layers

### Scalability
- Layers can be deployed independently
- Easy to add new features without affecting existing code
- Clear boundaries prevent tight coupling

## Project Structure

```
src/
├── AnkiSync.Domain.Core/          # Domain entities and ports
├── AnkiSync.Application/          # Application services and ports
│   ├── Services/                  # Application service interfaces
│   ├── Ports/                     # Port interfaces for adapters
│   └── DTOs/                      # Data transfer objects
├── AnkiSync.Adapter.AnkiConnect/  # AnkiConnect infrastructure adapter
└── AnkiSync.Presentation.Console/ # Console presentation layer

tests/
├── AnkiSync.Domain.Core.Tests/       # Domain unit tests
├── AnkiSync.Application.Tests/       # Application service tests (future)
├── AnkiSync.Adapter.AnkiConnect.Tests/ # Adapter unit tests
└── AnkiSync.IntegrationTests/        # End-to-end integration tests
```

## Development Guidelines

### Adding New Features
1. **Domain First**: Define domain entities and business rules
2. **Application Layer**: Define use cases and required ports
3. **Infrastructure**: Implement adapters for the ports
4. **Presentation**: Add UI to trigger the use cases

### Testing Strategy
- **Unit Tests**: Test domain logic and application services
- **Integration Tests**: Test adapter implementations
- **End-to-End Tests**: Test complete user workflows

### Dependency Injection
- Use constructor injection for all dependencies
- Register interfaces in the composition root
- Keep the domain layer free of DI concerns

### Code Organization
- **One Class Per File**: Each class must be defined in its own `.cs` file
- **File Naming**: Class files should be named after the class they contain (e.g., `Flashcard.cs` for the `Flashcard` class)
- **Namespace Structure**: Follow the project folder structure for namespaces
- **Partial Classes**: Use partial classes only when necessary (e.g., for generated code)</content>
<parameter name="filePath">d:\Development\AnkiSync\docs\clean_architecture.md