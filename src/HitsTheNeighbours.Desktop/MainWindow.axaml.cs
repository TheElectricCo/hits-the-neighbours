using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using HitsTheNeighbours.Infrastructure;
using HitsTheNeighbours.Infrastructure.Excel;
using HitsTheNeighbours.Infrastructure.Pdf;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace HitsTheNeighbours.Desktop;

public partial class MainWindow : Window
{
    private readonly PdfPrintService _pdfService ;
    private readonly ExcelReadService _excelService;
    
    private string _excelFilePath = string.Empty;
    private string _frontImagePath = string.Empty;
    private string _backImagePath = string.Empty;
    
    private string _title = "Hit(s)TheNeighbours";
    
    public MainWindow()
    {
        InitializeComponent();
        
        EnvironmentSetup.SetLicenses();
        _excelService = new ExcelReadService();
        _pdfService = new PdfPrintService();
        
        LoadExcelBtn.Click += LoadExcelBtnOnClick;
        LoadBackImageBtn.Click += LoadBackImageBtnOnClick;
        LoadFrontImageBtn.Click += LoadFrontImageBtnOnClick;

        ProcessBtn.Click += ProcessBtnOnClick;
    }

    private async void LoadBackImageBtnOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            //This can also be applied for SaveFilePicker.
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Select Hit The Neighbours Front Image File (QR side)",
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (files.Count > 0)
            {
                var file = files[0];
                BackImagePath.Text = file.Path.AbsolutePath;
                _backImagePath = file.Path.AbsolutePath;
                CanEnable();
            }
        }
        catch (Exception ex)
        {
            throw; // TODO handle exception
        }
    }

    private async void LoadExcelBtnOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            //This can also be applied for SaveFilePicker.
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Select Hits The Neighbours Excel File",
                FileTypeFilter = [new FilePickerFileType("Excel Files") { MimeTypes = [".xls", ".xlsx"] }]
            });

            if (files.Count > 0)
            {
                var file = files[0];
                ExcelPath.Text = file.Path.AbsolutePath;
                _excelFilePath = file.Path.AbsolutePath;
                CanEnable();
            }
        }
        catch (Exception ex)
        {
            throw; // TODO handle exception
        }
    }
    
    private async void LoadFrontImageBtnOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            //This can also be applied for SaveFilePicker.
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Select Hit The Neighbours Front Image File (QR side)",
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (files.Count > 0)
            {
                var file = files[0];
                FrontImagePath.Text = file.Path.AbsolutePath;
                _frontImagePath = file.Path.AbsolutePath;
                CanEnable();
            }
        }
        catch (Exception ex)
        {
            throw; // TODO handle exception
        }
    }
    
    private async void ProcessBtnOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            //This can also be applied for SaveFilePicker.
            var storageFile = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Select where to save Hits The Neighbours PDF File",
                FileTypeChoices =  [FilePickerFileTypes.Pdf]
                
            });
            if (storageFile != null)
            {
                var filePath = storageFile.Path.AbsolutePath;
                var hits = await _excelService.ReadExcelAsync(_excelFilePath);
                var result = _pdfService.GeneratePdf(filePath, _title, _backImagePath, _frontImagePath, hits);
                
                var box = MessageBoxManager
                    .GetMessageBoxStandard("PDF Generation", result ? "PDF generated successfully!" : "Failed to generate PDF.",
                        ButtonEnum.Ok);
                
                await box.ShowAsync();
            }
        }
        catch (Exception ex)
        {
            throw; // TODO handle exception
        }
    }

    private void CanEnable()
    {
        ProcessBtn.IsEnabled = !string.IsNullOrEmpty(_excelFilePath) &&
                               !string.IsNullOrEmpty(_frontImagePath) &&
                               !string.IsNullOrEmpty(_backImagePath);
    }
}