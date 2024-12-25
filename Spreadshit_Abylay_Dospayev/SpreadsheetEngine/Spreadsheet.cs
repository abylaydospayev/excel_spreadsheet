// <copyright file="Spreadsheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a spreadsheet containing a grid of cells.
    /// </summary>
    public class Spreadsheet
    {
        private readonly Cell[,] cells;

        /// <summary>
        /// Event triggered when a cell's property changes.
        /// </summary>
        public event EventHandler<CellChangedEventArgs> CellPropertyChanged;

        /// <summary>
        /// Gets the number of rows in the spreadsheet.
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// Gets the number of columns in the spreadsheet.
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        public Spreadsheet(int rows, int columns)
        {
            this.RowCount = rows;
            this.ColumnCount = columns;
            this.cells = new Cell[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    this.cells[row, col] = this.CreateCell(row, col);
                    this.cells[row, col].PropertyChanged += this.Cell_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// Creates a new cell instance.
        /// </summary>
        protected virtual Cell CreateCell(int rowIndex, int columnIndex)
        {
            return new DefaultCell(rowIndex, columnIndex);
        }

        /// <summary>
        /// Retrieves a cell at a specific location.
        /// </summary>
        public Cell GetCell(int row, int column)
        {
            if (row < 0 || row >= RowCount || column < 0 || column >= ColumnCount)
            {
                return null;
            }

            return this.cells[row, column];
        }


        // <summary>
        /// Handles the PropertyChanged event of a cell.
        /// </summary>

        private void Cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Cell cell)
            {
                if (e.PropertyName == nameof(Cell.Text))
                {
                    this.UpdateCellValue(cell);
                    this.UpdateDependentCells(cell);
                }
                this.CellPropertyChanged?.Invoke(this, new CellChangedEventArgs(cell));
            }
        }

        /// <summary>
        /// Updates the value of a cell based on its text content.
        /// </summary>
        private HashSet<string> evaluatingCells = new HashSet<string>();

        /// <summary>
        /// Updates the value of a cell based on its text content.
        /// </summary>
        /// <param name="cell">The cell to update.</param>
        private void UpdateCellValue(Cell cell)
        {
            if (cell.Text.StartsWith("="))
            {
                string formula = cell.Text.Substring(1);
                string cellName = GetCellName(cell);

                if (evaluatingCells.Contains(cellName))
                {
                    cell.SetValue("Error: Circular reference detected");
                    return;
                }

                evaluatingCells.Add(cellName);
                try
                {
                    if (ContainsSelfReference(formula, cell))
                    {
                        cell.SetValue("Error: Self-reference detected");
                    }
                    else if (HasCircularReference(cell, new HashSet<string>()))
                    {
                        cell.SetValue("Error: Circular reference detected");
                    }
                    else
                    {
                        cell.SetValue(EvaluateFormula(formula, cell));
                    }
                }
                finally
                {
                    evaluatingCells.Remove(cellName);
                }
            }
            else
            {
                cell.SetValue(cell.Text);
            }
        }

        /// <summary>
        /// Checks if a cell has a circular reference.
        /// </summary>
        /// <param name="cell">The cell to check.</param>
        /// <param name="visitedCells">Set of cells already visited in this check.</param>
        /// <returns>True if a circular reference is detected, false otherwise.</returns>
        private bool HasCircularReference(Cell cell, HashSet<string> visitedCells)
        {
            string cellName = GetCellName(cell);
            if (visitedCells.Contains(cellName))
            {
                return true;
            }

            visitedCells.Add(cellName);

            if (cell.Text.StartsWith("="))
            {
                string formula = cell.Text.Substring(1);
                foreach (var referencedCellName in GetVariableNames(formula))
                {
                    var referencedCell = GetReferencedCell(referencedCellName, cell.RowIndex, cell.ColumnIndex);
                    if (referencedCell != null && HasCircularReference(referencedCell, new HashSet<string>(visitedCells)))
                    {
                        return true;
                    }
                }
            }

            visitedCells.Remove(cellName);
            return false;
        }

        /// <summary>
        /// Gets the name of a cell in A1 notation.
        /// </summary>
        /// <param name="cell">The cell to get the name for.</param>
        /// <returns>The cell name in A1 notation.</returns>
        private string GetCellName(Cell cell)
        {
            return $"{(char)('A' + cell.ColumnIndex)}{cell.RowIndex + 1}";
        }

        /// <summary>
        /// Checks if a formula contains a reference to itself.
        /// </summary>
        /// <param name="formula">The formula to check.</param>
        /// <param name="cell">The cell containing the formula.</param>
        /// <returns>True if the formula contains a self-reference, false otherwise.</returns>
        private bool ContainsSelfReference(string formula, Cell cell)
        {
            string cellName = GetCellName(cell);
            return GetVariableNames(formula).Contains(cellName);
        }

        /// <summary>
        /// Updates all cells that depend on the changed cell.
        /// </summary>
        private void UpdateDependentCells(Cell changedCell)
        {
            HashSet<Cell> updatedCells = new HashSet<Cell>();
            Queue<Cell> cellsToUpdate = new Queue<Cell>();
            cellsToUpdate.Enqueue(changedCell);

            while (cellsToUpdate.Count > 0)
            {
                var cell = cellsToUpdate.Dequeue();
                for (int row = 0; row < this.RowCount; row++)
                {
                    for (int col = 0; col < this.ColumnCount; col++)
                    {
                        var dependentCell = this.GetCell(row, col);
                        if (dependentCell.Text.StartsWith("=") && this.DependsOn(dependentCell, cell))
                        {
                            this.UpdateCellValue(dependentCell);
                            if (!updatedCells.Contains(dependentCell))
                            {
                                cellsToUpdate.Enqueue(dependentCell);
                                updatedCells.Add(dependentCell);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a cell depends on another cell.
        /// </summary>
        private bool DependsOn(Cell cell, Cell dependencyCell)
        {
            var variables = this.GetVariableNames(cell.Text);
            string dependencyCellName = $"{(char)('A' + dependencyCell.ColumnIndex)}{dependencyCell.RowIndex + 1}";
            return variables.Contains(dependencyCellName);
        }

        /// <summary>
        /// Evaluates a formula for a given cell.
        /// </summary>
        private string EvaluateFormula(string formula, Cell currentCell)
        {
            try
            {
                var expressionTree = new ExpressionTree(formula);
                var variables = new Dictionary<string, double>();
                foreach (var variableName in this.GetVariableNames(formula))
                {
                    var referencedCell = this.GetReferencedCell(variableName, currentCell.RowIndex, currentCell.ColumnIndex);
                    if (referencedCell == null)
                    {
                        return $"Error: Invalid cell reference '{variableName}'";
                    }
                    if (double.TryParse(referencedCell.Value, out double value))
                    {
                        variables[variableName] = value;
                    }
                    else
                    {
                        variables[variableName] = 0; // Treat non-numeric values as 0
                    }
                }
                return expressionTree.Evaluate(variables).ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }





        // <summary>
        /// Extracts variable names from a formula.
        /// </summary>
        private IEnumerable<string> GetVariableNames(string formula)
        {
            // Simple regex to match cell references like A1, B2, etc.
            var matches = System.Text.RegularExpressions.Regex.Matches(formula, @"[A-Z]\d+");
            return matches.Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value);
        }

        /// <summary>
        /// Gets a referenced cell based on its string representation.
        /// </summary>
        private Cell GetReferencedCell(string reference, int currentRowIndex, int currentColumnIndex)
        {
            if (reference.Length < 2)
            {
                return null;
            }

            char columnLetter = reference[0];
            if (!int.TryParse(reference.Substring(1), out int rowNumber))
            {
                return null;
            }

            int columnIndex = columnLetter - 'A';
            int rowIndex = rowNumber - 1;

            if (rowIndex < 0 || rowIndex >= this.RowCount || columnIndex < 0 || columnIndex >= ColumnCount)
            {
                return null;
            }

            return this.GetCell(rowIndex, columnIndex);
        }

        /// <summary>
        /// Represents the undo/redo system for the spreadsheet.
        /// </summary>
        private UndoRedoSystem undoRedoSystem = new UndoRedoSystem();

        public void AddUndo(ICommand command)
        {
            this.undoRedoSystem.AddUndo(command);
        }

        /// <summary>
        /// Performs an undo operation.
        /// </summary>
        public void Undo()
        {
            this.undoRedoSystem.Undo();
        }

        /// <summary>
        /// Performs a redo operation.
        /// </summary>
        public void Redo()
        {
            this.undoRedoSystem.Redo();
        }

        /// <summary>
        /// Gets a value indicating whether an undo operation can be performed.
        /// </summary>
        public bool CanUndo => this.undoRedoSystem.CanUndo;

        /// <summary>
        /// Gets a value indicating whether a redo operation can be performed.
        /// </summary>
        public bool CanRedo => this.undoRedoSystem.CanRedo;


        /// <summary>
        /// Gets the description of the next undo operation.
        /// </summary>
        public string UndoDescription => this.undoRedoSystem.UndoDescription;

        /// <summary>
        /// Gets the description of the next redo operation.
        /// </summary>
        public string RedoDescription => this.undoRedoSystem.RedoDescription;


        /// <summary>
        /// Clears all cell contents and resets background colors in the spreadsheet.
        /// </summary>
        public void ClearAll()
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    Cell cell = GetCell(row, col);
                    cell.Text = string.Empty;
                    cell.BGColor = 0xFFFFFFFF;
                }
            }
        }

        /// <summary>
        /// Clears the undo and redo stacks, resetting the undo/redo functionality.
        /// </summary>
        public void ClearUndoRedo()
        {
            // Clear undo/redo stacks
            undoRedoSystem = new UndoRedoSystem();
        }

        /// <summary>
        /// Evaluates all formula cells in the spreadsheet.
        /// </summary>
        /// <remarks>
        /// This method iterates through all cells in the spreadsheet and updates
        /// the values of cells containing formulas (starting with '=').
        /// </remarks>
        public void EvaluateAllFormulas()
        {
            evaluatingCells.Clear();
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    Cell cell = GetCell(row, col);
                    if (cell.Text.StartsWith("=") && cell.Value != "Error: Circular reference detected")
                    {
                        UpdateCellValue(cell);
                    }
                }
            }
        }




    }

}