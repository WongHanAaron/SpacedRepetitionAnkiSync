# AnkiSync

A comprehensive .NET library for synchronizing flashcards with Anki instances using Clean Architecture principles.

## Project Overview

AnkiSync provides a robust, type-safe interface for programmatically interacting with Anki Desktop through the AnkiConnect add-on. The project follows Clean Architecture patterns with clear separation of concerns across domain, application, and infrastructure layers.

## Architecture

This project follows Clean Architecture (Hexagonal Architecture) principles:

```
AnkiSync/
├── src/
│   ├── AnkiSync.Domain.Core/     # Domain Layer - Core business logic and entities
│   ├── AnkiSync.Application/     # Application Layer - Use cases and port interfaces
│   ├── AnkiSync.Adapter.AnkiConnect/ # Infrastructure Layer - AnkiConnect adapter
│   └── AnkiSync.Presentation.Console/ # Presentation Layer - Console application
├── tests/
│   ├── AnkiSync.Domain.Core.Tests/     # Domain unit tests
│   ├── AnkiSync.Adapter.AnkiConnect.Tests/ # Adapter unit tests
│   └── AnkiSync.IntegrationTests/      # Integration tests
├── prototyping/          # Experimental Python prototypes
├── docs/                # Documentation and planning
└── README.md           # This file
```

### Layers

- **Domain Layer** (`AnkiSync.Domain.Core`): Contains business entities, domain services, and port interfaces that define what the application needs from external systems.

- **Application Layer** (`AnkiSync.Application`): Contains application services (use cases) and port interfaces that adapters must implement. This layer orchestrates domain objects and defines the application's interface.

- **Infrastructure Layer** (`AnkiSync.Adapter.AnkiConnect`): Contains adapters that implement the port interfaces defined in the application layer. Currently empty, ready for your custom AnkiConnect implementation.

- **Presentation Layer** (`AnkiSync.Presentation.Console`): Contains the console application that uses the application services.

## Quick Start

### Prerequisites

1. **Anki Desktop** must be installed and running
2. **AnkiConnect Add-on** must be installed in Anki
   - Install from: https://ankiweb.net/shared/info/2055492159
   - Or in Anki: Tools → Add-ons → Get Add-ons → Code: `2055492159`

### Building and Running

1. Build the solution:
   ```bash
   dotnet build
   ```

2. Run the console application:
   ```bash
   dotnet run --project src/AnkiSync.Presentation.Console
   ```

3. Run tests:
   ```bash
   dotnet test
   ```

## Current Features (Prototype)

The prototype demonstrates core AnkiConnect functionality:

- ✅ **Connection Testing**: Verify connectivity to Anki
- ✅ **Deck Management**: List and analyze all decks
- ✅ **Deck Statistics**: Get new/learning/review card counts
- ✅ **Note Search**: Find notes using Anki's query syntax
- ✅ **Note CRUD**: Create, read, and update flashcards
- ✅ **Sync Operations**: Trigger synchronization with AnkiWeb

## Development Status

### Phase 1: Foundation (Current)
- [x] Basic AnkiConnect connectivity
- [x] Core CRUD operations
- [x] Error handling framework
- [x] Documentation structure
- [ ] Unit test coverage
- [ ] Integration tests

### Upcoming Phases
- **Phase 2**: Sync Engine (bidirectional synchronization)
- **Phase 3**: Data Models (type-safe entities)
- **Phase 4**: Advanced Features (CLI, plugins, configuration)
- **Phase 5**: Production Release (packaging, documentation)

See [`docs/development_plan.md`](docs/development_plan.md) for detailed roadmap.

## API Usage Examples

```python
from anki_sync import AnkiConnector

# Initialize
anki = AnkiConnector()

# Check connection
anki.check_connection()

# Get all decks
decks = anki.get_deck_names()

# Add a card
note_id = anki.add_note(
    deck_name="Default",
    front="What is Python?",
    back="A programming language",
    tags=["programming"]
)

# Find notes
note_ids = anki.find_notes("deck:Default tag:programming")

# Get note details
notes = anki.get_note_info(note_ids)

# Sync with AnkiWeb
anki.sync()
```

## Documentation

- **[Architecture Overview](docs/architecture.md)**: System design and component relationships
- **[Development Plan](docs/development_plan.md)**: Roadmap and milestones
- **[API Reference](docs/api_reference.md)**: Complete method documentation and examples

## Configuration

Default AnkiConnect URL: `http://localhost:8765`

To use a different URL:
```python
anki = AnkiConnector(url="http://custom-host:8765")
```

## Contributing

This project follows a phased development approach. See the development plan for current priorities and contribution guidelines.

## Security

AnkiConnect allows local applications to control Anki. By default, it only accepts connections from localhost. For remote connections, configure AnkiConnect settings in Anki.

## License

[MIT License](LICENSE) - See LICENSE file for details.
