"""
Obsidian Spaced Repetition Flashcard Parser
Parses various flashcard formats used by the Obsidian Spaced Repetition plugin.
"""

import re
from typing import List, Dict, Tuple, Optional
from dataclasses import dataclass
from enum import Enum


class FlashcardType(Enum):
    SINGLE_LINE_BASIC = "single_line_basic"      # question::answer
    SINGLE_LINE_BIDIRECTIONAL = "single_line_bidirectional"  # question:::answer
    MULTI_LINE_BASIC = "multi_line_basic"        # question?\nanswer
    MULTI_LINE_BIDIRECTIONAL = "multi_line_bidirectional"  # question??\nanswer
    CLOZE = "cloze"                             # text with ==deletions==


@dataclass
class Flashcard:
    """Represents a parsed flashcard"""
    type: FlashcardType
    front: str
    back: str
    file_path: str
    line_number: int
    raw_text: str
    tags: List[str] = None
    deck: str = None

    def __post_init__(self):
        if self.tags is None:
            self.tags = []


class ObsidianFlashcardParser:
    """
    Parser for Obsidian Spaced Repetition flashcard formats.

    Supports:
    - Single-line basic: Question::Answer
    - Single-line bidirectional: Question:::Answer
    - Multi-line basic: Question?\nAnswer
    - Multi-line bidirectional: Question??\nAnswer
    - Cloze deletions: Text with ==deletions==
    """

    def __init__(self):
        # Default separators (configurable in Obsidian settings)
        self.single_line_separator = "::"
        self.single_line_bidirectional_separator = ":::"
        self.multi_line_separator = "?"
        self.multi_line_bidirectional_separator = "??"
        self.cloze_delimiter = "=="

        # Compile regex patterns for efficiency
        self._compile_patterns()

    def _compile_patterns(self):
        """Compile regex patterns for flashcard detection"""

        # Single-line bidirectional first (:::) - most specific
        self.single_line_bidirectional_pattern = re.compile(
            r'^([^:\n]+):::(.+)$',  # text:::text (no colons in first part)
            re.MULTILINE
        )

        # Single-line basic: text::text (but not :::)
        self.single_line_pattern = re.compile(
            r'^([^:\n]+)::(?!:)(.+)$',  # text::text (no triple colons)
            re.MULTILINE
        )

        # Multi-line bidirectional: text?? followed by newline and text
        self.multi_line_bidirectional_pattern = re.compile(
            r'^(.+?)\?\?\s*\n(.+?)(?=\n\s*\n|\n*$|$)',
            re.MULTILINE | re.DOTALL
        )

        # Multi-line basic: text? followed by newline and text (but not ??)
        self.multi_line_pattern = re.compile(
            r'^(.+?)\?(?!\?)\s*\n(.+?)(?=\n\s*\n|\n*$|$)',
            re.MULTILINE | re.DOTALL
        )

        # Cloze deletions: ==text==
        self.cloze_pattern = re.compile(
            rf'{re.escape(self.cloze_delimiter)}(.*?){re.escape(self.cloze_delimiter)}'
        )

    def _extract_file_tags(self, content: str) -> List[str]:
        """
        Extract all tags from the file content.
        Tags are in the format #tagname

        Args:
            content: File content as string

        Returns:
            List of tag names (without the # prefix)
        """
        # Find all #tag patterns, but exclude those that are part of URLs or code
        tag_pattern = re.compile(r'#([a-zA-Z][a-zA-Z0-9_-]*)')
        tags = []

        for match in tag_pattern.finditer(content):
            tag = match.group(1)
            # Skip if this looks like it's part of a URL or code block
            start_pos = match.start()
            # Check context - if preceded by http, https, or in a code block, skip
            context_before = content[max(0, start_pos-10):start_pos]
            if 'http' in context_before.lower() or '```' in context_before:
                continue
            tags.append(tag)

        return list(set(tags))  # Remove duplicates

    def parse_file(self, file_path: str, content: str) -> List[Flashcard]:
        """
        Parse a file for flashcards using a line-by-line approach.

        Args:
            file_path: Path to the file being parsed
            content: File content as string

        Returns:
            List of parsed flashcards
        """
        flashcards = []
        lines = content.split('\n')

        # Extract tags from the entire file content
        file_tags = self._extract_file_tags(content)

        i = 0
        while i < len(lines):
            line = lines[i].strip()

            # Try to match different flashcard types at this line
            card = None

            # Check for single-line flashcards
            if ':::' in line:
                card = self._parse_single_line_at_line(lines, i, is_bidirectional=True)
            elif '::' in line and not line.endswith('::'):
                card = self._parse_single_line_at_line(lines, i, is_bidirectional=False)

            # Check for multi-line flashcards
            elif line.endswith('??'):
                card = self._parse_multi_line_at_line(lines, i, is_bidirectional=True)
            elif line.endswith('?') and not line.endswith('??'):
                card = self._parse_multi_line_at_line(lines, i, is_bidirectional=False)

            if card:
                # Add file-level tags to the flashcard
                card['tags'] = file_tags.copy()
                flashcards.append(card)
                # Skip lines that were consumed by this flashcard
                i += card.get('lines_consumed', 1)
            else:
                i += 1

        # Parse cloze cards separately (they can be anywhere)
        cloze_cards = self._parse_cloze_cards_simple(file_path, content, lines)
        for cloze_card in cloze_cards:
            # Add file-level tags to cloze cards too
            cloze_card.tags.extend(file_tags)
            flashcards.append(cloze_card)

        # Convert dicts to Flashcard objects
        result = []
        for card_data in flashcards:
            if isinstance(card_data, dict):
                card_type = card_data['type']

                # Create the primary card
                result.append(Flashcard(
                    type=card_type,
                    front=card_data['front'],
                    back=card_data['back'],
                    file_path=file_path,
                    line_number=card_data['line_number'],
                    raw_text=f"{card_data['front']} :: {card_data['back']}"
                ))

                # Add reverse direction for bidirectional cards
                if 'bidirectional' in str(card_type.value):
                    result.append(Flashcard(
                        type=card_type,
                        front=card_data['back'],
                        back=card_data['front'],
                        file_path=file_path,
                        line_number=card_data['line_number'],
                        raw_text=f"{card_data['back']} :: {card_data['front']}"
                    ))
            else:
                # Already a Flashcard object
                result.append(card_data)

        return result

    def _parse_single_line_at_line(self, lines: List[str], line_idx: int, is_bidirectional: bool) -> Optional[Dict]:
        """Parse a single-line flashcard at the given line index"""
        line = lines[line_idx].strip()

        if is_bidirectional:
            # text:::text format
            if ':::' not in line:
                return None
            parts = line.split(':::', 1)
            if len(parts) != 2:
                return None

            front, back = parts[0].strip(), parts[1].strip()

            # For bidirectional, return the first direction
            # The second direction will be created in the main parsing loop
            return {
                'type': FlashcardType.SINGLE_LINE_BIDIRECTIONAL,
                'front': front,
                'back': back,
                'line_number': line_idx + 1,
                'lines_consumed': 1
            }
        else:
            # text::text format
            if '::' not in line or line.count('::') > 1:
                return None
            parts = line.split('::', 1)
            if len(parts) != 2:
                return None

            front, back = parts[0].strip(), parts[1].strip()

            return {
                'type': FlashcardType.SINGLE_LINE_BASIC,
                'front': front,
                'back': back,
                'line_number': line_idx + 1,
                'lines_consumed': 1
            }

    def _parse_multi_line_at_line(self, lines: List[str], line_idx: int, is_bidirectional: bool) -> Optional[Dict]:
        """Parse a multi-line flashcard starting at the given line index"""
        separator = '??' if is_bidirectional else '?'

        # Collect the question part (until separator)
        question_lines = []
        i = line_idx

        while i < len(lines):
            line = lines[i]
            if line.strip().endswith(separator):
                # Found the separator - add the line without separator
                question_lines.append(line.strip()[:-len(separator)].strip())
                break
            question_lines.append(line)
            i += 1
        else:
            return None  # No separator found

        # Move past the separator line
        i += 1

        # Collect the answer part (until empty line or end)
        answer_lines = []
        while i < len(lines):
            line = lines[i]
            if line.strip() == '':
                break
            answer_lines.append(line)
            i += 1

        question = '\n'.join(question_lines).strip()
        answer = '\n'.join(answer_lines).strip()

        if not question or not answer:
            return None

        return {
            'type': FlashcardType.MULTI_LINE_BIDIRECTIONAL if is_bidirectional else FlashcardType.MULTI_LINE_BASIC,
            'front': question,
            'back': answer,
            'line_number': line_idx + 1,
            'lines_consumed': i - line_idx
        }

    def _parse_single_line_basic(self, file_path: str, content: str, lines: List[str]) -> List[Flashcard]:
        """Legacy method - replaced by line-by-line parsing"""
        return []

    def _parse_single_line_bidirectional(self, file_path: str, content: str, lines: List[str]) -> List[Flashcard]:
        """Legacy method - replaced by line-by-line parsing"""
        return []

    def _parse_multi_line_basic(self, file_path: str, content: str, lines: List[str]) -> List[Flashcard]:
        """Legacy method - replaced by line-by-line parsing"""
        return []

    def _parse_multi_line_bidirectional(self, file_path: str, content: str, lines: List[str]) -> List[Flashcard]:
        """Legacy method - replaced by line-by-line parsing"""
        return []

    def _parse_cloze_cards(self, file_path: str, content: str, lines: List[str]) -> List[Flashcard]:
        """Legacy method - replaced by simplified cloze parsing"""
        return []

    def _get_line_number(self, content: str, position: int) -> int:
        """Get the line number for a given position in content"""
        return content[:position].count('\n') + 1

    def extract_tags_and_deck(self, flashcard: Flashcard, file_content: str) -> None:
        """
        Extract tags and deck information from the file content.
        This is a simplified implementation - in practice, you'd need more sophisticated parsing.
        """
        # Look for #flashcard tags near the flashcard
        lines = file_content.split('\n')
        start_line = max(0, flashcard.line_number - 5)  # Look a few lines before
        end_line = min(len(lines), flashcard.line_number + 5)  # Look a few lines after

        for line_num in range(start_line, end_line):
            line = lines[line_num].strip()

            # Extract tags (simplified - just #tag format)
            tag_matches = re.findall(r'#(\w+)', line)
            if tag_matches:
                flashcard.tags.extend(tag_matches)

            # Extract deck from folder structure (simplified)
            # In practice, this would be based on file path
            if 'flashcard' in tag_matches:
                # Use parent folder as deck
                import os
                parent_dir = os.path.basename(os.path.dirname(flashcard.file_path))
                if parent_dir and parent_dir != os.path.basename(flashcard.file_path):
                    flashcard.deck = parent_dir

    def _parse_cloze_cards_simple(self, file_path: str, content: str, lines: List[str]) -> List[Flashcard]:
        """Parse cloze deletion flashcards using a simpler approach"""
        flashcards = []

        # Process each line that contains cloze deletions
        for line_idx, line in enumerate(lines):
            if self.cloze_pattern.search(line):
                # This line has cloze deletions
                line_number = line_idx + 1

                # Generate individual cards for each cloze deletion in this line
                cloze_cards = self._generate_cloze_cards_from_line(line)

                for front, back in cloze_cards:
                    flashcards.append(Flashcard(
                        type=FlashcardType.CLOZE,
                        front=front,
                        back=back,
                        file_path=file_path,
                        line_number=line_number,
                        raw_text=line
                    ))

        return flashcards

    def _generate_cloze_cards_from_line(self, line: str) -> List[Tuple[str, str]]:
        """Generate cloze cards from a single line with multiple deletions"""
        cards = []

        # Find all cloze deletions in this line
        deletions = list(self.cloze_pattern.finditer(line))

        if not deletions:
            return cards

        # Generate a card for each deletion
        for i, deletion in enumerate(deletions):
            # Create front: hide this deletion, show others
            front_parts = []
            last_end = 0

            for j, other_deletion in enumerate(deletions):
                # Add text before this deletion
                front_parts.append(line[last_end:other_deletion.start()])

                if j == i:
                    # This is the deletion to hide
                    front_parts.append('[...]')
                else:
                    # Show other deletions
                    front_parts.append(other_deletion.group(1))

                last_end = other_deletion.end()

            # Add remaining text
            front_parts.append(line[last_end:])

            front = ''.join(front_parts).strip()
            back = line.strip()  # Full line is the back

            cards.append((front, back))

        return cards


# Example usage and testing
def test_parser():
    """Test the flashcard parser with sample content"""
    parser = ObsidianFlashcardParser()

    sample_content = """
# My Flashcards

What is Python?::A programming language

Capital of France:::Paris

What is the capital of France?
?
Paris is the capital and most populous city of France.

The ==capital== of ==France== is ==Paris==.

#flashcard What is recursion?::A function that calls itself

Some more text with ==cloze== deletions ==here==.

Multi-line example
?
This is the answer that spans multiple lines.
It can contain various information.

Another bidirectional example
??
This creates two cards - one in each direction.
"""

    flashcards = parser.parse_file("test.md", sample_content)

    print(f"Found {len(flashcards)} flashcards:")
    for i, card in enumerate(flashcards, 1):
        print(f"\n{i}. Type: {card.type.value}")
        print(f"   Front: {card.front[:50]}...")
        print(f"   Back: {card.back[:50]}...")
        print(f"   Line: {card.line_number}")


if __name__ == "__main__":
    test_parser()