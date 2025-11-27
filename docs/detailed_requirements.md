# AnkiSync Windows Service - Detailed Requirements

## Product Overview

AnkiSync is a Windows service that automatically synchronizes flashcards from text files to Anki. Users can write flashcards in their preferred text editors using Obsidian Spaced Repetition syntax, and the service automatically keeps Anki updated.

## User Stories

### Primary Users
1. **Technical Writer**: "I want to keep my flashcards in my note-taking system alongside my other notes"
2. **Language Learner**: "I want my vocabulary lists in text files to automatically sync to Anki"
3. **Student**: "I want flashcards integrated into my study materials without manual copying"

### User Journeys

#### Basic Usage
1. User installs AnkiSync service
2. Configures directories to monitor
3. Writes flashcards in text files using Obsidian syntax
4. Service automatically detects changes and syncs to Anki
5. User reviews cards in Anki as normal

#### Advanced Usage
1. User organizes flashcards in folder structure
2. Service maps folders to Anki decks
3. User modifies cards in either file or Anki
4. Service handles conflicts according to user preferences
5. User monitors sync status through logs

## Functional Requirements

### FR-001: File System Monitoring
**Priority**: Critical
**Description**: Monitor specified directories for file changes
**Requirements**:
- Support multiple directories with recursive monitoring
- Detect file creation, modification, deletion, and renaming
- Filter by configurable file extensions
- Handle file locking during writes (debounce changes)
- Support network drives and UNC paths

### FR-002: Flashcard Format Support
**Priority**: Critical
**Description**: Parse Obsidian Spaced Repetition flashcard formats
**Requirements**:
- Single-line basic: `Question::Answer`
- Single-line bidirectional: `Question:::Answer`
- Multi-line basic: `Question?\nAnswer`
- Multi-line bidirectional: `Question??\nAnswer`
- Cloze deletions: `Text with ==deletions==`
- Custom cloze patterns support
- Tag extraction from file content
- **Deck inference from Obsidian tags**
- Deck inference from file paths

### FR-003: Deck Inference and Management
**Priority**: High
**Description**: Automatically determine appropriate Anki decks for flashcards
**Requirements**:
- **Simple tag-based deck inference**: Use first tag found to determine deck
  - `#programming` → "Programming" deck
  - `#algorithms/datastructures` → "Algorithms::Datastructures" deck
- No configuration required - automatic conversion from tag hierarchy
- Automatic deck creation when needed
- Cards without tags are ignored

### FR-003: Anki Integration
**Priority**: Critical
**Description**: Synchronize flashcards with Anki via AnkiConnect
**Requirements**:
- Create new notes in appropriate decks
- Update existing notes when flashcards change
- Delete notes when flashcards are removed
- Preserve Anki review history
- Handle deck creation and management
- Support all Anki note types

### FR-004: State Management
**Priority**: High
**Description**: Track synchronization state for incremental updates
**Requirements**:
- Store file modification timestamps
- Map flashcards to Anki note IDs
- Detect changes since last sync
- Handle file renames and moves
- Support partial sync recovery

### FR-005: Conflict Resolution
**Priority**: High
**Description**: Handle conflicts between file and Anki changes
**Requirements**:
- Detect when both file and Anki have been modified
- Support configurable conflict resolution policies:
  - File wins (overwrite Anki)
  - Anki wins (ignore file changes)
  - Manual resolution (log conflict)
- Preserve user modifications appropriately

### FR-006: Configuration Management
**Priority**: High
**Description**: Allow users to configure service behavior
**Requirements**:
- YAML/JSON configuration file
- Directory and file type settings
- Anki connection and deck mapping
- Sync behavior preferences
- Performance tuning options
- Configuration validation and error reporting

### FR-007: Windows Service Integration
**Priority**: High
**Description**: Run as a proper Windows service
**Requirements**:
- Service installation and uninstallation
- Automatic startup with Windows
- Service control (start/stop/restart)
- Proper service account permissions
- Event logging integration

### FR-008: Logging and Monitoring
**Priority**: Medium
**Description**: Provide comprehensive logging and status monitoring
**Requirements**:
- Structured logging with multiple levels
- File and event log output
- Performance metrics logging
- Error tracking and reporting
- Service health monitoring

## Non-Functional Requirements

### Performance Requirements

#### PR-001: Sync Latency
- **Target**: File changes detected and synced within 5 seconds
- **Measurement**: Time from file save to Anki update
- **Constraints**: Network latency, AnkiConnect response time

#### PR-002: Resource Usage
- **Memory**: <100MB during normal operation
- **CPU**: <5% average during sync operations
- **Disk**: <10MB for state database and logs
- **Network**: Minimal traffic to localhost only

#### PR-003: Scalability
- **Files**: Support 1000+ files with flashcards
- **Cards**: Handle 10000+ flashcards efficiently
- **Batch Size**: Configurable batch processing (default 50 cards)

### Reliability Requirements

#### RR-001: Availability
- **Uptime**: 99.9% for file monitoring
- **MTBF**: No crashes during normal operation
- **Recovery**: Automatic restart on failure

#### RR-002: Data Integrity
- **Consistency**: No data loss during sync operations
- **Atomicity**: All-or-nothing batch operations
- **Backup**: State database backup before major operations

#### RR-003: Error Handling
- **Graceful Degradation**: Continue monitoring when Anki unavailable
- **Retry Logic**: Exponential backoff for transient failures
- **Error Reporting**: Clear error messages with actionable information

### Usability Requirements

#### UR-001: Installation
- **Ease**: One-click installation process
- **Prerequisites**: Clear system requirements
- **Uninstallation**: Clean removal of all components

#### UR-002: Configuration
- **Discovery**: Obvious configuration file location
- **Validation**: Real-time configuration validation
- **Documentation**: Comprehensive configuration examples

#### UR-003: Monitoring
- **Status**: Service status visible in Windows Services
- **Logs**: Human-readable log files
- **Troubleshooting**: Clear error messages and solutions

### Security Requirements

#### SR-001: Local Operation
- **Scope**: All operations local to user machine
- **Data**: No transmission of user data to external servers
- **Access**: Service runs under user account permissions

#### SR-002: Configuration Security
- **Storage**: Configuration files in secure locations
- **Permissions**: Proper file permissions on config files
- **Validation**: Input validation to prevent injection attacks

## Interface Requirements

### Configuration File Interface
```yaml
directories:
  - path: "C:\\Users\\User\\Notes"
    recursive: true
    extensions: [".md", ".txt"]

anki:
  base_url: "http://localhost:8765"
  default_deck: "Default"
  # Deck hierarchy automatically inferred from tag nesting
  # e.g., #algorithms/datastructures → "Algorithms::Datastructures"

sync:
  batch_size: 50
  debounce_ms: 500
  conflict_resolution: "file_wins"
```

### Service Control Interface
- Windows Services MMC snap-in
- Command-line service control
- PowerShell cmdlets (optional)

### Logging Interface
- Structured JSON logs
- Human-readable text logs
- Windows Event Log integration

## Implementation Phases

### Phase 1: Core Service (MVP)
- File monitoring for single directory
- Basic flashcard parsing
- One-way sync to Anki
- Simple configuration
- Windows service framework

### Phase 2: Advanced Features
- Multiple directories
- Bidirectional sync
- Conflict resolution
- State management
- Performance optimization

### Phase 3: Production Polish
- Comprehensive error handling
- Advanced configuration
- Monitoring and logging
- Documentation and testing

## Testing Requirements

### Unit Testing
- Parser accuracy for all flashcard formats
- State management correctness
- Configuration validation
- Error handling paths

### Integration Testing
- End-to-end sync scenarios
- File system event handling
- AnkiConnect API interactions
- Service lifecycle testing

### Performance Testing
- Large file set processing
- High-frequency file changes
- Memory and CPU usage profiling
- Network latency simulation

### User Acceptance Testing
- Real-world usage scenarios
- Configuration edge cases
- Error recovery validation
- Performance validation

## Acceptance Criteria

### MVP Success Criteria
- [ ] Service installs and runs as Windows service
- [ ] Monitors single directory for .md files
- [ ] Parses basic flashcard formats correctly
- [ ] Syncs flashcards to Anki within 10 seconds
- [ ] Handles service restart gracefully
- [ ] Comprehensive error logging

### Full Release Criteria
- [ ] All functional requirements implemented
- [ ] Performance requirements met
- [ ] Reliability requirements met
- [ ] Comprehensive test coverage
- [ ] User documentation complete
- [ ] Beta testing successful