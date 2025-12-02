# Card Synchronization Process

This document outlines the high-level pseudo code for the card synchronization process in AnkiSync, from the initial request to the final AnkiConnect API calls.

## Overall Synchronization Flow
The steps for performing the synchronization. 
In the CardSynchronizationService, create a method called "AccumulateSynchronizationInstructions" which is provided a list of source decks to synchronize to and all the Anki decks that exist currently.
This method should not perform any additional queries to the AnkiAdapter and only accumulate instructions to be performed later. These instructions should be declared in the domain layer.
In the method perform the following:
1. Given a list of source decks that should exist
2. Accumulate an ordered list of instructions to execute
4. For all Anki decks that are not in the source, create a list of deck deletion instructions for those decks
5. Iterate through each source deck:
    1. Query for a deck on Anki that matches that source deck
    2. If the deck does not exist, do the following:
        1. Create an instruction to create a new deck
        2. Iterate through every card in the source deck
            1. Look for 
                1. if the card exists, create a card move instruction to move this card to the source deck's deck id
                2. if the card does not exist, create a card creation instruction
    3. If the deck does exist, do the following: 
        1. Query for all cards in that deck on Anki
        2. For all cards in this deck on Anki that do not exist in the source deck, create card deletion instructions
        3. Iterate through every card in this source deck
        4. If there is no matching card in the Anki deck, create a card creation instruction
        5. If there is a matching card in the Anki deck, if the card's contents has changed, create a card update instruction
        6. If there is a matching card in the Anki deck and if the card's contents has not changed, skip this card
6. Add an instruction to perform an AnkiConnect Sync
The flow above should be implemented with small single responsibility methods with roughly 50-100 lines each.

### Card Matching
When performing card matching, given a source card and a target card, utilize a ICardEqualityChecker interface where the interface returns if the 2 cards are a match. Create a equality checker called "ExactMatchEqualityChecker" where for QuestionAndAnswerCards, it checks if the question is exactly the same. For ClozeCards, it checks that the text and the keywords key and values are the same.

### Deck Matching
When performing deck matching, given 2 decks, check that their ParentDeck and the DeckName is the same. 

### Instruction execution
Update the IDeckRepository to be able to invoke this list of instructions