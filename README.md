# Spreadsheet Engine

## Overview

The Spreadsheet Engine is a C# library designed to create and manage spreadsheets programmatically. It provides a robust set of features for handling cell operations, formula evaluation, and undo/redo functionality.

## Features

- **Dynamic Cell Management**: Create and manipulate cells within a customizable grid
- **Formula Evaluation**: Supports cell references and mathematical expressions
- **Circular Reference Detection**: Prevents infinite loops in cell dependencies
- **Undo/Redo System**: Allows for easy reversal and re-application of changes
- **Event-Driven Updates**: Cells automatically update when dependencies change
- **Error Handling**: Provides clear error messages for invalid formulas or references

## Usage

### Creating a Spreadsheet

```csharp
// Create a new spreadsheet with 10 rows and 5 columns
Spreadsheet spreadsheet = new Spreadsheet(10, 5);
```

### Accessing Cells

```csharp
// Get a cell at row 0, column 0 (A1)
Cell cell = spreadsheet.GetCell(0, 0);

// Set the text of the cell
cell.Text = "Hello, World!";
```

### Working with Formulas

```csharp
// Set a formula in cell B1 that references A1
spreadsheet.GetCell(0, 1).Text = "=A1 + 10";

// The value of B1 will automatically update when A1 changes
```

### Undo/Redo Operations

```csharp
// Perform an action
spreadsheet.AddUndo(new SomeCellChangeCommand());

// Undo the last action
if (spreadsheet.CanUndo)
{
    spreadsheet.Undo();
}

// Redo the undone action
if (spreadsheet.CanRedo)
{
    spreadsheet.Redo();
}
```

## Advanced Features

- **Circular Reference Detection**: The engine automatically detects and prevents circular references in formulas
- **Self-Reference Prevention**: Formulas cannot reference their own cell, avoiding logical errors
- **Dependent Cell Updates**: When a cell's value changes, all dependent cells are automatically recalculated

## Error Handling

The Spreadsheet Engine provides clear error messages for various scenarios:

- Invalid cell references
- Circular dependencies
- Self-references
- Syntax errors in formulas

## Performance Considerations

- The engine uses efficient data structures to manage cell dependencies and updates
- Formula evaluation is optimized to minimize unnecessary recalculations
