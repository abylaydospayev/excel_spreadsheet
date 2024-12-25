using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SpreadsheetEngine;

namespace Test_unit
{
    /// <summary>
    /// Unit tests for the <see cref="Spreadsheet"/> class.
    /// </summary>
    public class SpreadsheetTests
    {
        private Spreadsheet spreadsheet;

        [SetUp]
        public void Setup()
        {
            spreadsheet = new Spreadsheet(10, 10);
        }

        /// <summary>
        /// Tests the initialization of the spreadsheet with correct dimensions.
        /// </summary>
        [Test]
        public void TestSpreadsheetInitialization()
        {
            Assert.AreEqual(10, spreadsheet.RowCount);
            Assert.AreEqual(10, spreadsheet.ColumnCount);
            Assert.IsNotNull(spreadsheet.GetCell(5, 5));
        }

        /// <summary>
        /// Tests the retrieval of cells within the spreadsheet bounds.
        /// </summary>
        [Test]
        public void TestGetCell_WithinBounds()
        {
            var cell = spreadsheet.GetCell(0, 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(0, cell.RowIndex);
            Assert.AreEqual(0, cell.ColumnIndex);
        }

        /// <summary>
        /// Tests the retrieval of cells outside the spreadsheet bounds.
        /// </summary>
        [Test]
        public void TestGetCell_OutOfBounds()
        {
            Assert.IsNull(spreadsheet.GetCell(-1, 0));
            Assert.IsNull(spreadsheet.GetCell(0, -1));
            Assert.IsNull(spreadsheet.GetCell(10, 0));
            Assert.IsNull(spreadsheet.GetCell(0, 10));
        }

        /// <summary>
        /// Tests updating a cell's text and verifying its value.
        /// </summary>
        [Test]
        public void TestCellTextUpdate()
        {
            var cell = spreadsheet.GetCell(0, 0);
            cell.Text = "Test";
            Assert.AreEqual("Test", cell.Text);
            Assert.AreEqual("Test", cell.Value);
        }

        /// <summary>
        /// Tests a simple formula evaluation in a cell.
        /// </summary>
        [Test]
        public void TestSimpleFormulaEvaluation()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(0, 1).Text = "10";
            spreadsheet.GetCell(0, 2).Text = "=A1+B1";
            Assert.AreEqual("15", spreadsheet.GetCell(0, 2).Value);
        }

        /// <summary>
        /// Tests a complex formula evaluation involving multiple cells.
        /// </summary>
        [Test]
        public void TestComplexFormulaEvaluation()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(0, 1).Text = "3";
            spreadsheet.GetCell(0, 2).Text = "2";
            spreadsheet.GetCell(0, 3).Text = "=(A1+B1)*C1";
            Assert.AreEqual("16", spreadsheet.GetCell(0, 3).Value);
        }





        /// <summary>
        /// Tests the handling of formulas with parentheses.
        /// </summary>
        [Test]
        public void TestFormulaWithParentheses()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(0, 1).Text = "3";
            spreadsheet.GetCell(0, 2).Text = "=(A1+B1)*(A1-B1)";
            Assert.AreEqual("16", spreadsheet.GetCell(0, 2).Value);
        }


        /// <summary>
        /// Tests the handling of very small numbers in formulas.
        /// </summary>
        [Test]
        public void TestSmallNumberHandling()
        {
            spreadsheet.GetCell(0, 0).Text = double.Epsilon.ToString();
            spreadsheet.GetCell(0, 1).Text = "=A1*2";
            Assert.AreEqual((2 * double.Epsilon).ToString(), spreadsheet.GetCell(0, 1).Value);
        }



        /// <summary>
        /// Tests the handling of whitespace in formulas.
        /// </summary>
        [Test]
        public void TestWhitespaceInFormula()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(0, 1).Text = "3";
            spreadsheet.GetCell(0, 2).Text = "=  A1  +  B1  ";
            Assert.AreEqual("8", spreadsheet.GetCell(0, 2).Value);
        }

        [Test]
        public void TestSimpleCellReference()
        {
            // Arrange
            var spreadsheet = new Spreadsheet(10, 10);
            spreadsheet.GetCell(0, 0).Text = "5";

            // Act
            spreadsheet.GetCell(1, 1).Text = "=A1";

            // Assert
            Assert.AreEqual("5", spreadsheet.GetCell(1, 1).Value);
        }

       
        [Test]
        public void TestCircularReferenceChain()
        {
            // Arrange
            var spreadsheet = new Spreadsheet(10, 10);

            // Act
            spreadsheet.GetCell(0, 0).Text = "=B1"; // A1 = B1
            spreadsheet.GetCell(0, 1).Text = "=C1"; // B1 = C1
            spreadsheet.GetCell(0, 2).Text = "=A1"; // C1 = A1

            // Assert
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 0).Value);
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 1).Value);
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 2).Value);
        }
    }
}