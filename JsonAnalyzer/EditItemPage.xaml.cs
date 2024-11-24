using JsonAnalyzer.Models;

namespace JsonAnalyzer
{
    public partial class EditItemPage : ContentPage
    {
        private Car _car;
        private MainPage _mainPage;

        public EditItemPage(Car car, MainPage mainPage)
        {
            InitializeComponent();
            _car = car;
            _mainPage = mainPage;

            JsonEditor.Text = Newtonsoft.Json.JsonConvert.SerializeObject(_car, Newtonsoft.Json.Formatting.Indented);
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                var updatedCar = Newtonsoft.Json.JsonConvert.DeserializeObject<Car>(JsonEditor.Text);
                if (updatedCar != null)
                {
                    _car.Name = updatedCar.Name;
                    _car.Brand = updatedCar.Brand;
                    _car.Price = updatedCar.Price;
                    _car.Year = updatedCar.Year;
                    _car.Category = updatedCar.Category;
                    _car.Stock = updatedCar.Stock;

                    _mainPage.RefreshData();
                    Navigation.PopAsync();
                }
            }
            catch
            {
                DisplayAlert("Error", "Invalid JSON format.", "OK");
            }
        }
    }
}
