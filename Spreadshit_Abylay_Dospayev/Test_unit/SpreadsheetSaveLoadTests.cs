using NUnit.Framework;
using SpreadsheetEngine;
using System.IO;
using System.Xml.Linq;

public class SpreadsheetSaveLoadTests
{
    private Spreadsheet spreadsheet;
    private SpreadsheetXmlHandler xmlHandler;

    [SetUp]
    public void Setup()
    {
        spreadsheet = new Spreadsheet(10, 10);
        xmlHandler = new SpreadsheetXmlHandler();
    }

    /// <summary>
    /// Tests saving an empty spreadsheet.
    /// </summary>
    [Test]
    public void TestSaveEmptySpreadsheet()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            xmlHandler.SaveSpreadsheet(spreadsheet, stream);
            stream.Position = 0;
            XDocument doc = XDocument.Load(stream);
            Assert.AreEqual(0, doc.Root.Elements("cell").Count());
        }
    }

    /// <summary>
    /// Tests saving a spreadsheet with one cell modified.
    /// </summary>
    [Test]
    public void TestSaveSpreadsheetWithOneCell()
    {
        spreadsheet.GetCell(0, 0).Text = "Test";
        using (MemoryStream stream = new MemoryStream())
        {
            xmlHandler.SaveSpreadsheet(spreadsheet, stream);
            stream.Position = 0;
            XDocument doc = XDocument.Load(stream);
            Assert.AreEqual(1, doc.Root.Elements("cell").Count());
            Assert.AreEqual("Test", doc.Root.Element("cell").Element("text").Value);
        }
    }

    /// <summary>
    /// Tests saving a spreadsheet with multiple cells modified.
    /// </summary>
    [Test]
    public void TestSaveSpreadsheetWithMultipleCells()
    {
        spreadsheet.GetCell(0, 0).Text = "Test1";
        spreadsheet.GetCell(1, 1).Text = "Test2";
        spreadsheet.GetCell(2, 2).BGColor = 0xFF0000FF; // Red color

        using (MemoryStream stream = new MemoryStream())
        {
            xmlHandler.SaveSpreadsheet(spreadsheet, stream);
            stream.Position = 0;
            XDocument doc = XDocument.Load(stream);
            Assert.AreEqual(3, doc.Root.Elements("cell").Count());
        }
    }

    /// <summary>
    /// Tests loading an empty spreadsheet.
    /// </summary>
    [Test]
    public void TestLoadEmptySpreadsheet()
    {
        XDocument doc = new XDocument(new XElement("spreadsheet"));
        using (MemoryStream stream = new MemoryStream())
        {
            doc.Save(stream);
            stream.Position = 0;
            xmlHandler.LoadSpreadsheet(spreadsheet, stream);
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Assert.AreEqual(string.Empty, spreadsheet.GetCell(i, j).Text);
                Assert.AreEqual(0xFFFFFFFF, spreadsheet.GetCell(i, j).BGColor);
            }
        }
    }

    /// <summary>
    /// Tests loading a spreadsheet with one cell.
    /// </summary>
    [Test]
    public void TestLoadSpreadsheetWithOneCell()
    {
        XDocument doc = new XDocument(
            new XElement("spreadsheet",
                new XElement("cell",
                    new XAttribute("name", "A1"),
                    new XElement("text", "Test"),
                    new XElement("bgcolor", "FF0000FF")
                )
            )
        );

        using (MemoryStream stream = new MemoryStream())
        {
            doc.Save(stream);
            stream.Position = 0;
            xmlHandler.LoadSpreadsheet(spreadsheet, stream);
        }

        Assert.AreEqual("Test", spreadsheet.GetCell(0, 0).Text);
        Assert.AreEqual(0xFF0000FF, spreadsheet.GetCell(0, 0).BGColor);
    }

    /// <summary>
    /// Tests loading a spreadsheet with multiple cells.
    /// </summary>
    [Test]
    public void TestLoadSpreadsheetWithMultipleCells()
    {
        XDocument doc = new XDocument(
            new XElement("spreadsheet",
                new XElement("cell",
                    new XAttribute("name", "A1"),
                    new XElement("text", "Test1")
                ),
                new XElement("cell",
                    new XAttribute("name", "B2"),
                    new XElement("text", "Test2")
                ),
                new XElement("cell",
                    new XAttribute("name", "C3"),
                    new XElement("bgcolor", "FF0000FF")
                )
            )
        );

        using (MemoryStream stream = new MemoryStream())
        {
            doc.Save(stream);
            stream.Position = 0;
            xmlHandler.LoadSpreadsheet(spreadsheet, stream);
        }

        Assert.AreEqual("Test1", spreadsheet.GetCell(0, 0).Text);
        Assert.AreEqual("Test2", spreadsheet.GetCell(1, 1).Text);
        Assert.AreEqual(0xFF0000FF, spreadsheet.GetCell(2, 2).BGColor);
    }

    /// <summary>
    /// Tests loading a spreadsheet with a formula.
    /// </summary>
    [Test]
    public void TestLoadSpreadsheetWithFormula()
    {
        XDocument doc = new XDocument(
            new XElement("spreadsheet",
                new XElement("cell",
                    new XAttribute("name", "A1"),
                    new XElement("text", "5")
                ),
                new XElement("cell",
                    new XAttribute("name", "B1"),
                    new XElement("text", "=A1+10")
                )
            )
        );

        using (MemoryStream stream = new MemoryStream())
        {
            doc.Save(stream);
            stream.Position = 0;
            xmlHandler.LoadSpreadsheet(spreadsheet, stream);
        }

        Assert.AreEqual("5", spreadsheet.GetCell(0, 0).Text);
        Assert.AreEqual("=A1+10", spreadsheet.GetCell(0, 1).Text);
        Assert.AreEqual("15", spreadsheet.GetCell(0, 1).Value);
    }

    /// <summary>
    /// Tests loading a spreadsheet with invalid XML (error case).
    /// </summary>
    [Test]
    public void TestLoadSpreadsheetWithInvalidXml()
    {
        string invalidXml = "<spreadsheet><cell><invalid></spreadsheet>";
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(invalidXml)))
        {
            Assert.Throws<System.Xml.XmlException>(() => xmlHandler.LoadSpreadsheet(spreadsheet, stream));
        }
    }

}