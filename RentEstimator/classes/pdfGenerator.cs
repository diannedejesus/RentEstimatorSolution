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
using System.Windows.Input;

namespace RentEstimator
{
    internal class pdfGenerator
    {
        private PdfDocument document = new PdfDocument();
        private PdfPage page;
        private XGraphics gfx;
        private XTextFormatter textformatter;

        // Create a font
        private XFont titleFont = new XFont("Arial", 28, XFontStyleEx.Regular);
        private XFont subTitleFont = new XFont("Arial", 11, XFontStyleEx.Italic);
        private XFont bodyTextFont = new XFont("Arial", 12);
        private XFont tableTextFont = new XFont("Arial", 14);
        private XFont tableTextFontBold = new XFont("Arial", 14, XFontStyleEx.Bold);
        private XFont tableHeaderFontBold = new XFont("Arial", 12, XFontStyleEx.Bold);
        private XFont bodySubTextFont = new XFont("Arial", 9, XFontStyleEx.Italic);

        private XStringFormat format = new XStringFormat();
        private XBrush brush = XBrushes.Black; //text color
        private XBrush brushgrey = XBrushes.LightGray; //text color
        private XBrush brushwhite = XBrushes.White; //text color
        private XPen xpen = new XPen(XColors.Navy, 0.6); //border color
        private XPen xpenblack = new XPen(XColors.Black, 1.5); //border color
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
           
            baseContainer = new XRect(50, 50, page.Width.Value - 100, bodyTextFont.Height);
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
                    new XRect(0, pageLine, page.Width.Value, page.Height.Value),
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
                    new XRect(0, pageLine, page.Width.Value, page.Height.Value),
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
            double centerTable = (page.Width.Value - tableWidth) / 2;
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

        
        public void CreateTableLayout2(Dictionary<string, string[]> content)
        {
            int tableGap = 50;
            int cellPadding = 10;
            double[] textWidths = content["table1"]
                .SelectMany((t, i) => new double[] {
                    gfx.MeasureString(content["table1"][i], bodyTextFont).Width,
                    gfx.MeasureString(content["table2"][i], bodyTextFont).Width
                })
                .ToArray();
            double tableLeftMargin = content["labels"]
                .Select(label => gfx.MeasureString(label, bodyTextFont).Width)
                .Max();
            double tableWidth = textWidths.Max() + 10;
            double centerTable = (page.Width.Value - (tableLeftMargin + tableGap + (tableWidth *2))) / 2;
            int tableHeight = (content["labels"].Length * bodyTextFont.Height) + (5 * 2) + ((content["labels"].Length - 1) * cellPadding);
            double table1Offset = tableLeftMargin + centerTable;
            double table2Offset = table1Offset + tableGap + tableWidth;

            var outlinedContainer = new XRect(table1Offset, pageLine + bodyTextFont.Height + cellPadding, tableWidth, tableHeight);
            gfx.DrawRectangle(xpenblack, outlinedContainer);

            outlinedContainer = new XRect(table2Offset, pageLine + bodyTextFont.Height + cellPadding, tableWidth, tableHeight);
            gfx.DrawRectangle(xpenblack, outlinedContainer);

            pageLine += 5;

            textformatter.DrawString(
                    content["headers"][0],
                    tableHeaderFontBold,
                    brush,
                    new XRect(table1Offset + (tableWidth - gfx.MeasureString(content["headers"][0], tableHeaderFontBold).Width)/2, pageLine, tableWidth, tableHeaderFontBold.Height),
                    format
             );

            textformatter.DrawString(
                    content["headers"][1],
                    tableHeaderFontBold,
                    brush,
                    new XRect(table2Offset + (tableWidth - gfx.MeasureString(content["headers"][1], tableHeaderFontBold).Width) / 2, pageLine, tableWidth, tableHeaderFontBold.Height),
                    format
             );

            pageLine += tableHeaderFontBold.Height + cellPadding;


            for (int i = 0; i < 3; i++)
            {
                textformatter.DrawString(
                    content["labels"][i],
                    bodyTextFont,
                    brush,
                    new XRect(centerTable, pageLine, tableWidth, bodyTextFont.Height),
                    format
                );

                if (i == 0)
                {
                    var outlinedContainer3 = new XRect(table1Offset, pageLine -5, tableWidth, tableHeaderFontBold.Height+5);
                    gfx.DrawRectangle(brush, outlinedContainer3);
                    double centertext = (tableWidth - gfx.MeasureString(content["table1"][i], tableHeaderFontBold).Width)/2;

                    textformatter.DrawString(
                        content["table1"][i],
                        tableHeaderFontBold,
                        brushwhite,
                        new XRect(table1Offset + centertext, pageLine-2, tableWidth, tableHeaderFontBold.Height),
                        format
                    );

                    outlinedContainer3 = new XRect(table2Offset, pageLine - 5, tableWidth, tableHeaderFontBold.Height + 5);
                    gfx.DrawRectangle(brush, outlinedContainer3);
                    centertext = (tableWidth - gfx.MeasureString(content["table2"][i], tableHeaderFontBold).Width) / 2;

                    textformatter.DrawString(
                        content["table2"][i],
                        tableHeaderFontBold,
                        brushwhite,
                        new XRect(table2Offset + centertext, pageLine-2, tableWidth, tableHeaderFontBold.Height),
                        format
                    );

                    pageLine += tableHeaderFontBold.Height + cellPadding -3;
                }
                else
                {
                    double centertext = (tableWidth - gfx.MeasureString(content["table1"][i], bodyTextFont).Width) / 2;

                    textformatter.DrawString(
                        content["table1"][i],
                        bodyTextFont,
                        brush,
                        new XRect(table1Offset + centertext, pageLine, tableWidth, bodyTextFont.Height),
                        format
                    );

                    centertext = (tableWidth - gfx.MeasureString(content["table2"][i], bodyTextFont).Width) / 2;
                    textformatter.DrawString(
                        content["table2"][i],
                        bodyTextFont,
                        brush,
                        new XRect(table2Offset + centertext, pageLine, tableWidth, bodyTextFont.Height),
                        format
                    );

                    pageLine += bodyTextFont.Height + cellPadding;
                }
            }

            pageLine += lineBreak;
        }

        public void AddTwoCellLayout(List<string[]> content, bool isHighlighted)
        {
            int tableWidth = 390;
            double centerTable = (page.Width.Value - tableWidth) / 2;
            int tableMargin = 5;
            int cellPadding = 15;

            //int tableHeight = (content.Count * bodyTextFont.Height) + (tableMargin * 2) + ((content.Count - 1) * cellPadding);

            pageLine += 0;


            var outlinedContainer = new XRect(centerTable, pageLine, tableWidth, tableTextFont.Height);

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
                    new XRect(centerTable + tableMargin, pageLine, tableWidth, tableTextFont.Height),
                    format
                );

                textformatter.DrawString(
                    item[1],
                    tableTextFont,
                    brush,
                    new XRect(centerTable + (tableWidth - tableMargin - gfx.MeasureString(item[1], tableTextFont).Width), pageLine, tableWidth, tableTextFont.Height),
                    format
                );

                pageLine += tableTextFont.Height + cellPadding;
            }

            pageLine += lineBreak;
        }

        public void AddTwoCellLayoutHeader(List<string[]> content, bool isHighlighted)
        {
            int tableWidth = 390;
            double centerTable = (page.Width.Value - tableWidth) / 2;
            int tableMargin = 0;
            int cellPadding = 15;

            //int tableHeight = (content.Count * bodyTextFont.Height) + (tableMargin * 2) + ((content.Count - 1) * cellPadding);

            pageLine += lineBreak * 3;


            var outlinedContainer = new XRect(centerTable, pageLine, tableWidth, tableTextFont.Height);

            foreach (var item in content)
            {

                if (isHighlighted == true)
                {
                    outlinedContainer.Y = pageLine;
                    gfx.DrawRectangle(brushgrey, outlinedContainer);
                }

                textformatter.DrawString(
                    item[0],
                    tableTextFontBold,
                    brush,
                    new XRect(centerTable, pageLine, tableWidth, tableTextFont.Height),
                    format
                );

                textformatter.DrawString(
                    item[1],
                    tableTextFontBold,
                    brush,
                    new XRect(centerTable + (tableWidth - tableMargin - gfx.MeasureString(item[1], tableTextFont).Width), pageLine, tableWidth, tableTextFont.Height),
                    format
                );

                pageLine += tableTextFont.Height + cellPadding;
            }
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

            double x = (page.Width.Value - width) / 2;
            

            gfx.DrawImage(image, x, 10, width, height);
            pageLine = Convert.ToInt16(height) + 10;
        }

        public void AddFooterText(string[] content)
        {
            XUnit contentLength = XUnit.FromPoint(content.Length);
            XUnit positionY = page.Height - (contentLength * bodyTextFont.Height);

            foreach (string item in content)
            {
                XUnit bodyWidth = XUnit.FromPoint(gfx.MeasureString(item, bodySubTextFont).Width);
                XUnit positionX = (page.Width - bodyWidth) / 2;

                textformatter.DrawString(
                    item,
                    bodySubTextFont,
                    XBrushes.Black,
                    new XRect(positionX.Value, positionY.Value, gfx.MeasureString(item, bodySubTextFont).Width, bodySubTextFont.Height),
                    format
                );

                positionY += XUnit.FromPoint(bodySubTextFont.Height);
            }
        }
    }
}
