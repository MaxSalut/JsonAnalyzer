using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonAnalyzer;

public partial class MainPage : ContentPage
{
    private List<Car> _cars = new();

    public MainPage()
    {
        InitializeComponent();
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        try
        {
            var json = File.ReadAllText("cars.json");
            _cars = JsonConvert.DeserializeObject<List<Car>>(json) ?? new List<Car>();
            JsonCollectionView.ItemsSource = _cars;
        }
        catch
        {
            DisplayAlert("Error", "Could not load sample data.", "OK");
        }
    }

    private void OnAddFileClicked(object sender, EventArgs e)
    {
        try
        {
            var filePath = "cars.json"; // Замінити на діалог вибору файлів за необхідності
            var json = File.ReadAllText(filePath);
            var carsFromFile = JsonConvert.DeserializeObject<List<Car>>(json);
            if (carsFromFile != null)
            {
                _cars = carsFromFile;
                JsonCollectionView.ItemsSource = _cars;
            }
        }
        catch
        {
            DisplayAlert("Error", "Invalid JSON file format.", "OK");
        }
    }

    private void OnFindClicked(object sender, EventArgs e)
    {
        var filteredCars = _cars.AsEnumerable();

        // Фільтри за введеними даними
        if (!string.IsNullOrWhiteSpace(FilterNameEntry.Text))
            filteredCars = filteredCars.Where(car => car.Name.Contains(FilterNameEntry.Text, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(FilterBrandEntry.Text))
            filteredCars = filteredCars.Where(car => car.Brand.Contains(FilterBrandEntry.Text, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(FilterCategoryEntry.Text))
            filteredCars = filteredCars.Where(car => car.Category.Contains(FilterCategoryEntry.Text, StringComparison.OrdinalIgnoreCase));

        if (double.TryParse(FilterMinPriceEntry.Text, out var minPrice))
            filteredCars = filteredCars.Where(car => car.Price >= minPrice);

        if (double.TryParse(FilterMaxPriceEntry.Text, out var maxPrice))
            filteredCars = filteredCars.Where(car => car.Price <= maxPrice);

        if (int.TryParse(FilterYearEntry.Text, out var year))
            filteredCars = filteredCars.Where(car => car.Year == year);

        if (int.TryParse(FilterStockEntry.Text, out var stock))
            filteredCars = filteredCars.Where(car => car.Stock >= stock);

        // Оновлюємо CollectionView з результатами фільтрації
        JsonCollectionView.ItemsSource = filteredCars.ToList();
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        // Очистити поля фільтрів
        FilterNameEntry.Text = string.Empty;
        FilterBrandEntry.Text = string.Empty;
        FilterCategoryEntry.Text = string.Empty;
        FilterMinPriceEntry.Text = string.Empty;
        FilterMaxPriceEntry.Text = string.Empty;
        FilterYearEntry.Text = string.Empty;
        FilterStockEntry.Text = string.Empty;

        // Повернути початковий список
        JsonCollectionView.ItemsSource = _cars;
    }

    private async void OnAddNewItemClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddNewItemPage(_cars, this));
    }

    private async void OnEditItemInfoClicked(object sender, EventArgs e)
    {
        if (JsonCollectionView.SelectedItem is Car selectedCar)
        {
            await Navigation.PushAsync(new EditItemPage(selectedCar, this));
        }
        else
        {
            await DisplayAlert("Error", "Please select an item to edit.", "OK");
        }
    }

    private async void OnInfoClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Error", "Please select an item to edit.", "OK");
    }

    public void RefreshData()
    {
        JsonCollectionView.ItemsSource = null;
        JsonCollectionView.ItemsSource = _cars;
    }
}
