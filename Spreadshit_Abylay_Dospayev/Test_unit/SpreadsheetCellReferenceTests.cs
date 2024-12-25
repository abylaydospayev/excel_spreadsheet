using SpreadsheetEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_unit
{
    /// <summary>
    /// Test fixture for spreadsheet cell reference functionality.
    /// </summary>
    [TestFixture]
    public class SpreadsheetCellReferenceTests
    {
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            spreadsheet = new Spreadsheet(10, 10); // Create a 10x10 spreadsheet
        }

        /// <summary>
        /// Tests a simple cell reference.
        /// </summary>
        [Test]
        public void TestSimpleCellReference()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(1, 1).Text = "=A1";

            Assert.AreEqual("5", spreadsheet.GetCell(1, 1).Value);
        }

        /// <summary>
        /// Tests a formula with multiple cell references.
        /// </summary>
        [Test]
        public void TestFormulaWithMultipleCellReferences()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(0, 1).Text = "10";
            spreadsheet.GetCell(1, 0).Text = "=A1+B1";

            Assert.AreEqual("15", spreadsheet.GetCell(1, 0).Value);
        }

        /// <summary>
        /// Tests detection of self-reference in a cell.
        /// </summary>
        [Test]
        public void TestSelfReference()
        {
            spreadsheet.GetCell(0, 0).Text = "=A1";

            Assert.AreEqual("Error: Self-reference detected", spreadsheet.GetCell(0, 0).Value);
        }

        /// <summary>
        /// Tests detection of direct circular reference between cells.
        /// </summary>
        [Test]
        public void TestDirectCircularReference()
        {
            spreadsheet.GetCell(0, 0).Text = "=B1";
            spreadsheet.GetCell(0, 1).Text = "=A1";

            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 0).Value);
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 1).Value);
        }

        /// <summary>
        /// Tests detection of indirect circular reference among cells.
        /// </summary>
        [Test]
        public void TestIndirectCircularReference()
        {
            spreadsheet.GetCell(0, 0).Text = "=B1";
            spreadsheet.GetCell(0, 1).Text = "=C1";
            spreadsheet.GetCell(0, 2).Text = "=A1";

            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 0).Value);
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 1).Value);
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 2).Value);
        }

        /// <summary>
        /// Tests a non-circular chain of cell references.
        /// </summary>
        [Test]
        public void TestNonCircularChainOfReferences()
        {
            spreadsheet.GetCell(0, 0).Text = "5";
            spreadsheet.GetCell(0, 1).Text = "=A1+1";
            spreadsheet.GetCell(0, 2).Text = "=B1+1";
            spreadsheet.GetCell(0, 3).Text = "=C1+1";

            Assert.AreEqual("5", spreadsheet.GetCell(0, 0).Value);
            Assert.AreEqual("6", spreadsheet.GetCell(0, 1).Value);
            Assert.AreEqual("7", spreadsheet.GetCell(0, 2).Value);
            Assert.AreEqual("8", spreadsheet.GetCell(0, 3).Value);
        }

        /// <summary>
        /// Tests referencing an empty cell.
        /// </summary>
        [Test]
        public void TestReferenceToEmptyCell()
        {
            spreadsheet.GetCell(0, 1).Text = "=A1";

            Assert.AreEqual("0", spreadsheet.GetCell(0, 1).Value);
        }

        /// <summary>
        /// Tests referencing a non-existent cell.
        /// </summary>
        [Test]
        public void TestReferenceToNonExistentCell()
        {
            spreadsheet.GetCell(0, 0).Text = "=Z100";

            Assert.AreEqual("Error: Invalid cell reference 'Z100'", spreadsheet.GetCell(0, 0).Value);
        }

        /// <summary>
        /// Tests breaking a circular reference and updating cell values.
        /// </summary>
        [Test]
        public void TestBreakingCircularReference()
        {
            spreadsheet.GetCell(0, 0).Text = "=B1";
            spreadsheet.GetCell(0, 1).Text = "=A1";

            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 0).Value);
            Assert.AreEqual("Error: Circular reference detected", spreadsheet.GetCell(0, 1).Value);

            spreadsheet.GetCell(0, 1).Text = "5";

            Assert.AreEqual("5", spreadsheet.GetCell(0, 0).Value);
            Assert.AreEqual("5", spreadsheet.GetCell(0, 1).Value);
        }
    }
}