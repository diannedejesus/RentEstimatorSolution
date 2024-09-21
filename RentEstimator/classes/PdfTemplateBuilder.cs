using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentCalculator;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Collections.ObjectModel;

namespace RentEstimator
{
    internal class PdfTemplateBuilder
    {
        private readonly Dictionary<string, string> _replacementValues;
        private readonly Dictionary<string, List<Dictionary<string, object>>> _firstpageData;
        private readonly Dictionary<string, string> _headerfooter;
        public PdfTemplateBuilder(string templateJsonLocation, string headerFooterJsonLocation, Dictionary<string, string> replacementValues)
        {
            _firstpageData = new ReadandParseJsonFile(templateJsonLocation).ExtractPageData();
            _headerfooter = new ReadandParseJsonFile(headerFooterJsonLocation).ExtractFirstPageData();
            _replacementValues = replacementValues;

            GenerateTemplate(_firstpageData, _headerfooter);
        }

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private void GenerateTemplate(Dictionary<string, List<Dictionary<string, object>>> templateContent, Dictionary<string, string> headerFooter)
        {
            pdfGenerator currentPDF = new pdfGenerator();

            //loop through pages
            foreach (var page in templateContent)
            {
                currentPDF.AddPage();

                //add header
                if(headerFooter != null)
                    currentPDF.AddHeaderImage(1, headerFooter["logopath"]);
                //ERROR MESSAGE

                //loop through content
                foreach (var contentItem in page.Value)
                {
                    if (contentItem["type"].ToString() == "table")
                    {
                        List<string[]> tableline = JsonSerializer.Deserialize<List<string[]>>((JsonElement)contentItem["content"], _options);
                        foreach (var item in tableline)
                        {
                            item[1] = replaceValues(item[1].ToString());
                        }
                        currentPDF.CreateTableLayout(tableline);
                    }else if (contentItem["type"].ToString() == "table2")
                    {
                        List<string[]> tableline = JsonSerializer.Deserialize<List<string[]>>((JsonElement)contentItem["content"], _options);
                        foreach (var item in tableline)
                        {
                            item[1] = replaceValues(item[1].ToString());
                        }
                        currentPDF.AddTwoCellLayout(tableline, contentItem["bg"].ToString() == "true");
                    }
                    else if (contentItem["type"].ToString() == "table2Header")
                    {
                        List<string[]> tableline = JsonSerializer.Deserialize<List<string[]>>((JsonElement)contentItem["content"], _options);
                        foreach (var item in tableline)
                        {
                            item[1] = replaceValues(item[1].ToString());
                        }
                        currentPDF.AddTwoCellLayoutHeader(tableline, contentItem["bg"].ToString() == "true");
                    }else if (contentItem["type"].ToString() == "table3")
                    {
                        Dictionary<string, string[]> tableline = JsonSerializer.Deserialize<Dictionary<string, string[]>>((JsonElement)contentItem["content"], _options);
                        foreach (var item in tableline)
                        {
                            for(int i=0; i<item.Value.Length; i++)
                            {
                                tableline[item.Key][i] = replaceValues(tableline[item.Key][i].ToString());
                            }
                                
                        }
                        currentPDF.CreateTableLayout2(tableline);
                    }
                    else
                    {
                        currentPDF.AddTextPdf(contentItem["content"].ToString(), contentItem["type"].ToString());
                    }
                }

                //add footer
                if (headerFooter != null)
                    currentPDF.AddFooterText(new string[] { headerFooter["footer1"], headerFooter["footer1"] });
                //ERROR MESSAGE
            }

            currentPDF.CreatePDF("test.pdf");
        }

        private string replaceValues(string value)
        {
             if(!string.IsNullOrEmpty(value)) 
             {
                if (_replacementValues.ContainsKey(value))
                {
                    return _replacementValues[value];
                }
             }

             return value;
        }
    }
}
