// See https://aka.ms/new-console-template for more information

using HitsTheNeighbours.Infrastructure;
using HitsTheNeighbours.Infrastructure.Excel;
using HitsTheNeighbours.Infrastructure.Pdf;

EnvironmentSetup.SetLicenses();
var excelPath = new FileInfo("/Users/lmeyvaert/Git/GitHub/CodeLens/hits-the-neighbours/assets/songs.xlsx");
var backImagePath = "/Users/lmeyvaert/Git/GitHub/CodeLens/hits-the-neighbours/assets/back.jpeg";
var frontImagePath = "/Users/lmeyvaert/Git/GitHub/CodeLens/hits-the-neighbours/assets/front.jpeg";

var excelSvc = new ExcelReadService();
var pdfSvc = new PdfPrintService();

var hits = await excelSvc.ReadExcelAsync(excelPath.FullName);
Console.WriteLine("Read {0} hits from Excel.", hits.Count());
var result = pdfSvc.GeneratePdf("/Users/lmeyvaert/Downloads/hits.pdf", "Hit(s)TheNeighbours",  backImagePath, frontImagePath, hits.ToList());

if(result)
    Console.WriteLine("PDF generated successfully!");
else
    Console.WriteLine("Failed to generate PDF.");