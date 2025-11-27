# AnkiSync Project Summary

## Project Vision

AnkiSync is a Windows service that automatically synchronizes flashcards from text files to Anki, enabling users to maintain their flashcard collections in their preferred text editors using Obsidian Spaced Repetition syntax.

## Current Status

### ✅ Completed Components
- **AnkiConnect Interface**: Full API wrapper for Anki communication
- **Obsidian Parser**: Comprehensive parser for all Spaced Repetition flashcard formats
- **Project Structure**: Organized codebase with prototyping and documentation
- **Requirements Analysis**: Detailed system architecture and requirements documentation

### 🔄 Next Steps
- **File System Monitoring**: Implement Windows service with directory watching
- **State Management**: SQLite database for tracking sync state
- **Sync Engine**: Core synchronization logic with conflict resolution
- **Configuration System**: YAML-based configuration management

## System Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   File Monitor  │───▶│  Flashcard      │───▶│   Sync Engine   │
│   (Windows      │    │  Parser         │    │   (State Mgmt)  │
│    Service)     │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Configuration  │    │   State DB      │    │   AnkiConnect   │
│   Management    │    │   (SQLite)      │    │     API         │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Key Features

### Flashcard Format Support
- ✅ Single-line basic: `Question::Answer`
- ✅ Single-line bidirectional: `Question:::Answer`
- ✅ Multi-line basic: `Question?\nAnswer`
- ✅ Multi-line bidirectional: `Question??\nAnswer`
- ✅ Cloze deletions: `Text with ==deletions==`

### Core Capabilities
- 🔄 **File Monitoring**: Watch directories for changes
- 📝 **Parsing**: Extract flashcards from text files
- 🏷️ **Deck Inference**: Automatically determine decks from Obsidian tags
- 🔄 **Sync**: Bidirectional synchronization with Anki
- ⚙️ **Configuration**: Flexible YAML-based settings
- 📊 **State Tracking**: Incremental sync with change detection
- 🔧 **Conflict Resolution**: Handle file vs Anki modifications

## Technical Stack

- **Language**: Python 3.8+ (pyinstaller for Windows service)
- **Database**: SQLite for state management
- **Service Framework**: Windows Service API
- **File Monitoring**: Windows FileSystemWatcher
- **Anki Integration**: AnkiConnect HTTP API
- **Configuration**: YAML/JSON
- **Logging**: Structured logging with Windows Event Log

## Development Phases

### Phase 1: Foundation (Current → 4 weeks)
**Goal**: Deliver working MVP with basic sync functionality
- [x] AnkiConnect interface
- [x] Flashcard parser
- [ ] Windows service framework
- [ ] Single directory monitoring
- [ ] Basic one-way sync
- [ ] Simple configuration

### Phase 2: Enhanced Sync (4-10 weeks)
**Goal**: Production-ready bidirectional synchronization
- [ ] Multiple directory support
- [ ] Bidirectional sync with conflict resolution
- [ ] Advanced state management
- [ ] Performance optimization
- [ ] Comprehensive error handling

### Phase 3: Production Polish (10-14 weeks)
**Goal**: Enterprise-grade reliability and usability
- [ ] MSI installer
- [ ] Advanced monitoring and logging
- [ ] Comprehensive testing
- [ ] User documentation
- [ ] Beta testing and feedback

## Success Metrics

### MVP (Phase 1)
- Service installs and runs as Windows service
- Monitors directory for .md file changes
- Parses flashcard formats accurately
- Syncs to Anki within 10 seconds
- Handles service restart gracefully

### Full Release (Phase 3)
- Supports multiple directories and file types
- Bidirectional sync with conflict resolution
- <5 second sync latency
- 99.9% uptime
- Comprehensive user documentation

## User Personas & Use Cases

### Primary Users
1. **Technical Writers**: Keep flashcards in documentation
2. **Language Learners**: Vocabulary in text files
3. **Students**: Integrated study materials
4. **Researchers**: Note-taking workflow integration

### Example Workflow
1. User writes flashcards in Obsidian/VS Code with tags like `#programming` or `#spanish`
2. Saves file with flashcard syntax
3. AnkiSync detects change within 5 seconds
4. **Automatically infers deck from tags** (e.g., `#programming` → "Programming" deck)
5. Parses and syncs flashcards to appropriate Anki deck
6. User reviews cards in Anki normally

## Competitive Advantages

- **Native Integration**: Works with existing text editors
- **Format Flexibility**: Supports multiple flashcard syntaxes
- **Performance**: Fast, reliable synchronization
- **User Control**: Local operation, no data sharing
- **Extensibility**: Plugin architecture for custom formats

## Risk Assessment

### High Risk
- **AnkiConnect API Changes**: Mitigated by version checking
- **File System Monitoring**: Extensive testing planned
- **Service Stability**: Comprehensive error handling

### Medium Risk
- **Performance Scaling**: Early optimization focus
- **User Adoption**: Technical user focus
- **Configuration Complexity**: Extensive documentation

### Low Risk
- **Technology Stack**: Mature, stable technologies
- **Market Competition**: Unique value proposition
- **Monetization**: Open source core with potential premium features

## Resource Requirements

- **Development**: 1-3 developers over 14 weeks
- **Budget**: ~$285K development + $60K annual operations
- **Timeline**: 14 weeks to production release
- **Team**: Python developers with Windows service experience

## Next Immediate Steps

1. **Implement Windows Service Framework** (Week 1)
   - Service skeleton with proper lifecycle
   - Installation/uninstallation scripts
   - Basic logging framework

2. **File System Monitoring** (Week 2)
   - FileSystemWatcher implementation
   - Change detection and debouncing
   - File filtering and validation

3. **State Management Database** (Week 3)
   - SQLite schema design
   - File tracking and mapping
   - Incremental sync logic

4. **Core Sync Engine** (Week 4)
   - Integration of parser and Anki interface
   - Basic one-way synchronization
   - Error handling and retry logic

## Documentation Index

- **[System Architecture](docs/system_architecture.md)**: Technical design and components
- **[Detailed Requirements](docs/detailed_requirements.md)**: Functional and non-functional requirements
- **[Development Roadmap](docs/development_roadmap.md)**: Phased implementation plan
- **[API Reference](docs/api_reference.md)**: AnkiConnect API documentation
- **[README](../README.md)**: Project overview and setup

## Conclusion

AnkiSync represents a unique solution for users who want to maintain flashcards in their text-based workflows while benefiting from Anki's spaced repetition algorithm. With a solid foundation in place and clear development roadmap, the project is well-positioned for successful implementation and user adoption.

The combination of comprehensive flashcard format support, robust Windows service architecture, and focus on user experience creates a compelling product that addresses a real need in the spaced repetition community.