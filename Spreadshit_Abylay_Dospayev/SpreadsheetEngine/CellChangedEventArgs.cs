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
    /// Event arguments for cell property changes.
    /// </summary>
    public class CellChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the cell that has changed.
        /// </summary>
        public Cell ChangedCell { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedCell">The cell that has changed.</param>
        public CellChangedEventArgs(Cell changedCell)
        {
            this.ChangedCell = changedCell;
        }
    }

}