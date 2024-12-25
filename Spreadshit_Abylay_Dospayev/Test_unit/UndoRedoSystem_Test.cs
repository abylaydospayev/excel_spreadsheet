using NUnit.Framework;
using System;

namespace SpreadsheetEngine.Tests
{
    [TestFixture]
    public class UndoRedoSystem_Test
    {
        private UndoRedoSystem undoRedoSystem;
        private Spreadsheet spreadsheet;

        [SetUp]
        public void Setup()
        {
            undoRedoSystem = new UndoRedoSystem();
            spreadsheet = new Spreadsheet(10, 10);
        }

        [Test]
        public void TestUndoOnEmptyStack()
        {
            Assert.DoesNotThrow(() => undoRedoSystem.Undo());
            Assert.IsFalse(undoRedoSystem.CanUndo);
        }

        [Test]
        public void TestRedoOnEmptyStack()
        {
            Assert.DoesNotThrow(() => undoRedoSystem.Redo());
            Assert.IsFalse(undoRedoSystem.CanRedo);
        }

        [Test]
        public void TestUndoDescriptionOnEmptyStack()
        {
            Assert.AreEqual("", undoRedoSystem.UndoDescription);
        }

        [Test]
        public void TestRedoDescriptionOnEmptyStack()
        {
            Assert.AreEqual("", undoRedoSystem.RedoDescription);
        }

        [Test]
        public void TestChangeCellValueCommandWithNullCell()
        {
            Assert.Throws<ArgumentNullException>(() => new ChangeCellValueCommand(null, "old", "new"));
        }

        [Test]
        public void TestChangeCellColorCommandWithNullCell()
        {
            Assert.Throws<ArgumentNullException>(() => new ChangeCellColorCommand(null, 0, 1));
        }

        [Test]
        public void TestChangeCellValueCommandWithEmptyStrings()
        {
            var cell = spreadsheet.GetCell(0, 0);
            var command = new ChangeCellValueCommand(cell, "", "");
            undoRedoSystem.AddUndo(command);
            Assert.DoesNotThrow(() => undoRedoSystem.Undo());
            Assert.DoesNotThrow(() => undoRedoSystem.Redo());
        }

        [Test]
        public void TestChangeCellColorCommandWithSameColor()
        {
            var cell = spreadsheet.GetCell(0, 0);
            var command = new ChangeCellColorCommand(cell, 0xFFFFFFFF, 0xFFFFFFFF);
            undoRedoSystem.AddUndo(command);
            Assert.DoesNotThrow(() => undoRedoSystem.Undo());
            Assert.DoesNotThrow(() => undoRedoSystem.Redo());
        }

        [Test]
        public void TestUndoRedoWithMaximumStackSize()
        {
            // Assuming a reasonable maximum stack size, e.g., 1000
            for (int i = 0; i < 1000; i++)
            {
                var cell = spreadsheet.GetCell(0, 0);
                var command = new ChangeCellValueCommand(cell, i.ToString(), (i + 1).ToString());
                undoRedoSystem.AddUndo(command);
            }

            for (int i = 0; i < 1000; i++)
            {
                Assert.DoesNotThrow(() => undoRedoSystem.Undo());
            }

            Assert.IsFalse(undoRedoSystem.CanUndo);
        }

        [Test]
        public void TestCellReferenceAtEdgeOfSpreadsheet()
        {
            var cell = spreadsheet.GetCell(9, 9);
            var command = new ChangeCellValueCommand(cell, "old", "new");
            Assert.DoesNotThrow(() => command.Execute());
            Assert.AreEqual("Change cell J10 value", command.Description);
        }

        [Test]
        public void TestClearingRedoStackAfterNewUndo()
        {
            var cell = spreadsheet.GetCell(0, 0);
            var command1 = new ChangeCellValueCommand(cell, "old", "new");
            var command2 = new ChangeCellValueCommand(cell, "new", "newer");

            undoRedoSystem.AddUndo(command1);
            undoRedoSystem.Undo();
            Assert.IsTrue(undoRedoSystem.CanRedo);

            undoRedoSystem.AddUndo(command2);
            Assert.IsFalse(undoRedoSystem.CanRedo);
        }
    }
}