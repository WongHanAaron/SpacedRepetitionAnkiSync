"""
Anki Sync Prototype
This script demonstrates how to sync with an Anki instance using AnkiConnect.
Requires: AnkiConnect add-on installed in Anki (https://ankiweb.net/shared/info/2055492159)
"""

import json
import urllib.request
import urllib.error


class AnkiConnector:
    """Interface to communicate with Anki via AnkiConnect API"""
    
    def __init__(self, url="http://localhost:8765"):
        self.url = url
        self.version = 6
    
    def _invoke(self, action, **params):
        """Send a request to AnkiConnect"""
        request_json = json.dumps({
            "action": action,
            "version": self.version,
            "params": params
        }).encode('utf-8')
        
        try:
            response = urllib.request.urlopen(
                urllib.request.Request(self.url, request_json),
                timeout=10
            )
            response_data = json.loads(response.read().decode('utf-8'))
            
            if len(response_data) != 2:
                raise Exception('Response has an unexpected number of fields')
            if 'error' not in response_data:
                raise Exception('Response is missing required error field')
            if 'result' not in response_data:
                raise Exception('Response is missing required result field')
            if response_data['error'] is not None:
                raise Exception(response_data['error'])
            
            return response_data['result']
        
        except urllib.error.URLError as e:
            raise Exception(f"Failed to connect to Anki. Make sure Anki is running with AnkiConnect installed: {e}")
    
    def check_connection(self):
        """Verify connection to Anki"""
        try:
            version = self._invoke("version")
            print(f"✓ Connected to AnkiConnect (version {version})")
            return True
        except Exception as e:
            print(f"✗ Connection failed: {e}")
            return False
    
    def get_deck_names(self):
        """Get all deck names"""
        return self._invoke("deckNames")
    
    def get_deck_stats(self, deck_name):
        """Get statistics for a specific deck"""
        return self._invoke("getDeckStats", decks=deck_name)
    
    def find_notes(self, query):
        """Find notes matching a query"""
        return self._invoke("findNotes", query=query)
    
    def get_note_info(self, note_ids):
        """Get detailed information about notes"""
        return self._invoke("notesInfo", notes=note_ids)
    
    def add_note(self, deck_name, front, back, tags=None):
        """Add a new note to a deck"""
        if tags is None:
            tags = []
        
        note = {
            "deckName": deck_name,
            "modelName": "Basic",
            "fields": {
                "Front": front,
                "Back": back
            },
            "tags": tags
        }
        
        return self._invoke("addNote", note=note)
    
    def update_note_fields(self, note_id, fields):
        """Update fields of an existing note"""
        note = {
            "id": note_id,
            "fields": fields
        }
        return self._invoke("updateNoteFields", note=note)
    
    def sync(self):
        """Trigger Anki sync with AnkiWeb"""
        return self._invoke("sync")
    
    def get_reviews_of_cards(self, deck_name, start_date):
        """Get review history for cards in a deck"""
        # Note: This requires finding cards first
        card_ids = self._invoke("findCards", query=f"deck:{deck_name}")
        return self._invoke("getReviewsOfCards", cards=card_ids)


def demo_sync():
    """Demonstrate basic Anki sync functionality"""
    
    print("=== Anki Sync Prototype ===\n")
    
    # Initialize connector
    anki = AnkiConnector()
    
    # Check connection
    if not anki.check_connection():
        print("\nMake sure:")
        print("1. Anki is running")
        print("2. AnkiConnect add-on is installed")
        print("3. AnkiConnect is configured to allow connections")
        return
    
    print()
    
    # Get all decks
    try:
        decks = anki.get_deck_names()
        print(f"Available decks ({len(decks)}):")
        for deck in decks:
            print(f"  - {deck}")
        print()
        
        # Get stats for each deck
        if decks:
            print("Deck Statistics:")
            for deck in decks[:3]:  # Show first 3 decks
                try:
                    stats = anki.get_deck_stats(deck)
                    print(f"\n  {deck}:")
                    print(f"    New cards: {stats.get('new_count', 0)}")
                    print(f"    Learning: {stats.get('learn_count', 0)}")
                    print(f"    Review: {stats.get('review_count', 0)}")
                except Exception as e:
                    print(f"    (Could not get stats: {e})")
        
        # Example: Find recent notes
        print("\n" + "="*40)
        print("Finding notes in first deck...")
        if decks and decks[0] != "Default":
            deck_name = decks[0]
            note_ids = anki.find_notes(f"deck:{deck_name}")
            print(f"Found {len(note_ids)} notes in '{deck_name}'")
            
            # Get details of first few notes
            if note_ids:
                sample_notes = anki.get_note_info(note_ids[:3])
                print(f"\nSample notes:")
                for note in sample_notes:
                    fields = note.get('fields', {})
                    print(f"  - Note ID {note['noteId']}: {list(fields.keys())}")
        
        # Example: Add a new note (commented out for safety)
        print("\n" + "="*40)
        print("To add a new note, uncomment the following code:")
        print('# note_id = anki.add_note("Default", "Question?", "Answer!", ["prototype"])')
        print('# print(f"Added note with ID: {note_id}")')
        
        # Sync with AnkiWeb
        print("\n" + "="*40)
        print("To sync with AnkiWeb, uncomment:")
        print('# anki.sync()')
        print('# print("Sync initiated")')
        anki.add_note("Default", "Question?", "Answer!", ["prototype"])
        
    except Exception as e:
        print(f"Error during sync demo: {e}")


if __name__ == "__main__":
    demo_sync()
