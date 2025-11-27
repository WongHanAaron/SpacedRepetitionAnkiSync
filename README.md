# AnkiSync

A comprehensive Python library for synchronizing with Anki instances using AnkiConnect.

## Project Overview

AnkiSync provides a robust, type-safe interface for programmatically interacting with Anki Desktop through the AnkiConnect add-on. The project is structured into phases, starting with a working prototype and evolving toward a production-ready synchronization engine.

## Project Structure

```
AnkiSync/
├── prototyping/          # Experimental code and prototypes
│   ├── anki_sync.py     # Main AnkiConnect interface
│   └── requirements.txt # Python dependencies
├── docs/                # Documentation and planning
│   ├── architecture.md  # System architecture overview
│   ├── development_plan.md # Development roadmap
│   └── api_reference.md # API documentation
├── src/                 # Production source code (future)
├── tests/               # Test suites (future)
└── README.md           # This file
```

## Quick Start

### Prerequisites

1. **Anki Desktop** must be installed and running
2. **AnkiConnect Add-on** must be installed in Anki
   - Install from: https://ankiweb.net/shared/info/2055492159
   - Or in Anki: Tools → Add-ons → Get Add-ons → Code: `2055492159`

### Running the Prototype

1. Navigate to the prototyping directory:
   ```bash
   cd prototyping
   ```

2. Install dependencies (if any):
   ```bash
   pip install -r requirements.txt
   ```

3. Run the demo:
   ```bash
   python anki_sync.py
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
