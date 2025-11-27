# AnkiSync Project Summary

## Project Overview

AnkiSync is a Windows service that monitors directories for file changes, parses Obsidian-formatted flashcards from markdown files, and automatically syncs them with Anki using the AnkiConnect API. The system supports intelligent deck inference based on Obsidian tags, eliminating manual deck management.

## Core Capabilities

### File Monitoring & Parsing
- **Directory Monitoring**: Windows service continuously monitors configured directories for file changes
- **Obsidian Format Support**: Parses all Obsidian Spaced Repetition formats:
  - Single-line flashcards: `Question::Answer`
  - Multi-line flashcards: `Question?\nAnswer`
  - Bidirectional flashcards: `Term:::Definition`
  - Cloze deletions: `Text with ==hidden== content`
- **Incremental Sync**: Only processes changed files to minimize resource usage

### Anki Integration
- **AnkiConnect API**: Full integration with Anki's HTTP JSON-RPC interface
- **Deck Management**: Create, update, and organize Anki decks programmatically
- **Note Operations**: Add, update, and delete flashcards with proper field mapping
- **Conflict Resolution**: Handle duplicate cards and update existing content

### Intelligent Deck Inference
- **Zero Configuration**: Automatically uses first tag found in each file
- **Automatic Nested Decks**: `#algorithms/datastructures` → `"Algorithms::Datastructures"`
- **Simple Logic**: First tag wins, no complex priority systems

## Architecture

### System Components
- **File Monitor**: Windows FileSystemWatcher for real-time file change detection
- **Parser Engine**: Obsidian format parser with comprehensive flashcard extraction
- **Sync Engine**: AnkiConnect integration with state management
- **Configuration System**: YAML-based settings for directories, mappings, and behavior
- **State Database**: SQLite for tracking file states and flashcard-to-note mappings

### Data Flow
1. File change detected → Parser extracts flashcards → Deck inference applied
2. Sync engine compares with Anki state → Updates made via AnkiConnect API
3. State database updated → Process repeats for next change

## Current Status

### Completed Planning Phase ✅
- **System Architecture**: Complete design with all components specified
- **Detailed Requirements**: All functional and non-functional requirements documented
- **Development Roadmap**: Phased implementation plan with milestones
- **Deck Inference Design**: Comprehensive specification for tag-based deck mapping

### Prototype Code Available ✅
- **AnkiConnect Interface**: Working Python implementation for Anki integration
- **Obsidian Parser**: Complete parser supporting all flashcard formats
- **Basic Sync Logic**: Foundation for the sync engine

### Ready for Implementation 🚀
All planning documents are complete and approved. The project is ready to begin Phase 1 implementation including:
- Windows service framework setup
- File monitoring system
- State management database
- Configuration system
- Integration of deck inference with sync engine

## Key Differentiators

### User Experience
- **Zero Configuration**: Works out-of-the-box with sensible defaults
- **Natural Organization**: Uses existing Obsidian tagging system
- **Background Operation**: Runs as Windows service with no UI required
- **Incremental Updates**: Only syncs what's changed

### Technical Excellence
- **Robust Parsing**: Handles complex Obsidian formats reliably
- **Performance Optimized**: Efficient file monitoring and incremental sync
- **Error Resilient**: Graceful handling of network issues and malformed content
- **Observable**: Comprehensive logging and monitoring capabilities

## Success Metrics

- **Reliability**: 99.9% uptime for file monitoring
- **Accuracy**: >95% correct deck inference from tags
- **Performance**: <100ms response to file changes
- **User Adoption**: Seamless integration with existing Obsidian workflows

## Next Steps

With planning complete, the project is ready to enter implementation phase:

1. **Phase 1**: Core Windows service with file monitoring
2. **Phase 2**: State management and sync engine
3. **Phase 3**: Deck inference integration
4. **Phase 4**: Advanced features and optimization

The foundation is solid, requirements are clear, and the prototype validates the core concepts. AnkiSync is positioned to become the premier solution for automated Obsidian-to-Anki synchronization.