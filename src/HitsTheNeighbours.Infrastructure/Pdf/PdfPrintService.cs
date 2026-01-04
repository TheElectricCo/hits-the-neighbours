using HitsTheNeighbours.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;

namespace HitsTheNeighbours.Infrastructure.Pdf;

public class PdfPrintService
{
    public bool GeneratePdf(string filePath, string gameTitle, string backImagePath, string frontImagePath, List<Hit> hits)
    {
        try
        {
            //Step 1: Create a PDF document
            var document = Document.Create(container =>
            {
                var hitsPerPage = 12;
                var totalPages = (int)Math.Ceiling((double)hits.Count / hitsPerPage);
                for (var pageIndex = 0; pageIndex < totalPages; pageIndex++)
                {
                    var hitsForPage = hits.Skip(pageIndex * hitsPerPage).Take(hitsPerPage).ToList();
                    GenerateQrSheet(container,  gameTitle, frontImagePath, hitsForPage);
                    GenerateAnswerSheet(container,backImagePath, hitsForPage);
                }
            });

            document.GeneratePdf(filePath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void GenerateAnswerSheet(IDocumentContainer container, string imagePath, List<Hit> hits)
    {
        var mirroredHits = hits.Select((item, index) => new { item, index })
            .GroupBy(x => x.index / 3)
            .SelectMany(g => g.Reverse())
            .Select(x => x.item)
            .ToList();
        
        container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                    page.Content()
                        .AlignCenter()
                        .AlignMiddle()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var i in Enumerable.Range(0, 3))
                                    columns.ConstantColumn(5.5f, Unit.Centimetre);
                            });
                            foreach (var i in mirroredHits)
                            {
                                table
                                    .Cell()
                                    .Height(5.5f, Unit.Centimetre)
                                    //.ShowEntire()
                                    //.PaddingHorizontal(5)
                                    .Border(1)
                                    //.Padding(10)
                                    .Layers(lyrs =>
                                    {
                                        lyrs.Layer().Height(5.5f, Unit.Centimetre).Width(5.5f, Unit.Centimetre).Image(imagePath).FitArea();
                                        lyrs.PrimaryLayer().Column(column =>
                                        {
                                            column.Item()
                                                .Height(5.5f, Unit.Centimetre)
                                                .AlignMiddle()
                                                .Column(cl =>
                                                {
                                                    cl.Item()  
                                                        .PaddingBottom(.5f, Unit.Centimetre)
                                                        .Text(i.Artist)
                                                        .AlignCenter()
                                                        .FontSize(10);
                                            
                                                    cl.Item()
                                                        .Text(i.Year.ToString())
                                                        .Bold()
                                                        .AlignCenter()
                                                        .FontSize(32);
                                            
                                                    cl.Item()
                                                        .PaddingTop(.5f, Unit.Centimetre)
                                                        .Text(i.Title)
                                                        .AlignCenter()
                                                        .FontSize(10);
                                                });
                                        
                                        });
                                    });
                            }
                        });
                });
    }
    private void GenerateQrSheet(IDocumentContainer container, string gameTitle, string imagePath, List<Hit> hits)
    {
        container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                    page.Content()
                        .AlignCenter()
                        .AlignMiddle()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var i in Enumerable.Range(0, 3))
                                    columns.ConstantColumn(5.5f, Unit.Centimetre);
                            });
                            foreach (var i in hits)
                            {
                                table
                                    .Cell()
                                    .Height(5.5f, Unit.Centimetre)
                                    //.ShowEntire()
                                    //.PaddingHorizontal(5)
                                    .Border(1)
                                    //.Padding(10)
                                    .Column(column =>
                                    {
                                      
                                        column.Item()
                                            .Height(5.5f, Unit.Centimetre)
                                            .Layers(lyrs =>
                                            {
                                                lyrs.Layer().Height(5.5f, Unit.Centimetre).Width(5.5f, Unit.Centimetre).Image(imagePath).FitArea();
                                                lyrs.PrimaryLayer()
                                                    .PaddingLeft(1.25f, Unit.Centimetre)
                                                    .PaddingTop(1f, Unit.Centimetre)
                                                    .Column(cl =>
                                                {
                                                    cl.Item()
                                                        .Height(3, Unit.Centimetre)
                                                        .Width(3, Unit.Centimetre)
                                                        .AlignCenter()
                                                        .Background(Colors.White)
                                                        .Svg(size =>
                                                        {
                                                            var writer = new QRCodeWriter();
                                                            var qrCode = writer.encode(i.SpotifyLink, BarcodeFormat.QR_CODE, (int)size.Width, (int)size.Height);
                                                            var renderer = new SvgRenderer { FontName = "Lato" };
                                                            return renderer.Render(qrCode, BarcodeFormat.EAN_13, null).Content;
                                                        });
                                                    
                                                    cl.Item()
                                                        .PaddingTop(0.3f, Unit.Centimetre)
                                                        .PaddingLeft(-1.25f, Unit.Centimetre)
                                                        .Text(gameTitle)
                                                        .AlignCenter()
                                                        .FontSize(14)
                                                        .Bold()
                                                        .FontColor(Colors.White);   
                                                });
                                            });
                                            
                                    });
                            }
                        });
                });
    }
     
    
    
}