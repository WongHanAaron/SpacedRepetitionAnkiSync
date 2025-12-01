# AnkiSync Cards and Decks Documentation

## Tag Parsing Rules

### Tag Location Rules
- **File Restriction**: Tags are only read from the **first 2 lines** of each markdown file
- **Line Position**: Tags must start with `#` as the **first character** on their line (after trimming whitespace)
- **Multiple Tags**: Multiple tags can exist on the same line, separated by spaces

### Tag Format Rules
All tags are continuous alphanumeric characters, hyphens, slashes and underscores
- **Valid Format**: 
  - `#tagname` where tagname contains only alphanumeric characters, hyphens, slashes and underscores
  - `#tag/name` where tagname contains only alphanumeric characters, hyphens, slashes and underscores
- **Invalid Formats**:
  - `# test` (space after #) - rejected
  - `##tag` (multiple #) - rejected
  - `#tag with spaces` - rejected (spaces not allowed in tag names)
  - `#tag@` - tags with any invalid characters will be ignored

### Nested Tag Rules
- **Hierarchy Support**: Tags support nested hierarchies using `/` separator
- **Example**: `#aws/compute/ec2` creates nested tags: `["aws", "compute", "ec2"]`
- **Splitting**: Each `/` creates a separate tag in the hierarchy
- **Empty Parts**: Empty parts between `/` are filtered out

### Tag Processing Examples
Only keep the first tag found
```markdown
#aws/compute/ec2
#database
```

Results in tags: `["aws", "compute", "ec2"]`

```markdown
#cloud #aws/compute
#database/sql
```

Results in tags: `["cloud", "aws", "compute"]`

## Card Parsing Rules

### Supported Card Formats

#### 1. Obsidian Format
- **Question-Answer**: `Question:::Answer`
- **Cloze Deletion**: `{{c1::text}}` format with incremental cloze numbers
- **Highlights**: `==highlighted text==`
- **Bold**: `**bold text**`
- **Curly Braces**: `{{curly text}}`

#### 2. Basic Q&A Format
- **Question Line**: `Q: What is the capital of France?`
- **Answer Line**: `A: Paris`

#### 3. Multi-line Format
- **Question Block**: Any text before separator
- **Separator**: `?` (forward) or `??` (reversed)
- **Answer Block**: Any text after separator

### Card Content Rules

#### Multi-line Cards
- **Separator Required**: Must have `?` or `??` separator to create cards
- **Content Filtering**: Lines starting with `#` are excluded from card content
- **Flashcard Patterns**: Cards are only created if content contains `::`, `Q:`, or `A:` patterns
- **Exception**: Separator-based cards (`?`/`??`) don't require flashcard patterns

#### Single-line Cards
- **Obsidian Format**: `Question:::Answer` creates Q&A cards
- **Cloze Format**: Text with `{{c1::answer}}` creates cloze cards
- **Basic Format**: `Q:` and `A:` lines create Q&A cards

### Card Type Detection

#### Question-Answer Cards
- Created from: `Question:::Answer` format
- Created from: `Q:` and `A:` format
- Created from: Multi-line with `?` separator

#### Cloze Cards
- Created from: Text containing `{{c1::answer}}` patterns
- Created from: `==highlighted text==`
- Created from: `**bold text**`
- Created from: `{{curly text}}`

#### Reversed Cards
- Created when using `??` separator in multi-line format
- Creates both forward and reverse versions of the card

## Synchronization Rules

### Source Discovery
- **Directory Scanning**: Recursively scans provided directories for `.md` files
- **File Processing**: Each markdown file becomes a potential deck source
- **Tag-based Grouping**: Files with the same primary tag are grouped into decks

### Deck Creation Rules
- **Primary Tag**: First tag found in file becomes the deck identifier
- **Deck Naming**: Deck names follow Anki naming conventions
- **Empty Decks**: Files with no valid tags are skipped
- **Multiple Decks**: Single file can contribute to multiple decks if it has multiple tags

### Card Synchronization

#### Addition Rules
- **New Cards**: Cards not existing in Anki are added
- **Content Changes**: Cards with updated content are modified
- **Date-based Updates**: Only cards newer than existing Anki cards are updated

#### Deletion Rules
- **Obsolete Cards**: Cards removed from source files are deleted from Anki
- **Deck Cleanup**: Entire decks removed from source are deleted from Anki
- **Safe Deletion**: Only deletes cards that match exactly with source content

### Conflict Resolution
- **Source Priority**: Source files always take precedence over Anki content
- **Merge Strategy**: Existing Anki cards are preserved unless they conflict with source
- **Duplicate Handling**: Duplicate cards in source are deduplicated before sync

## Anki Upload Rules

### Deck Management
- **Auto-Creation**: Decks are created in Anki if they don't exist
- **Naming Convention**: Deck names use hierarchical format (e.g., "Parent::Child")
- **Empty Deck Handling**: Empty decks in source result in empty decks in Anki

### Card Upload Process
- **Batch Processing**: Cards are processed in batches for efficiency
- **Duplicate Prevention**: Existing cards are updated, not duplicated
- **Type Preservation**: Card types (Q&A, Cloze) are maintained in Anki

### AnkiWeb Synchronization
- **Post-Sync Trigger**: AnkiWeb sync occurs automatically after all local changes
- **Error Handling**: AnkiWeb sync failures don't prevent local synchronization
- **Cross-Device Sync**: Ensures changes are propagated to all Anki clients

### Error Handling
- **Network Resilience**: Handles temporary Anki connection issues
- **Partial Failures**: Continues processing other decks/cards if one fails
- **Logging**: Comprehensive logging of all sync operations and errors

## File Monitoring Rules

### Change Detection
- **File System Watcher**: Monitors for file creation, modification, deletion, and renaming
- **Extension Filter**: Only monitors `.md` files
- **Recursive Monitoring**: Watches subdirectories recursively

### Sync Triggers
- **Immediate Sync**: Triggers sync on any file change
- **Periodic Sync**: Automatic sync every 5 minutes
- **Debounced Sync**: Prevents multiple rapid syncs from file system events

### Performance Optimization
- **Incremental Updates**: Only processes changed files
- **Conflict Prevention**: Prevents concurrent sync operations
- **Resource Cleanup**: Proper cleanup of file watchers and timers

## Configuration and Customization

### Default Behaviors
- **Tag Parsing**: First 2 lines, `#` as first character
- **Card Types**: All supported formats enabled by default
- **Sync Frequency**: File changes + 5-minute intervals
- **Error Handling**: Continue on individual failures

### Extension Points
- **Custom Parsers**: Interface allows for custom card format parsers
- **Custom Sync Logic**: Extensible synchronization engine
- **Custom Anki Integration**: Pluggable Anki communication adapters

This documentation covers the complete ruleset for AnkiSync's parsing, synchronization, and upload functionality.</content>
<parameter name="filePath">d:\Development\AnkiSync\docs\cards_decks.md