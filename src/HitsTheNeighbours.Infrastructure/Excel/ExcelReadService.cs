using HitsTheNeighbours.Core.Models;
using OfficeOpenXml;

namespace HitsTheNeighbours.Infrastructure.Excel;

public class ExcelReadService
{
    public Task<List<Hit>> ReadExcelAsync(string filePath)
    {
       // Step 1: Load the Excel file
       using var package = new ExcelPackage(new FileInfo(filePath));
       var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet
       
       // Step 2: Parse the data into Hit objects
         var hits = new List<Hit>();
         var rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row < rowCount; row++) // Assuming the first row contains headers
            {
                try {
                    var hit = new Hit
                    {
                        Title = worksheet.Cells[row, 1].Text,
                        Artist = worksheet.Cells[row, 2].Text,
                        Year = int.Parse(worksheet.Cells[row, 3].Text),
                        SpotifyLink = worksheet.Cells[row, 4].Text
                    };
                    hits.Add(hit);
                } catch (Exception)
                {
                    // Log or handle the error for the specific row if necessary
                    continue; // Skip rows with invalid data
                }
            }
            
         return Task.FromResult<List<Hit>>(hits);
    }
}