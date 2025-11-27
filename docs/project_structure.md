# AnkiSync Project Structure

## Overview

AnkiSync follows a **hexagonal architecture** (ports & adapters) pattern with clear separation between business logic, infrastructure concerns, and presentation layers. The project structure emphasizes testability, maintainability, and clear architectural boundaries.

## Architecture Layers

### 🏛️ Domain Layer (Business Logic)
Contains the core business rules, entities, and abstract interfaces (ports) that define what the system does, independent of external technologies.

### 🔌 Adapter Layer (Infrastructure)
Contains concrete implementations (adapters) for external concerns like databases, APIs, and communication protocols.

### 🎭 Presentation Layer (User Interfaces)
Contains entry points and user interface implementations that drive the application.

### 🧪 Testing Layer (Quality Assurance)
Contains comprehensive test suites for each architectural layer.

## Folder Structure

```
AnkiSync/
├── .git/
├── .gitignore
├── .vscode/                    # VS Code workspace settings
├── docs/                       # Documentation
├── src/                        # Source code
├── tests/                      # Test projects
├── scripts/                    # Build/deployment scripts
├── prototyping/                # Existing prototype code
├── README.md
└── AnkiSync.sln               # .NET solution file
```

## Source Code Organization (`src/`)

### Domain Layer
```
src/
├── AnkiSync.Domain.Core/            # 🏛️ Business Logic & Domain Models
│   ├── Interfaces/                   # 🏛️ Ports (abstract contracts)
│   │   ├── IAnkiSyncService.cs      # Sync orchestration interface
│   │   ├── IFlashcardRepository.cs  # Data persistence interface
│   │   └── IDeckService.cs          # Deck management interface
│   ├── Models/                       # 🏛️ Domain entities
│   │   ├── Flashcard.cs             # Core flashcard entity
│   │   ├── Deck.cs                  # Deck representation
│   │   └── SyncResult.cs            # Sync operation results
│   ├── Exceptions/                   # 🏛️ Domain-specific exceptions
│   │   ├── AnkiSyncException.cs     # Base sync exception
│   │   └── ValidationException.cs   # Validation errors
│   └── Extensions/                   # 🏛️ Domain utility extensions
```

### Adapter Layer
```
├── AnkiSync.Adapter.AnkiConnect/    # 🔌 AnkiConnect HTTP API Adapter
│   ├── Client/                       # 🔌 HTTP client implementation
│   │   ├── AnkiConnectHttpClient.cs # Low-level HTTP communication
│   │   └── AnkiConnectClient.cs     # High-level API operations
│   ├── Models/                       # 🔌 Anki-specific data models
│   │   ├── AnkiNote.cs              # Anki note structure
│   │   └── AnkiDeck.cs              # Anki deck structure
│   └── Configuration/                # 🔌 Adapter configuration
│       └── AnkiConnectOptions.cs    # Connection settings
├── AnkiSync.Adapter.State/          # 🔌 Persistence Adapter (SQLite)
│   ├── Database/                     # 🔌 Database context & setup
│   │   ├── AnkiSyncDbContext.cs     # EF Core context
│   │   └── SyncStateRepository.cs   # Repository implementation
│   ├── Migrations/                   # 🔌 Database schema migrations
│   └── Repositories/                 # 🔌 Repository interfaces/impls
```

### Presentation Layer
```
└── AnkiSync.Presentation.Console/   # 🎭 Console Application (Phase 3)
    ├── Commands/                     # 🎭 CLI command handlers
    │   ├── SyncCommand.cs           # Sync command implementation
    │   └── StatusCommand.cs         # Status command implementation
    └── Program.cs                    # 🎭 Application entry point
```

## Testing Organization (`tests/`)

```
tests/
├── AnkiSync.Domain.Core.Tests/      # 🧪 Domain logic unit tests
├── AnkiSync.Adapter.AnkiConnect.Tests/ # 🧪 AnkiConnect adapter tests
│   ├── Unit/                         # 🧪 Unit tests (mocked dependencies)
│   └── Integration/                  # 🧪 Integration tests (real Anki)
├── AnkiSync.Adapter.State.Tests/     # 🧪 Persistence adapter tests
└── AnkiSync.IntegrationTests/       # 🧪 End-to-end system tests
```

## Architectural Principles

### Dependency Direction
```
🎭 Presentation → 🏛️ Domain ← 🔌 Adapters
```

- Presentation layer depends on Domain
- Domain defines interfaces (ports) that Adapters implement
- Adapters depend on Domain but Domain doesn't depend on Adapters
- Each layer has dedicated test projects

### Benefits
1. **Testability**: Each adapter can be mocked/replaced for testing
2. **Technology Independence**: Domain logic doesn't depend on external technologies
3. **Maintainability**: Clear separation of concerns
4. **Flexibility**: Easy to swap implementations (e.g., database, API client)

## Naming Conventions

- **`AnkiSync.Domain.*`**: Core business logic and domain models
- **`AnkiSync.Adapter.*`**: Infrastructure implementations
- **`AnkiSync.Presentation.*`**: User interfaces and entry points
- **`*.Tests`**: Test projects following the same naming pattern

## Future Extensions

If Python/gRPC layer is needed:
- Add `AnkiSync.Adapter.Grpc` (.NET gRPC client)
- Add `AnkiSync.Adapter.Python` (Python gRPC server)
- Update communication flow: .NET → gRPC → Python → Anki

## Build & Development

- **Solution**: `AnkiSync.sln` contains all .NET projects
- **Scripts**: `scripts/` contains build, test, and deployment automation
- **Dependencies**: Managed via NuGet packages
- **CI/CD**: GitHub Actions for automated testing and deployment</content>
<parameter name="filePath">d:\Development\AnkiSync\docs\project_structure.md