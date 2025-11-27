# Deck Inference Design Specification

## Overview

AnkiSync automatically determines the appropriate Anki deck for flashcards based on the **first Obsidian tag** found in each file. The system converts tag hierarchies directly to deck hierarchies without requiring any configuration.

## Problem Statement

Users currently need to manually specify which Anki deck their flashcards should go to, either through configuration or file paths. This creates friction in the workflow and requires users to think about Anki's organizational structure rather than their natural knowledge organization.

## Solution

Implement simple deck inference that:
1. Extracts tags from Obsidian markdown files
2. Uses the first tag found to determine the deck
3. Converts tag hierarchy directly to deck hierarchy
4. Ignores cards that don't have tags

## Functional Requirements

### Tag Extraction
- Parse Obsidian-style tags: `#tag`, `#parent/child`, `#tag-with-dashes`
- Extract tags from anywhere in the file (not just flashcards)
- Support inline tags within flashcard text

### Deck Mapping Rules
- **Simple hierarchy conversion**: `#algorithms/datastructures` → `"Algorithms::Datastructures"`
- **First tag wins**: Use the first tag found in the file
- **No configuration required**: Automatic conversion from tag to deck name

### Inference Priority
1. **Explicit flashcard tags**: Tags within flashcard text take highest priority
2. **File-level tags**: Tags found anywhere in the file
3. **No fallback**: Cards without tags are ignored

### Deck Creation
- Automatic deck creation when inferred deck doesn't exist
- Support for hierarchical deck creation (`Parent::Child`)
- Validation of deck names and hierarchy

## Configuration

**No configuration required** - The system automatically converts tags to deck names using simple rules.

## Algorithm Design

### Tag Extraction Process

Extract all Obsidian tags from file content in order found, preserving the order of first occurrence. Tags follow the pattern `#tag` or `#parent/child` format.

### Deck Inference Algorithm

1. **Check flashcard content first**: Look for tags within the flashcard text itself
2. **Combine with file tags**: Add any tags found at the file level
3. **Use first tag found**: Take the first tag from the combined list
4. **Convert to deck name**: Transform tag hierarchy to deck hierarchy
5. **Skip if no tags**: Ignore flashcards that have no tags

### Tag to Deck Conversion

Convert any tag to a deck name by:
- Removing the `#` prefix
- Splitting on `/` separators for hierarchy
- Capitalizing each part
- Joining with `::` for Anki's deck hierarchy

## User Experience

### How It Works

The system automatically converts the first tag found into a deck name:

- `#programming` → `"Programming"`
- `#algorithms/datastructures` → `"Algorithms::Datastructures"`
- `#science/biology/cells` → `"Science::Biology::Cells"`

**No configuration needed** - just add tags to your Obsidian files!

### File Examples

**Simple Tag Conversion:**
```markdown
# Programming Notes

#python

What is a closure?::A function that captures variables from its lexical scope
What is recursion?::A programming technique where a function calls itself
```

**Inferred Decks:**
- Both flashcards → "Python" (from #python tag)

**Nested Tag Hierarchy:**
```markdown
# Data Structures Study Guide

#algorithms/datastructures

What is a hash table?::A data structure that maps keys to values
What is Big O notation?::A mathematical notation describing algorithm complexity
```

**Inferred Decks:**
- Both flashcards → "Algorithms::Datastructures" (from #algorithms/datastructures tag)

**Deep Hierarchy:**
```markdown
# Cell Biology Notes

#science/biology/cells/mitosis

What is mitosis?::The process of cell division
What is cytokinesis?::The division of the cytoplasm
```

**Inferred Decks:**
- Both flashcards → "Science::Biology::Cells::Mitosis" (from #science/biology/cells/mitosis tag)

**First Tag Wins:**
```markdown
# Mixed Topics

#programming #algorithms/datastructures #web-development

What is a variable?::A storage location with a name
What is Big O notation?::Algorithm complexity analysis
```

**Inferred Decks:**
- Both flashcards → "Programming" (first tag #programming wins)

## Implementation Considerations

### Performance
- Tag extraction should be fast (<100ms per file)
- Simple string processing with no configuration lookups
- Batch deck creation operations

### Error Handling
- Invalid tag formats → Log warning, skip invalid tags
- Deck creation failures → Skip flashcard with error logging
- Missing tags → Skip flashcards silently

## Testing Strategy

### Unit Tests
- Tag extraction from various file formats
- First tag selection logic
- Tag to deck name conversion (simple and nested)
- Edge cases (no tags, invalid tags, deep nesting)

### Integration Tests
- End-to-end deck inference workflow
- Multiple files with different tag patterns
- Deck creation and hierarchy handling
- Tag order preservation

### User Acceptance Tests
- Real Obsidian vaults with complex tag structures
- Performance with large numbers of files
- Error handling with malformed tags

## Future Enhancements

### Advanced Features
- **Dynamic Deck Creation**: Create decks based on tag patterns
- **Tag Hierarchies**: Infer parent decks from child tags
- **Context-Aware Inference**: Use surrounding content for better inference
- **Machine Learning**: Learn user preferences over time

### Integration Features
- **Obsidian Plugin**: Direct integration with Obsidian
- **Tag Synchronization**: Sync tags between Obsidian and Anki
- **Bulk Operations**: Reorganize decks based on tag changes
- **Analytics**: Show deck usage statistics by tag

## Success Metrics

- **Simplicity**: Zero configuration required for basic usage
- **Accuracy**: >95% of flashcards go to correct decks automatically
- **Performance**: <10ms overhead per file for tag processing
- **User Satisfaction**: Eliminates manual deck management entirely
- **Adoption**: Used by 80% of active users within 3 months

## Migration Strategy

### Simplicity First
- No configuration needed - works out of the box
- Just add tags to Obsidian files
- Automatic deck creation and hierarchy

### Future Extensions
- Optional configuration for custom deck names
- Tag filtering and exclusion rules
- Advanced hierarchy customization

This design provides a simple, reliable system for automatic deck inference that works out of the box with zero configuration.