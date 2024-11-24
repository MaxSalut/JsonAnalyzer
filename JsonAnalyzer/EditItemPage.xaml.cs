using JsonAnalyzer.Models;

namespace JsonAnalyzer;

public partial class EditItemPage : ContentPage
{
    private Car _car;
    private MainPage _mainPage;

    public EditItemPage(Car car, MainPage mainPage)
    {
        InitializeComponent();
        _car = car;
        _mainPage = mainPage;

        // Заповнення даних у форму
        NameEntry.Text = _car.Name;
        BrandEntry.Text = _car.Brand;
        PriceEntry.Text = _car.Price.ToString();
        YearEntry.Text = _car.Year.ToString();
        CategoryEntry.Text = _car.Category;
        StockEntry.Text = _car.Stock.ToString();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        // Оновлення даних автомобіля
        _car.Name = NameEntry.Text;
        _car.Brand = BrandEntry.Text;
        _car.Price = double.TryParse(PriceEntry.Text, out var price) ? price : 0;
        _car.Year = int.TryParse(YearEntry.Text, out var year) ? year : 0;
        _car.Category = CategoryEntry.Text;
        _car.Stock = int.TryParse(StockEntry.Text, out var stock) ? stock : 0;

        _mainPage.RefreshData();
        Navigation.PopAsync();
    }
}
