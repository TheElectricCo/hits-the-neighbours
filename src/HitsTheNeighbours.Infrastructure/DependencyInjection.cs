using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using QuestPDF.Infrastructure;

namespace HitsTheNeighbours.Infrastructure;

public static class EnvironmentSetup
{

    public static void SetLicenses()
    {
        //Set Licenses
        ExcelPackage.License.SetNonCommercialPersonal("HitsTheNeighbours");
        QuestPDF.Settings.License = LicenseType.Community;
    }
}