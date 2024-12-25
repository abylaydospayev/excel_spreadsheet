// <copyright file="Form1.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Abylay_Dospayev
{
    using System.Windows.Forms;
    using SpreadsheetEngine;

    /*
     * Author: Abylay Dospayev
     * WSU ID: 011858661
     * 
     * Main form for the spreadsheet application.
     * This form initializes the spreadsheet and manages user interactions.
     */
    public partial class Form1 : Form
    {
        private Spreadsheet spreadsheet;
        private SpreadsheetXmlHandler xmlHandler = new SpreadsheetXmlHandler();

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            this.InitializeSpreadsheet();
            this.dataGridView1.CellBeginEdit += this.dataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.dataGridView1_CellEndEdit;
        }

        /// <summary>
        /// Initializes the spreadsheet and sets up event handlers.
        /// </summary>
        private void InitializeSpreadsheet()
        {
            this.spreadsheet = new Spreadsheet(50, 26);
            this.spreadsheet.CellPropertyChanged += this.Spreadsheet_CellPropertyChanged;
            this.InitializeDataGridView();
            this.InitializeUndoRedoButtons();
        }

        /// <summary>
        /// Initializes the DataGridView with columns and rows.
        /// </summary>
        private void InitializeDataGridView()
        {
            this.dataGridView1.ColumnCount = 26;
            for (int i = 0; i < 26; i++)
            {
                this.dataGridView1.Columns[i].HeaderText = ((char)('A' + i)).ToString();
            }

            for (int i = 0; i < 50; i++)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }

        /// <summary>
        /// Initializes the Undo and Redo buttons.
        /// </summary>
        private void InitializeUndoRedoButtons()
        {
            Button undoButton = new Button
            {
                Text = "Undo",
                Location = new Point(170, dataGridView1.Bottom + 10),
                Size = new Size(75, 30)
            };
            undoButton.Click += this.undoTextChangeToolStripMenuItem_Click;
            this.Controls.Add(undoButton);

            Button redoButton = new Button
            {
                Text = "Redo",
                Location = new Point(250, dataGridView1.Bottom + 10),
                Size = new Size(75, 30),
            };
            redoButton.Click += this.redoTextChangeToolStripMenuItem_Click;
            this.Controls.Add(redoButton);

            this.UpdateUndoRedoMenuItems();
        }

        /// <summary>
        /// Updates the Undo and Redo menu items based on their availability.
        /// </summary>
        private void UpdateUndoRedoMenuItems()
        {
            this.undoTextChangeToolStripMenuItem.Enabled = this.spreadsheet.CanUndo;
            this.redoTextChangeToolStripMenuItem.Enabled = this.spreadsheet.CanRedo;

            this.undoTextChangeToolStripMenuItem.Text = this.spreadsheet.CanUndo ? $"Undo ({this.spreadsheet.UndoDescription})" : "Undo";
            this.redoTextChangeToolStripMenuItem.Text = this.spreadsheet.CanRedo ? $"Redo ({this.spreadsheet.RedoDescription})" : "Redo";
        }




        /// <summary>
        /// Event handler for cell property changes in the spreadsheet.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments containing the changed cell information.</param>
        private void Spreadsheet_CellPropertyChanged(object sender, CellChangedEventArgs e)
        {
            // If e.ChangedCell is null, refresh all cells
            if (e.ChangedCell == null)
            {
                this.dataGridView1.Refresh();
                return;
            }

            var cell = e.ChangedCell;
            var dataGridViewCell = this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex];
            dataGridViewCell.Value = cell.Value;

            // Update background color
            dataGridViewCell.Style.BackColor = System.Drawing.Color.FromArgb((int)cell.BGColor);

            Console.WriteLine($"Updated cell ({cell.RowIndex}, {cell.ColumnIndex}) color to {cell.BGColor:X8}"); // Debug output

            // Force a refresh of the cell
            this.dataGridView1.InvalidateCell(dataGridViewCell);
        }

        /// <summary>
        /// Updates the UI representation of a cell.
        /// </summary>
        /// <param name="cell">The cell to update in the UI.</param>
        private void UpdateCellUI(Cell cell)
        {
            var dataGridViewCell = this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex];
            dataGridViewCell.Value = cell.Value;
            dataGridViewCell.Style.BackColor = System.Drawing.Color.FromArgb((int)cell.BGColor);
            this.dataGridView1.InvalidateCell(dataGridViewCell);
        }

        /// <summary>
        /// Event handler for when cell editing begins.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments containing cell information.</param>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = cell.Text;
        }

        /// <summary>
        /// Event handler for when cell editing ends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments containing cell information.</param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            string oldValue = cell.Text;
            string newValue = this.dataGridView1[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? string.Empty;

            if (oldValue != newValue)
            {
                var command = new ChangeCellValueCommand(cell, oldValue, newValue);
                this.spreadsheet.AddUndo(command);
                command.Execute();
                this.UpdateUndoRedoMenuItems();
            }

            this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = cell.Value;
        }

        /// <summary>
        /// Event handler for cell content click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments containing cell information.</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// <summary>
        /// Event handler for button click events.
        /// Sets random cells to "I love C!" and updates column B cells.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing information about the button click.</param>

        private void button1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            // Set random cells to "I love C#!"
            for (int i = 0; i < 50; i++)
            {
                int randomRow = rand.Next(0, 50);
                int randomCol = rand.Next(0, 26);
                this.spreadsheet.GetCell(randomRow, randomCol).Text = "I love C#!";
            }

            // Set every cell in column B to "This is cell B#"
            for (int row = 0; row < 50; row++)
            {
                this.spreadsheet.GetCell(row, 0).Text = $"This is cell B{row + 1}"; // Column B
            }

        }

        /// <summary>
        /// Event handler for form load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler for undo text change menu item click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void undoTextChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Undo();
            this.UpdateUndoRedoMenuItems();
            this.dataGridView1.Refresh();
            // Update all cells in case multiple cells were affected
            for (int row = 0; row < this.spreadsheet.RowCount; row++)
            {
                for (int col = 0; col < this.spreadsheet.ColumnCount; col++)
                {
                    this.UpdateCellUI(this.spreadsheet.GetCell(row, col));
                }
            }
        }


        private void redoTextChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Redo();
            this.UpdateUndoRedoMenuItems();
            this.dataGridView1.Refresh();
            // Update all cells in case multiple cells were affected
            for (int row = 0; row < this.spreadsheet.RowCount; row++)
            {
                for (int col = 0; col < this.spreadsheet.ColumnCount; col++)
                {
                    UpdateCellUI(this.spreadsheet.GetCell(row, col));
                }
            }
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    uint newColor = (uint)colorDialog.Color.ToArgb();

                    foreach (DataGridViewCell selectedCell in this.dataGridView1.SelectedCells)
                    {
                        var cell = this.spreadsheet.GetCell(selectedCell.RowIndex, selectedCell.ColumnIndex);
                        uint oldColor = cell.BGColor;

                        var command = new ChangeCellColorCommand(cell, oldColor, newColor);
                        this.spreadsheet.AddUndo(command);
                        command.Execute();

                        // Set the color directly on the DataGridViewCell as well
                        selectedCell.Style.BackColor = colorDialog.Color;
                    }

                    // Force a refresh of the entire DataGridView
                    this.dataGridView1.Refresh();
                    this.UpdateUndoRedoMenuItems();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML files (*.xml)|*.xml";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        xmlHandler.SaveSpreadsheet(this.spreadsheet, stream);
                    }
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML files (*.xml)|*.xml";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        xmlHandler.LoadSpreadsheet(this.spreadsheet, stream);
                    }
                    this.dataGridView1.Refresh();
                    this.UpdateUndoRedoMenuItems();
                }
            }
        }
    }
}
