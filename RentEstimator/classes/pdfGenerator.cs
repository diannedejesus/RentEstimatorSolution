using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RentEstimator
{
    internal class pdfGenerator
    {
        private PdfDocument document = new PdfDocument();
        private PdfPage page;
        private XGraphics gfx;
        private XTextFormatter textformatter;

        // Create a font
        private XFont titleFont = new XFont("Calibri Light", 28, XFontStyle.Regular);
        private XFont subTitleFont = new XFont("Calibri", 11, XFontStyle.Italic);
        private XFont bodyTextFont = new XFont("Helvetica", 12);
        private XFont tableTextFont = new XFont("Helvetica", 14);
        private XFont bodySubTextFont = new XFont("Calibri", 9, XFontStyle.Italic);

        private XStringFormat format = new XStringFormat();
        private XBrush brush = XBrushes.Black; //text color
        private XBrush brushgrey = XBrushes.LightGray; //text color
        private XPen xpen = new XPen(XColors.Navy, 0.6); //border corlor
        private int pageLine = 50;
        public int lineBreak = 15;

        private XRect baseContainer;

        public pdfGenerator()
        {
            document.Info.Title = "Created with PDFsharp";
            format.LineAlignment = XLineAlignment.Near;
            format.Alignment = XStringAlignment.Near;
        }

        public void AddPage()
        {
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            textformatter = new XTextFormatter(gfx);
            pageLine = 50;
            baseContainer = new XRect(50, 50, page.Width - 100, bodyTextFont.Height);
        }
        
        public void AddTextPdf(string content, string contentType)
        {
            if(contentType == "title")
            {
                //NOTE:should only be one line
                gfx.DrawString(
                    content,
                    titleFont,
                    brush,
                    new XRect(0, pageLine, page.Width, page.Height),
                    XStringFormats.TopCenter
                 );

                pageLine += titleFont.Height;
            }

            if (contentType == "subtitle")
            {
                gfx.DrawString(
                    content, 
                    subTitleFont, 
                    XBrushes.Gray,
                    new XRect(0, pageLine, page.Width, page.Height),
                    XStringFormats.TopCenter
                );

                pageLine += subTitleFont.Height + lineBreak;
            }

            if(contentType == "paragraph")
            {
                int textHeight = GetSplittedLineCount(gfx, content, bodyTextFont, baseContainer.Width) * bodyTextFont.Height;

                textformatter.DrawString(
                    content,
                    bodyTextFont,
                    brush,
                    new XRect(baseContainer.X, pageLine, baseContainer.Width, textHeight), 
                    format
                );

                pageLine += textHeight + lineBreak;
            }

            if (contentType == "header")
            {
                int textHeight = bodyTextFont.Height;

                textformatter.DrawString(
                    content,
                    tableTextFont,
                    brush,
                    new XRect(baseContainer.X, pageLine, baseContainer.Width, textHeight),
                    format
                );

                pageLine += textHeight;
            }

            if (contentType == "subtext")
            {
                textformatter.DrawString(
                    content,
                    bodySubTextFont,
                    XBrushes.Black,
                    new XRect(baseContainer.X, pageLine, baseContainer.Width, bodySubTextFont.Height), 
                    format
                );

                pageLine += bodySubTextFont.Height + (lineBreak/2);
            }
        }
        
        public void CreateTableLayout(List<string[]> content)
        {
            int tableWidth = 400;
            double centerTable = (page.Width - tableWidth) / 2;
            int tableMargin = 10;
            int cellPadding = 15;

            int tableHeight = (content.Count * bodyTextFont.Height) + (tableMargin * 2) + ((content.Count - 1) * cellPadding);

            var outlinedContainer = new XRect(centerTable, pageLine, tableWidth, tableHeight);
            gfx.DrawRectangle(xpen, outlinedContainer);

            pageLine += tableMargin;

            foreach (var item in content)
            {
                textformatter.DrawString(
                    item[0],
                    bodyTextFont,
                    brush,
                    new XRect(centerTable + tableMargin, pageLine, tableWidth, bodyTextFont.Height),
                    format
                );

                textformatter.DrawString(
                    item[1],
                    bodyTextFont,
                    brush,
                    new XRect(centerTable + (tableWidth - tableMargin - gfx.MeasureString(item[1], bodyTextFont).Width), pageLine, tableWidth, bodyTextFont.Height),
                    format
                );

                pageLine += bodyTextFont.Height + cellPadding;
            }

            pageLine += lineBreak;
        }
        
        public void AddTwoCellLayout(List<string[]> content, bool isHighlighted)
        {
            int tableWidth = 250;
            //double centerTable = (page.Width - tableWidth) / 2;
            int tableMargin = 20;
            int cellPadding = 15;

            //int tableHeight = (content.Count * bodyTextFont.Height) + (tableMargin * 2) + ((content.Count - 1) * cellPadding);

            pageLine += lineBreak * 2;


            var outlinedContainer = new XRect(baseContainer.X, pageLine, tableWidth, tableTextFont.Height);

            foreach (var item in content)
            {

                if(isHighlighted == true)
                {
                    outlinedContainer.Y = pageLine;
                    gfx.DrawRectangle(brushgrey, outlinedContainer);
                }

                textformatter.DrawString(
                    item[0],
                    tableTextFont,
                    brush,
                    new XRect(baseContainer.X, pageLine, tableWidth, tableTextFont.Height),
                    format
                );

                textformatter.DrawString(
                    item[1],
                    tableTextFont,
                    brush,
                    new XRect(baseContainer.X + (tableWidth - tableMargin - gfx.MeasureString(item[1], tableTextFont).Width), pageLine, tableWidth, tableTextFont.Height),
                    format
                );

                pageLine += tableTextFont.Height + cellPadding;
            }

            pageLine += lineBreak;
        }

        public void CreatePDF(string fileName)
        {
            // Save the document...
            string filename = string.IsNullOrEmpty(fileName) ? "HelloWorld.pdf" : fileName;
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }

        /// <summary>
        /// Calculate the number of soft line breaks
        /// </summary>
        private int GetSplittedLineCount(XGraphics gfx, string content, XFont font, double maxWidth)
        {
            //handy function for creating list of string
            Func<string, IList<string>> listFor = val => new List<string> { val };
            // string.IsNullOrEmpty is too long :p
            Func<string, bool> nOe = str => string.IsNullOrEmpty(str);
            // return a space for an empty string (sIe = Space if Empty)
            Func<string, string> sIe = str => nOe(str) ? " " : str;
            // check if we can fit a text in the maxWidth
            Func<string, string, bool> canFitText = (t1, t2) => gfx.MeasureString($"{(nOe(t1) ? "" : $"{t1} ")}{sIe(t2)}", font).Width <= maxWidth;

            Func<IList<string>, string, IList<string>> appendtoLast =
                    (list, val) => list.Take(list.Count - 1)
                                       .Concat(listFor($"{(nOe(list.Last()) ? "" : $"{list.Last()} ")}{sIe(val)}"))
                                       .ToList();

            var splitted = content.Split(' ');

            var lines = splitted.Aggregate(listFor(""),
                    (lfeed, next) => canFitText(lfeed.Last(), next) ? appendtoLast(lfeed, next) : lfeed.Concat(listFor(next)).ToList(),
                    list => list.Count());

            return lines;
        }

        public void AddHeaderImage(int number, string imagePath)
        {
            XImage image;

            Console.WriteLine(imagePath);

            if (XImage.ExistsFile(imagePath))
            {
                image = XImage.FromFile(imagePath);
            }
            else
            {
                Console.WriteLine("Header image not added.");
                //header image is not added
                return;
            }
            

            // Left position in point
            double width = (image.PixelWidth * 72 / image.HorizontalResolution) * 1;
            double height = (image.PixelHeight * 72 / image.HorizontalResolution) * 1;

            double x = (page.Width - width) / 2;
            

            gfx.DrawImage(image, x, 10, width, height);
            pageLine = Convert.ToInt16(height) + 10;
        }

        public void AddFooterText(string[] content)
        {
            XUnit positionY = page.Height - (content.Length * bodyTextFont.Height);

            foreach (string item in content)
            {
                XUnit positionX = (page.Width - gfx.MeasureString(item, bodySubTextFont).Width) / 2;

                textformatter.DrawString(
                    item,
                    bodySubTextFont,
                    XBrushes.Black,
                    new XRect(positionX, positionY, gfx.MeasureString(item, bodySubTextFont).Width, bodySubTextFont.Height),
                    format
                );

                positionY += bodySubTextFont.Height;
            }
        }
    }
}
