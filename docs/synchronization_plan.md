# Synchronization Implementation Plan

This document outlines the step-by-step plan to implement the instruction-based synchronization design described in `synchronization.md` using Test-Driven Development (TDD) principles.

## High-Level Changes to Implement

### 1. Domain Layer Changes
- Create synchronization instruction models in `src/AnkiSync.Domain/Models/`
- Add `ICardEqualityChecker` interface and `ExactMatchEqualityChecker` implementation
- Add `IDeckEqualityChecker` interface and implementation for deck matching
- Ensure domain models have required properties for equality checking

### 2. Application Layer Changes
- Implement `AccumulateSynchronizationInstructions` method in `CardSynchronizationService`
- Break down the method into small, single-responsibility methods (50-100 lines each)
- Implement deck matching logic using `IDeckEqualityChecker`
- Implement card matching logic using `ICardEqualityChecker`

### 3. Adapter Layer Changes
- Create `ISynchronizationInstructionExecutor` interface
- Implement `SynchronizationInstructionExecutor` to execute instructions via `IDeckRepository`
- Update `IDeckRepository` to support instruction execution

### 4. Testing Changes
- Update existing unit tests for new synchronization logic
- Create comprehensive unit tests for instruction models and equality checkers
- Create integration tests for end-to-end synchronization scenarios
- Ensure all tests follow TDD principles (Red-Green-Refactor)

### 5. Infrastructure Changes
- Update dependency injection registrations
- Ensure proper separation of concerns between layers
- Maintain existing functionality while adding new features

## TDD Implementation Phases

### Phase 1: Domain Models and Interfaces (Foundation)
**Goal**: Establish the core domain models and interfaces that will be used throughout the synchronization process.

#### 1.1 Synchronization Instructions
**Red**: Write failing tests for synchronization instruction models
- Test `SynchronizationInstruction` base class/interface
- Test concrete instruction types: `CreateDeckInstruction`, `DeleteDeckInstruction`, `CreateCardInstruction`, `UpdateCardInstruction`, `DeleteCardInstruction`, `MoveCardInstruction`, `SyncWithAnkiInstruction`

**Green**: Implement the instruction models
- Create `SynchronizationInstruction.cs` as abstract base class
- Implement all concrete instruction classes with required properties
- Ensure instructions are serializable and comparable

**Refactor**: Improve code structure and remove duplication

#### 1.2 Equality Checkers
**Red**: Write failing tests for equality checker interfaces and implementations
- Test `ICardEqualityChecker` interface
- Test `ExactMatchEqualityChecker` implementation
- Test `IDeckEqualityChecker` interface and implementation

**Green**: Implement equality checkers
- Create `ICardEqualityChecker.cs` interface
- Implement `ExactMatchEqualityChecker.cs` with logic for QuestionAndAnswerCards (question match) and ClozeCards (text + keywords match)
- Create `IDeckEqualityChecker.cs` interface
- Implement deck equality checker (ParentDeck + DeckName match)

**Refactor**: Optimize equality checking logic and improve test coverage

#### 1.3 Domain Model Updates
**Red**: Write tests to verify domain models have required properties
- Test `Deck.cs` has ParentDeck and DeckName properties
- Test `Card.cs` has all properties needed for equality checking

**Green**: Update domain models as needed
- Add any missing properties to `Deck.cs` and `Card.cs`

**Refactor**: Ensure models are properly structured for the new synchronization logic

### Phase 2: Instruction Execution Engine (Core Engine)
**Goal**: Create the mechanism to execute accumulated synchronization instructions.

#### 2.1 Instruction Executor Interface
**Red**: Write failing tests for instruction executor
- Test `ISynchronizationInstructionExecutor` interface
- Test execution of individual instruction types

**Green**: Implement instruction executor
- Create `ISynchronizationInstructionExecutor.cs` interface
- Implement `SynchronizationInstructionExecutor.cs` that delegates to `IDeckRepository`

**Refactor**: Improve error handling and logging

#### 2.2 Repository Updates
**Red**: Write tests for new repository methods needed for instruction execution
- Test `IDeckRepository` can execute instruction lists
- Test individual instruction execution methods

**Green**: Update repository interface and implementation
- Add `ExecuteInstructionsAsync(IEnumerable<SynchronizationInstruction>)` to `IDeckRepository`
- Implement instruction execution in `DeckRepository.cs`

**Refactor**: Ensure repository methods remain focused on data access concerns

### Phase 3: Synchronization Logic Implementation (Business Logic)
**Goal**: Implement the core `AccumulateSynchronizationInstructions` method with all its sub-methods.

#### 3.1 Method Structure
**Red**: Write failing tests for the main synchronization method structure
- Test `AccumulateSynchronizationInstructions` method signature
- Test method accepts source decks and existing Anki decks
- Test method returns list of synchronization instructions

**Green**: Implement method skeleton
- Add `AccumulateSynchronizationInstructions` method to `CardSynchronizationService`
- Implement basic method structure with empty instruction list return

**Refactor**: Improve method organization and documentation

#### 3.2 Deck Deletion Instructions
**Red**: Write tests for deck deletion logic
- Test identification of decks to delete (Anki decks not in source)
- Test creation of `DeleteDeckInstruction`s

**Green**: Implement deck deletion logic
- Add logic to compare source decks with existing Anki decks
- Create deletion instructions for decks not in source

**Refactor**: Optimize deck comparison logic

#### 3.3 New Deck Processing
**Red**: Write tests for new deck creation and card processing
- Test deck matching logic (no matching Anki deck found)
- Test `CreateDeckInstruction` creation
- Test card processing for new decks (create/move instructions)

**Green**: Implement new deck processing
- Add deck matching using `IDeckEqualityChecker`
- Create deck creation instructions
- Implement card processing logic for new decks

**Refactor**: Break down into smaller methods as specified (50-100 lines each)

#### 3.4 Existing Deck Processing
**Red**: Write tests for existing deck updates
- Test deck matching logic (matching Anki deck found)
- Test card deletion instructions (Anki cards not in source)
- Test card creation/update/move instructions

**Green**: Implement existing deck processing
- Add logic to query Anki cards for existing decks
- Create card deletion instructions for missing cards
- Create card creation/update instructions based on matching logic

**Refactor**: Extract card matching and instruction creation into separate methods

#### 3.5 Card Matching Logic
**Red**: Write tests for card matching using equality checkers
- Test `ICardEqualityChecker.AreEqual()` method
- Test different card types (QuestionAndAnswer, Cloze)
- Test edge cases (null cards, different content)

**Green**: Implement card matching
- Integrate `ICardEqualityChecker` into card processing logic
- Handle different card types appropriately

**Refactor**: Improve matching performance and accuracy

#### 3.6 Final Sync Instruction
**Red**: Write test for AnkiConnect sync instruction
- Test that final sync instruction is added to instruction list

**Green**: Add sync instruction
- Append `SyncWithAnkiInstruction` to the instruction list

**Refactor**: Ensure proper instruction ordering

### Phase 4: Application Layer Integration (Integration)
**Goal**: Integrate the new synchronization logic with the existing application entry points.

#### 4.1 Service Integration
**Red**: Write tests for service method updates
- Test `SynchronizeCardsAsync` calls new synchronization method
- Test dependency injection of new interfaces

**Green**: Update service integration
- Modify `SynchronizeCardsAsync` to use `AccumulateSynchronizationInstructions`
- Add dependency injection for `ICardEqualityChecker` and `ISynchronizationInstructionExecutor`

**Refactor**: Ensure clean integration with existing code

#### 4.2 Dependency Injection Updates
**Red**: Write tests for DI container configuration
- Test that new interfaces are properly registered
- Test that implementations can be resolved

**Green**: Update DI registrations
- Add registrations in service collection extension methods
- Ensure proper lifetime management

**Refactor**: Optimize DI configuration

### Phase 5: Comprehensive Testing (Validation)
**Goal**: Ensure all functionality works correctly through comprehensive testing.

#### 5.1 Unit Test Updates
**Red**: Write failing tests for updated methods
- Update existing `CardSynchronizationServiceTests.cs`
- Test new synchronization logic thoroughly

**Green**: Implement updated unit tests
- Cover all new methods and logic paths
- Ensure high test coverage (>90%)

**Refactor**: Improve test readability and maintainability

#### 5.2 Integration Test Updates
**Red**: Write failing integration tests
- Test end-to-end synchronization scenarios
- Test complex operations (deck changes, card moves)

**Green**: Implement integration tests
- Create comprehensive test scenarios
- Validate real AnkiConnect interactions

**Refactor**: Optimize test performance and reliability

#### 5.3 Edge Case Testing
**Red**: Write tests for edge cases
- Test empty deck lists, null values, network failures
- Test concurrent synchronization attempts

**Green**: Implement edge case handling
- Add proper error handling and validation
- Ensure thread safety where needed

**Refactor**: Improve error messages and logging

### Phase 6: Migration and Optimization (Cleanup)
**Goal**: Remove old code and optimize the implementation.

#### 6.1 Legacy Code Removal
**Red**: Write tests to ensure old methods are no longer needed
- Test that old synchronization methods are deprecated
- Ensure all functionality is covered by new implementation

**Green**: Remove legacy code
- Delete old `SynchronizeDeckAsync` and `MergeDecks` methods
- Update any references to use new methods

**Refactor**: Clean up unused imports and dependencies

#### 6.2 Performance Optimization
**Red**: Write performance tests
- Test instruction accumulation performance
- Test execution performance with large datasets

**Green**: Implement optimizations
- Batch API calls where possible
- Optimize instruction ordering
- Add progress reporting

**Refactor**: Improve memory usage and execution speed

#### 6.3 Error Handling and Monitoring
**Red**: Write tests for error scenarios
- Test partial failure handling
- Test rollback mechanisms

**Green**: Implement robust error handling
- Add transaction-like behavior for instruction execution
- Implement proper logging and monitoring

**Refactor**: Improve error recovery and user feedback

## Implementation Order and Dependencies

1. **Phase 1** (Domain Models) - No dependencies
2. **Phase 2** (Execution Engine) - Depends on Phase 1
3. **Phase 3** (Business Logic) - Depends on Phase 1
4. **Phase 4** (Integration) - Depends on Phase 2 & 3
5. **Phase 5** (Testing) - Can run in parallel with other phases
6. **Phase 6** (Cleanup) - Depends on all previous phases

## Risk Assessment

- **High Risk**: Changing core synchronization logic - extensive TDD testing required
- **Medium Risk**: New instruction system complexity - mitigated by TDD approach
- **Low Risk**: Interface additions - well-tested through TDD
- **Low Risk**: Repository updates - minimal changes to existing interface

## Success Criteria

- All TDD cycles complete (Red-Green-Refactor) for each feature
- 90%+ test coverage on new and modified code
- All existing functionality preserved
- New synchronization logic handles all specified scenarios
- Clean architecture maintained with proper separation of concerns
- Performance meets or exceeds current implementation</content>
<parameter name="filePath">d:\Development\AnkiSync\docs\synchronization_plan.md