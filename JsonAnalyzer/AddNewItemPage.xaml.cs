using JsonAnalyzer.Models;

namespace JsonAnalyzer;

public partial class AddNewItemPage : ContentPage
{
    private readonly List<Car> _cars;
    private readonly MainPage _mainPage;

    public AddNewItemPage(List<Car> cars, MainPage mainPage)
    {
        InitializeComponent();
        _cars = cars;
        _mainPage = mainPage;
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var newCar = new Car
            {
                Id = new Random().Next(1000), // Simple ID generator
                Name = NameEntry.Text,
                Brand = BrandEntry.Text,
                Price = double.TryParse(PriceEntry.Text, out var price) ? price : 0,
                Year = int.TryParse(YearEntry.Text, out var year) ? year : 0,
                Category = CategoryEntry.Text,
                Stock = int.TryParse(StockEntry.Text, out var stock) ? stock : 0
            };

            _cars.Add(newCar);
            _mainPage.RefreshData();
            Navigation.PopAsync();
        }
        catch
        {
            DisplayAlert("Error", "Failed to save new item. Please check the inputs.", "OK");
        }
    }
}
