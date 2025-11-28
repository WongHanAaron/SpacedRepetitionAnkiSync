# Phase 2 Tasks: File Parsing Implementation

## Overview
Phase 2 focuses on implementing file parsing for Obsidian Spaced Repetition syntax. The parsing functionality will be implemented in an adapter layer following Clean Architecture principles, with the domain defining the port interface.

## New Projects and Files

### AnkiSync.Domain (Existing Project)
- **File**: `src/AnkiSync.Domain/Interfaces/ICardSourceRepository.cs`
  - **Responsibility**: Defines the port interface for retrieving flashcards from external sources (files). Provides methods to get cards from specified file paths or directories, and raises events when cards might be updated.

### AnkiSync.Adapter.SpacedRepetitionNotes (New Project)
- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/AnkiSync.Adapter.SpacedRepetitionNotes.csproj`
  - **Responsibility**: Project file for the Spaced Repetition Notes adapter, containing all parsing-related implementations.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/SpacedRepetitionNotesServiceCollectionExtensions.cs`
  - **Responsibility**: Extension methods for registering the adapter services in the dependency injection container.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/SpacedRepetitionNotesRepository.cs`
  - **Responsibility**: Main adapter class implementing `ICardSourceRepository`. Orchestrates file discovery, parsing, and card extraction from Obsidian files.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/FileParser.cs`
  - **Responsibility**: Handles reading and basic parsing of individual markdown files, extracting metadata (filepath, modification time, tags) and raw content into a Document record.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/CardExtractor.cs`
  - **Responsibility**: Processes Document content to identify and extract flashcard data using Obsidian Spaced Repetition syntax patterns, returning ParsedCard objects.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/DeckInferencer.cs`
  - **Responsibility**: Analyzes file paths, tags, and folder structures to automatically infer deck hierarchies and assign cards to appropriate decks.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/Models/Document.cs`
  - **Responsibility**: Data model representing metadata and content of a parsed document, including filepath, modification time, tags, and raw content.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/Models/Tag.cs`
  - **Responsibility**: Data model representing a tag with nested tag support containing a list of strings for hierarchical tags.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/Models/ParsedCard.cs`
  - **Responsibility**: Data model representing a parsed flashcard with front/back content, tags, and metadata before domain conversion.

- **File**: `src/AnkiSync.Adapter.SpacedRepetitionNotes/Models/ParsedDeck.cs`
  - **Responsibility**: Data model representing a parsed deck with hierarchical path information and associated cards.

### Tests
- **File**: `tests/AnkiSync.Adapter.SpacedRepetitionNotes.Tests/AnkiSync.Adapter.SpacedRepetitionNotes.Tests.csproj`
  - **Responsibility**: Test project for the Spaced Repetition Notes adapter.

- **File**: `tests/AnkiSync.Adapter.SpacedRepetitionNotes.Tests/SpacedRepetitionNotesRepositoryTests.cs`
  - **Responsibility**: Unit and integration tests for the repository implementation.

- **File**: `tests/AnkiSync.Adapter.SpacedRepetitionNotes.Tests/FileParserTests.cs`
  - **Responsibility**: Tests for file parsing logic with various markdown formats, verifying metadata extraction and content parsing.

- **File**: `tests/AnkiSync.Adapter.SpacedRepetitionNotes.Tests/CardExtractorTests.cs`
  - **Responsibility**: Tests for card extraction from FileMetadata content using different Obsidian syntax patterns.

- **File**: `tests/AnkiSync.Adapter.SpacedRepetitionNotes.Tests/DeckInferencerTests.cs`
  - **Responsibility**: Tests for deck inference from file paths, tags, and folder structures.