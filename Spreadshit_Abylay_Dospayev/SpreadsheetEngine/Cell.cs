// <copyright file="Cell.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SpreadsheetEngine
{
    using System.ComponentModel;

    /// <summary>
    /// Represents an abstract base class for a cell in a spreadsheet.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the row index of the cell.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets the column index of the cell.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// The text content of the cell.
        /// </summary>
        protected string text;

        /// <summary>
        /// Gets or sets the text content of the cell.
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged(nameof(this.Text));
                }
            }
        }

        /// <summary>
        /// The evaluated value of the cell.
        /// </summary>
        protected string value;

        /// <summary>
        /// Gets the evaluated value of the cell.
        /// </summary>
        public string Value => this.value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        protected Cell(int rowIndex, int columnIndex)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
            this.text = string.Empty;
            this.value = string.Empty;
        }

        /// <summary>
        /// Sets the value of the cell.
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        internal void SetValue(string newValue)
        {
            if (this.value != newValue)
            {
                this.value = newValue;
                this.OnPropertyChanged(nameof(Value));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The background color of the cell, stored as a 32-bit unsigned integer.
        /// </summary>
        private uint bgColor = 0xFFFFFFFF; // Default white color

        /// <summary>
        /// Gets or sets the background color of the cell.
        /// </summary>
        public uint BGColor
        {
            get => this.bgColor;
            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.OnPropertyChanged(nameof(this.BGColor));
                    Console.WriteLine($"BGColor changed to {this.bgColor:X8}"); // Debug output
                }
            }
        }
    }
}