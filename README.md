# Spreadsheet Application

## Overview

This application is a simple spreadsheet program developed using C# and Windows Forms. It provides basic spreadsheet functionality including cell editing, formula evaluation, undo/redo capabilities, and file saving/loading.

## Features

- **Grid View**: A 50x26 grid (A1 to Z50) for data entry and display.
- **Cell Editing**: Users can input text or formulas into cells.
- **Formula Evaluation**: Cells starting with '=' are treated as formulas and evaluated.
- **Undo/Redo**: Full support for undoing and redoing cell changes.
- **Color Customization**: Users can change the background color of cells.
- **File Operations**: Save and load spreadsheet data in XML format.
- **Random Cell Population**: A button to populate random cells with "I love C#!".

## Key Components

### Form1 Class

The main form of the application, responsible for:
- Initializing the spreadsheet and UI components.
- Handling user interactions (cell editing, button clicks, menu selections).
- Managing the undo/redo functionality.
- Coordinating between the UI and the underlying spreadsheet model.

### Spreadsheet Class

Represents the data model of the spreadsheet, handling:
- Cell value storage and retrieval.
- Formula evaluation and cell dependency management.
- Event notifications for cell property changes.

### SpreadsheetXmlHandler Class

Manages the saving and loading of spreadsheet data in XML format.

## Usage

1. **Cell Editing**: Click on a cell to edit its content. Enter text or formulas (starting with '=').
2. **Undo/Redo**: Use the Undo and Redo buttons or menu items to revert or reapply changes.
3. **Change Cell Color**: Select cells and use the "Change Color" option in the context menu.
4. **Save/Load**: Use the File menu to save your spreadsheet or load an existing one.
5. **Random Population**: Click the button to randomly populate cells with "I love C#!".

## Implementation Details

- The application uses a custom `Spreadsheet` class to manage the data and logic.
- Cell changes trigger property change events, updating the UI accordingly.
- Formulas are evaluated using a separate `ExpressionTree` class (not shown in the provided code).
- Undo/Redo functionality is implemented using command pattern.
- XML serialization is used for file I/O operations.

## Future Enhancements

- Implement more complex formula functions.
- Add cell formatting options (font, alignment, etc.).
- Improve performance for large spreadsheets.
- Add support for charts and graphs.

Citations:
[1] https://ppl-ai-file-upload.s3.amazonaws.com/web/direct-files/6790462/b1163518-a1b0-46d6-8fcb-4fd6d2e3b920/paste.txt
