using Microsoft.Maui.Controls;
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
                // Зчитуємо вміст JSON
                var fileContent = File.ReadAllText(result.FullPath);

                // Логування для відладки
                Console.WriteLine("Loaded JSON Content:");
                Console.WriteLine(fileContent);

                // Десеріалізуємо JSON у список
                _cars = JsonConvert.DeserializeObject<List<Car>>(fileContent) ?? new List<Car>();

                // Логування десеріалізованих даних
                Console.WriteLine("Parsed Cars:");
                foreach (var car in _cars)
                {
                    Console.WriteLine($"{car.Id}, {car.Name}, {car.Brand}");
                }

                // Оновлюємо CollectionView
                JsonCollectionView.ItemsSource = null;
                JsonCollectionView.ItemsSource = _cars;

                // Зберігаємо шлях до JSON
                _jsonFilePath = result.FullPath;
            }
            catch (JsonReaderException)
            {
                await DisplayAlert("Error", "Invalid JSON format.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
        }
    }
    private void SaveJsonToFile()
    {
        try
        {
            if (!string.IsNullOrEmpty(_jsonFilePath))
            {
                var jsonContent = JsonConvert.SerializeObject(_cars, Formatting.Indented); // Форматований JSON
                File.WriteAllText(_jsonFilePath, jsonContent); // Запис у файл
                Console.WriteLine("JSON file saved successfully.");
            }
            else
            {
                Console.WriteLine("JSON file path is not set.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving JSON file: {ex.Message}");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (SelectedCar != null)
        {
            // Підтвердження видалення
            bool isConfirmed = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete '{SelectedCar.Name}'?", "Yes", "No");
            if (isConfirmed)
            {
                _cars.Remove(SelectedCar); // Видалення зі списку
                SelectedCar = null; // Скидання вибраного елемента
                RefreshData(); // Оновлення в UI
                SaveJsonToFile(); // Збереження у файл
            }
        }
        else
        {
            await DisplayAlert("Error", "Please select an item to delete.", "OK");
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
        SaveJsonToFile();
    }

    private Car? _selectedCar;

    public Car? SelectedCar
    {
        get => _selectedCar;
        set
        {
            _selectedCar = value;
            OnPropertyChanged(); // Сповіщення про зміну властивості
        }
    }

    private async void OnEditItemInfoClicked(object sender, EventArgs e)
    {
        if (SelectedCar != null)
        {
            await Navigation.PushAsync(new EditItemPage(SelectedCar, this));

        }
        else
        {
            await DisplayAlert("Error", "Please select an item to edit.", "OK");
        }
    }
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Car selectedCar)
        {
            SelectedCar = selectedCar;
        }
        else
        {
            SelectedCar = null;
        }
    }


    private async void OnInfoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InfoPage());
    }

    public void RefreshData()
    {
        JsonCollectionView.ItemsSource = null;
        JsonCollectionView.ItemsSource = _cars;
        SaveJsonToFile();
    }
}
