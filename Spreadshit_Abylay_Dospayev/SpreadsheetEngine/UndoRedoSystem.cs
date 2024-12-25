using System;
using System.Collections.Generic;
// <copyright file="UndoRedoSystem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents a system for managing undo and redo operations.
    /// </summary>
    public class UndoRedoSystem
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        /// <summary>
        /// Adds an undo command to the system.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void AddUndo(ICommand command)
        {
            this.undoStack.Push(command);
            this.redoStack.Clear();
        }

        /// <summary>
        /// Performs an undo operation.
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count > 0)
            {
                ICommand command = this.undoStack.Pop();
                command.Undo();
                this.redoStack.Push(command);
            }
        }

        /// <summary>
        /// Performs a redo operation.
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count > 0)
            {
                ICommand command = this.redoStack.Pop();
                command.Execute();
                this.undoStack.Push(command);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an undo operation can be performed.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return this.undoStack.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a redo operation can be performed.
        /// </summary>
        public bool CanRedo => this.redoStack.Count > 0;

        /// <summary>
        /// Gets the description of the next undo operation.
        /// </summary>
        public string UndoDescription => this.undoStack.Count > 0 ? this.undoStack.Peek().Description : "";

        /// <summary>
        /// Gets the description of the next redo operation.
        /// </summary>
        public string RedoDescription => redoStack.Count > 0 ? redoStack.Peek().Description : "";
    }

    /// <summary>
    /// Represents a command that can be executed, undone, and described.
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }

    /// <summary>
    /// Represents a command to change the value of a cell in the spreadsheet.
    /// This command can be executed, undone, and provides a description of the action.
    /// </summary>
    public class ChangeCellValueCommand : ICommand
    {
        private Cell cell;
        private string oldValue;
        private string newValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeCellValueCommand"/> class.
        /// </summary>
        /// <param name="cell">The cell whose value is to be changed.</param>
        /// <param name="oldValue">The original value of the cell.</param>
        /// <param name="newValue">The new value to be set for the cell.</param>
        public ChangeCellValueCommand(Cell cell, string oldValue, string newValue)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Executes the command, changing the cell's value to the new value.
        /// </summary>
        public void Execute() => this.cell.Text = this.newValue;

        /// <summary>
        /// Undoes the command, reverting the cell's value to the old value.
        /// </summary>
        public void Undo() => this.cell.Text = this.oldValue;

        /// <summary>
        /// Gets a description of the command, including the cell reference and the action performed.
        /// </summary>
        public string Description
        {
            get
            {
                return $"Change cell {(char)('A' + cell.ColumnIndex)}{cell.RowIndex + 1} value";
            }
        }
    }


    /// <summary>
    /// Represents a command to change the background color of a cell.
    /// </summary>
    public class ChangeCellColorCommand : ICommand
    {
        private Cell cell;
        private uint oldColor;
        private uint newColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeCellColorCommand"/> class.
        /// </summary>
        /// <param name="cell">The cell to change.</param>
        /// <param name="oldColor">The old background color of the cell.</param>
        /// <param name="newColor">The new background color of the cell.</param>
        public ChangeCellColorCommand(Cell cell, uint oldColor, uint newColor)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
            this.oldColor = oldColor;
            this.newColor = newColor;
        }

        /// <summary>
        /// Executes the command, changing the cell's background color to the new color.
        /// </summary>
        public void Execute()
        {
            this.cell.BGColor = this.newColor;
            Console.WriteLine($"Executing color change: {this.oldColor:X8} -> {this.newColor:X8}"); // Debug output
        }

        /// <summary>
        /// Undoes the command, reverting the cell's background color to the old color.
        /// </summary>
        public void Undo()
        {
            this.cell.BGColor = this.oldColor;
            Console.WriteLine($"Undoing color change: {this.newColor:X8} -> {this.oldColor:X8}"); // Debug output
        }

        /// <summary>
        /// Gets a description of the command.
        /// </summary>
        public string Description
        {
            get
            {
                return $"Change cell {(char)('A' + this.cell.ColumnIndex)}{this.cell.RowIndex + 1} color";
            }
        }
    }
}