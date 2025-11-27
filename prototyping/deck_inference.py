"""
Deck Inference Engine for AnkiSync
Maps flashcards to Anki decks based on tags and configuration.
"""

from typing import Dict, List, Optional, Set
import re
from dataclasses import dataclass


@dataclass
class DeckMappingRule:
    """Represents a rule for mapping tags to decks"""
    tag_pattern: str  # Regex pattern to match tags
    deck_name: str    # Target Anki deck name
    priority: int     # Higher priority rules take precedence
    compiled_pattern: Optional[re.Pattern] = None

    def __post_init__(self):
        self.compiled_pattern = re.compile(self.tag_pattern, re.IGNORECASE)


class DeckInferenceEngine:
    """
    Engine for inferring Anki deck names based on flashcard tags and configuration.

    Supports:
    - Tag-to-deck mapping with regex patterns
    - Priority-based conflict resolution
    - Folder-based deck inference
    - Default deck fallback
    """

    def __init__(self, config: Dict):
        """
        Initialize the deck inference engine.

        Args:
            config: Configuration dictionary with deck mapping rules
        """
        self.default_deck = config.get('default_deck', 'Default')
        self.tag_mappings = self._load_tag_mappings(config.get('tag_mappings', {}))
        self.folder_mappings = config.get('folder_mappings', {})

    def _load_tag_mappings(self, mappings_config: Dict) -> List[DeckMappingRule]:
        """Load tag-to-deck mapping rules from configuration"""
        rules = []

        for tag_pattern, deck_config in mappings_config.items():
            if isinstance(deck_config, str):
                # Simple mapping: "tag": "Deck Name"
                rules.append(DeckMappingRule(
                    tag_pattern=tag_pattern,
                    deck_name=deck_config,
                    priority=1
                ))
            elif isinstance(deck_config, dict):
                # Advanced mapping with priority
                rules.append(DeckMappingRule(
                    tag_pattern=tag_pattern,
                    deck_name=deck_config['deck'],
                    priority=deck_config.get('priority', 1)
                ))

        # Sort by priority (highest first)
        rules.sort(key=lambda r: r.priority, reverse=True)
        return rules

    def infer_deck(self, file_path: str, tags: List[str]) -> str:
        """
        Infer the appropriate Anki deck for a flashcard.

        Args:
            file_path: Path to the file containing the flashcard
            tags: List of tags associated with the flashcard

        Returns:
            Anki deck name
        """
        # First, try tag-based mapping
        tag_based_deck = self._infer_from_tags(tags)
        if tag_based_deck:
            return tag_based_deck

        # Second, try folder-based mapping
        folder_based_deck = self._infer_from_folder(file_path)
        if folder_based_deck:
            return folder_based_deck

        # Finally, use default deck
        return self.default_deck

    def _infer_from_tags(self, tags: List[str]) -> Optional[str]:
        """
        Infer deck from tags using configured mapping rules.

        Args:
            tags: List of tags to check against mapping rules

        Returns:
            Deck name if a rule matches, None otherwise
        """
        if not tags:
            return None

        # Convert tags to set for faster lookup
        tag_set = set(tags)

        # Check each rule in priority order
        for rule in self.tag_mappings:
            for tag in tag_set:
                if rule.compiled_pattern.match(tag):
                    return rule.deck_name

        return None

    def _infer_from_folder(self, file_path: str) -> Optional[str]:
        """
        Infer deck from file path using folder mappings.

        Args:
            file_path: Path to the file

        Returns:
            Deck name if a folder mapping matches, None otherwise
        """
        import os

        # Get the directory path
        dir_path = os.path.dirname(file_path)

        # Check folder mappings
        for folder_pattern, deck_name in self.folder_mappings.items():
            # Simple string matching for now (could be enhanced with regex)
            if folder_pattern in dir_path:
                return deck_name

        # Try to use the immediate parent folder as deck name
        parent_folder = os.path.basename(dir_path)
        if parent_folder and parent_folder != os.path.basename(file_path):
            # Clean up folder name for use as deck name
            deck_name = self._sanitize_deck_name(parent_folder)
            if deck_name:
                return deck_name

        return None

    def _sanitize_deck_name(self, name: str) -> str:
        """
        Sanitize a folder name to be used as an Anki deck name.

        Args:
            name: Raw folder name

        Returns:
            Sanitized deck name
        """
        # Remove common prefixes/suffixes
        name = re.sub(r'^(notes?|flashcards?|cards?)[\-_]?', '', name, flags=re.IGNORECASE)
        name = re.sub(r'[\-_]?(notes?|flashcards?|cards?)$', '', name, flags=re.IGNORECASE)

        # Replace underscores and hyphens with spaces
        name = name.replace('_', ' ').replace('-', ' ')

        # Title case
        name = name.title()

        # Remove extra whitespace
        name = ' '.join(name.split())

        return name if name else None

    def get_available_decks(self) -> Set[str]:
        """
        Get all deck names that could be inferred from current configuration.

        Returns:
            Set of possible deck names
        """
        decks = {self.default_deck}

        # Add decks from tag mappings
        for rule in self.tag_mappings:
            decks.add(rule.deck_name)

        # Add decks from folder mappings
        decks.update(self.folder_mappings.values())

        return decks

    def validate_configuration(self) -> List[str]:
        """
        Validate the current configuration for issues.

        Returns:
            List of validation error messages
        """
        errors = []

        # Check for invalid regex patterns
        for rule in self.tag_mappings:
            try:
                re.compile(rule.tag_pattern)
            except re.error as e:
                errors.append(f"Invalid regex pattern '{rule.tag_pattern}': {e}")

        # Check for duplicate priorities (warning, not error)
        priorities = [rule.priority for rule in self.tag_mappings]
        if len(priorities) != len(set(priorities)):
            errors.append("Warning: Multiple rules have the same priority - order may be unpredictable")

        return errors


# Example configuration and usage
def create_example_config():
    """Create an example configuration for deck inference"""
    return {
        'default_deck': 'Default',
        'tag_mappings': {
            # Simple mappings
            'programming': 'Programming',
            'python': 'Programming::Python',
            'javascript': 'Programming::JavaScript',

            # Regex patterns
            r'^lang-(\w+)$': 'Languages',  # lang-spanish -> Languages
            r'^cs-\w+$': 'Computer Science',  # cs-algorithms -> Computer Science

            # Advanced mappings with priority
            'important': {
                'deck': 'High Priority',
                'priority': 10
            },
            'review': {
                'deck': 'Review',
                'priority': 5
            }
        },
        'folder_mappings': {
            'Notes/Programming': 'Programming',
            'Notes/Languages': 'Languages',
            'Flashcards': 'Study Cards'
        }
    }


def test_deck_inference():
    """Test the deck inference engine"""
    config = create_example_config()
    engine = DeckInferenceEngine(config)

    # Test cases
    test_cases = [
        # (file_path, tags, expected_deck)
        ('C:/Notes/Programming/python.md', ['python', 'programming'], 'Programming::Python'),
        ('C:/Notes/Languages/spanish.md', ['lang-spanish'], 'Languages'),
        ('C:/Flashcards/review.md', ['review', 'important'], 'High Priority'),  # important has higher priority
        ('C:/Notes/random.md', [], 'Default'),  # no tags, no folder match
        ('C:/Notes/Programming/algorithms.md', ['cs-algorithms'], 'Computer Science'),
    ]

    print("Testing Deck Inference Engine:")
    print("=" * 50)

    for file_path, tags, expected in test_cases:
        inferred = engine.infer_deck(file_path, tags)
        status = "✓" if inferred == expected else "✗"
        print(f"{status} {file_path}")
        print(f"  Tags: {tags}")
        print(f"  Expected: {expected}")
        print(f"  Inferred: {inferred}")
        print()

    # Show validation
    errors = engine.validate_configuration()
    if errors:
        print("Configuration Issues:")
        for error in errors:
            print(f"  - {error}")
    else:
        print("Configuration is valid ✓")


if __name__ == "__main__":
    test_deck_inference()