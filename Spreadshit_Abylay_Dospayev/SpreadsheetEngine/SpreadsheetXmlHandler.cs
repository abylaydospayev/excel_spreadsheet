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
    /// Handles XML operations for saving and loading spreadsheet data.
    /// </summary>
    public class SpreadsheetXmlHandler
    {
        /// <summary>
        /// Saves the spreadsheet data to an XML format.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet to save.</param>
        /// <param name="stream">The stream to save the XML data to.</param>
        public void SaveSpreadsheet(Spreadsheet spreadsheet, Stream stream)
        {
            XDocument doc = new XDocument(
                new XElement("spreadsheet",
                    from row in Enumerable.Range(0, spreadsheet.RowCount)
                    from col in Enumerable.Range(0, spreadsheet.ColumnCount)
                    let cell = spreadsheet.GetCell(row, col)
                    where cell.Text != string.Empty || cell.BGColor != 0xFFFFFFFF
                    select new XElement("cell",
                        new XAttribute("name", $"{(char)('A' + col)}{row + 1}"),
                        new XElement("bgcolor", cell.BGColor.ToString("X8")),
                        new XElement("text", cell.Text)
                    )
                )
            );

            doc.Save(stream);
        }

        /// <summary>
        /// Loads spreadsheet data from an XML format.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet to load data into.</param>
        /// <param name="stream">The stream containing the XML data to load.</param>
        public void LoadSpreadsheet(Spreadsheet spreadsheet, Stream stream)
        {
            spreadsheet.ClearAll(); // Clear existing data
            spreadsheet.ClearUndoRedo(); // Clear undo/redo stacks

            XDocument doc = XDocument.Load(stream);

            foreach (var cellElement in doc.Root.Elements("cell"))
            {
                string cellName = cellElement.Attribute("name").Value;
                int col = cellName[0] - 'A';
                int row = int.Parse(cellName.Substring(1)) - 1;

                Cell cell = spreadsheet.GetCell(row, col);

                var bgColorElement = cellElement.Element("bgcolor");
                if (bgColorElement != null)
                {
                    cell.BGColor = uint.Parse(bgColorElement.Value, System.Globalization.NumberStyles.HexNumber);
                }

                var textElement = cellElement.Element("text");
                if (textElement != null)
                {
                    cell.Text = textElement.Value;
                }
            }

            spreadsheet.EvaluateAllFormulas();
        }
    }
}