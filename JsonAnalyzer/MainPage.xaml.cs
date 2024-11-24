﻿using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using JsonAnalyzer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JsonAnalyzer;

public partial class MainPage : ContentPage
{
    private List<Car> _cars = new();
    private string? _jsonFilePath;
    private List<dynamic> _jsonData = new();
    public MainPage()
    {
        InitializeComponent();
    }


    private async void OnAddFileClicked(object sender, EventArgs e)
    {
        var customJsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "public.json" } },
        { DevicePlatform.Android, new[] { "application/json" } },
        { DevicePlatform.WinUI, new[] { ".json" } },
        { DevicePlatform.MacCatalyst, new[] { "json" } }
    });

        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select a JSON file",
            FileTypes = customJsonFileType
        });

        if (result != null)
        {
            try
            {
                var fileContent = File.ReadAllText(result.FullPath);

                // Десеріалізація JSON
                var carsFromFile = JsonConvert.DeserializeObject<List<Car>>(fileContent);

                if (carsFromFile == null || !carsFromFile.Any())
                {
                    await DisplayAlert("Error", "JSON file is empty or has invalid structure.", "OK");
                    return;
                }

                _cars = carsFromFile;

                // Оновлення CollectionView
                JsonCollectionView.ItemsSource = null;
                JsonCollectionView.ItemsSource = _cars;

                _jsonFilePath = result.FullPath; // Збереження шляху до JSON
            }
            catch (JsonReaderException jsonEx)
            {
                await DisplayAlert("Error", $"Invalid JSON format: {jsonEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
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
        FilterNameEntry.Text = string.Empty;
        FilterBrandEntry.Text = string.Empty;
        FilterCategoryEntry.Text = string.Empty;
        FilterMinPriceEntry.Text = string.Empty;
        FilterMaxPriceEntry.Text = string.Empty;
        FilterYearEntry.Text = string.Empty;
        FilterStockEntry.Text = string.Empty;

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
      //  await Navigation.PushAsync(new InfoPage());
    }

    public void RefreshData()
    {
        JsonCollectionView.ItemsSource = null;
        JsonCollectionView.ItemsSource = _cars;
    }
}
