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
    /// Represents a default implementation of a cell in the spreadsheet.
    /// </summary>
    public class DefaultCell : Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCell"/> class.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        public DefaultCell(int rowIndex, int columnIndex) : base(rowIndex, columnIndex) { }
    }

}