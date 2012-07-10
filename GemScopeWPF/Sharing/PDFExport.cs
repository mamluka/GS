using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemScopeWPF.Repository;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
namespace GemScopeWPF.Sharing
{
    public class PDFExport
    {
        
        public string CreatePDFFile(Stone stone)
        {
            var filename = stone.FullFilePath+".pdf";

            var document = this.CreateDocument();

            DefineStyles();
            PlaceImage(stone);

            var pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            var savedlg = new SaveFileDialog();
            savedlg.DefaultExt = ".pdf";
            
           // Save the PDF document...
        	if (savedlg.ShowDialog() != true)
        		return String.Empty;

        	filename = savedlg.FileName;
        	pdfRenderer.Save(filename);
        	Process.Start(filename);
        	return filename;
           
        }

        public string CreateRawPDFinTemp(Stone stone)
        {
            
            string filename = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(stone.Filename) + ".pdf");
                

            Document document = CreateDocument();

            DefineStyles();
            PlaceImage(stone);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

         
           pdfRenderer.Save(filename);

           return File.Exists(filename) ? filename : String.Empty;

        }


        private Document CreateDocument() {

              this.document = new Document();
              this.document.Info.Title = "A sample invoice";
              this.document.Info.Subject = "Demonstrates how to create an invoice.";
              this.document.Info.Author = "Stefan Lange";

              return document;
        }

        private void DefineStyles()
            {
              // Get the predefined style Normal.
              Style style = this.document.Styles["Normal"];
              // Because all styles are derived from Normal, the next line changes the 
              // font of the whole document. Or, more exactly, it changes the font of
              // all styles and paragraphs that do not redefine the font.
              style.Font.Name = "Verdana";

              style = this.document.Styles[StyleNames.Header];
              style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

              style = this.document.Styles[StyleNames.Footer];
              style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

              // Create a new style called Table based on style Normal
              style = this.document.Styles.AddStyle("Table", "Normal");
              style.Font.Name = "Verdana";
              style.Font.Name = "Times New Roman";
              style.Font.Size =12;

              // Create a new style called Reference based on style Normal
              style = this.document.Styles.AddStyle("Reference", "Normal");
              style.ParagraphFormat.SpaceBefore = "5mm";
              style.ParagraphFormat.SpaceAfter = "5mm";
              style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
            }
        private void PlaceImage(Stone stone) {

            Section section = this.document.AddSection();

            //Table mainTable  = section.AddTable();
            //mainTable.Style = "Table";
            //mainTable.Borders.Color = new Color(81, 125, 192);
            //mainTable.Borders.Width = 0.25;
            //mainTable.Format.SpaceAfter = "5mm";
            

            //Column column = mainTable.AddColumn(Unit.FromMillimeter(170));
            //column.Format.Alignment = ParagraphAlignment.Center;

            ////column = mainTable.AddColumn("7cm");
            ////column.Format.Alignment = ParagraphAlignment.Center;

            //Row row2 = mainTable.AddRow();
            //row2.Format.Alignment = ParagraphAlignment.Center;
            //row2.Format.Font.Bold = true;

            Image image =  section.AddImage(stone.FullFilePath);
            image.Width = Unit.FromMillimeter(170);
            image.LockAspectRatio = true;
            

          
            //write the properties table

            TextFrame frame = section.AddTextFrame();

            frame.Left = ShapePosition.Center;
            frame.Top = "1cm";

            this.infopartTable = frame.AddTable();
            
           // this.infopartTable.Format.SpaceBefore = "10mm";
            this.infopartTable.Style = "Table";
            this.infopartTable.Borders.Color = new Color(81, 125, 192);
            this.infopartTable.Borders.Width = 0.25;
            this.infopartTable.BottomPadding = "2mm";
            this.infopartTable.LeftPadding = "2mm";
            this.infopartTable.RightPadding = "2mm";
            this.infopartTable.TopPadding = "2mm";

            Column column2 = this.infopartTable.AddColumn("3cm");
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.infopartTable.AddColumn("2cm");
            column2.Format.Alignment = ParagraphAlignment.Center;

            foreach (var infopart in stone.InfoList)
            {
                  Row row = this.infopartTable.AddRow();

                  row.Cells[0].AddParagraph(infopart.TitleForReport);
                  row.Cells[0].Format.Font.Bold = true;
                  row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                  row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                  



                  row.Cells[1].AddParagraph(infopart.Value);
                  row.Cells[1].Format.Font.Bold = false;
                  row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                  row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
               
            }




           


    }


            private Document document;
            private Table infopartTable;
    }
}
