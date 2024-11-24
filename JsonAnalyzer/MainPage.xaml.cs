using System.Text.Json;

namespace JsonAnalyzer;

public partial class MainPage : ContentPage
{
    private string? _jsonFilePath;
    private List<dynamic> _jsonData = new();

    public MainPage()
    {
        InitializeComponent();
        AddFileButton.Clicked += OnAddFileButtonClicked;
        FindButton.Clicked += OnFindButtonClicked;
        ClearButton.Clicked += OnClearButtonClicked;
        AddNewItemButton.Clicked += OnAddNewItemButtonClicked;
        EditItemInfoButton.Clicked += OnEditItemInfoButtonClicked;
        InfoAboutProjectButton.Clicked += OnInfoAboutProjectButtonClicked;
    }

    private async void OnAddFileButtonClicked(object sender, EventArgs e)
    {
        var customJsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "public.json" } }, // iOS MIME type
        { DevicePlatform.Android, new[] { "application/json" } }, // Android MIME type
        { DevicePlatform.WinUI, new[] { ".json" } }, // Windows extension
        { DevicePlatform.MacCatalyst, new[] { "json" } } // Mac MIME type
    });

        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select a JSON file",
            FileTypes = customJsonFileType
        });

        if (result != null)
        {
            _jsonFilePath = result.FullPath;
            var fileContent = File.ReadAllText(_jsonFilePath);
            try
            {
                _jsonData = JsonSerializer.Deserialize<List<dynamic>>(fileContent) ?? new List<dynamic>();
                JsonCollectionView.ItemsSource = _jsonData;
            }
            catch (JsonException)
            {
                await DisplayAlert("Error", "Invalid JSON format", "OK");
            }
        }
    }


    private void OnFindButtonClicked(object sender, EventArgs e)
    {
        var filteredData = _jsonData.AsEnumerable();

        // Фільтрування за критеріями
        if (!string.IsNullOrWhiteSpace(FilterNameEntry.Text))
            filteredData = filteredData.Where(item => item.name.ToString().Contains(FilterNameEntry.Text, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(FilterBrandEntry.Text))
            filteredData = filteredData.Where(item => item.brand.ToString().Contains(FilterBrandEntry.Text, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(FilterCategoryEntry.Text))
            filteredData = filteredData.Where(item => item.category.ToString().Contains(FilterCategoryEntry.Text, StringComparison.OrdinalIgnoreCase));

        if (double.TryParse(FilterMinPriceEntry.Text, out var minPrice))
            filteredData = filteredData.Where(item => item.price >= minPrice);

        if (double.TryParse(FilterMaxPriceEntry.Text, out var maxPrice))
            filteredData = filteredData.Where(item => item.price <= maxPrice);

        if (int.TryParse(FilterYearEntry.Text, out var year))
            filteredData = filteredData.Where(item => item.year == year);

        if (int.TryParse(FilterStockEntry.Text, out var stock))
            filteredData = filteredData.Where(item => item.stock >= stock);

        JsonCollectionView.ItemsSource = filteredData.ToList();
    }

    private void OnClearButtonClicked(object sender, EventArgs e)
    {
        FilterNameEntry.Text = string.Empty;
        FilterBrandEntry.Text = string.Empty;
        FilterCategoryEntry.Text = string.Empty;
        FilterMinPriceEntry.Text = string.Empty;
        FilterMaxPriceEntry.Text = string.Empty;
        FilterYearEntry.Text = string.Empty;
        FilterStockEntry.Text = string.Empty;

        JsonCollectionView.ItemsSource = _jsonData;
    }

    private async void OnAddNewItemButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditPage());
    }

    private async void OnEditItemInfoButtonClicked(object sender, EventArgs e)
    {
        if (_jsonData != null && _jsonData.Any())
        {
            await Navigation.PushAsync(new EditPage(_jsonData));
        }
    }

    private async void OnInfoAboutProjectButtonClicked(object sender, EventArgs e)
    {
        await DisplayAlert("About Project",
            "Author: [Your Name]\nCourse: [Your Course]\nYear: 2024\nDescription: JSON Analyzer Application",
            "OK");
    }
}
