using System.Text.Json;

namespace JsonAnalyzer;

public partial class EditPage : ContentPage
{
    private dynamic? _itemToEdit;

    public EditPage()
    {
        InitializeComponent();
    }

    public EditPage(dynamic item)
    {
        InitializeComponent();
        _itemToEdit = item;
        JsonEditor.Text = JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true });
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(JsonEditor.Text)) return;

        try
        {
            var updatedItem = JsonSerializer.Deserialize<dynamic>(JsonEditor.Text);
            if (updatedItem != null)
            {
                // TODO: Implement logic to save this updated item back to the main list
            }

            await Navigation.PopAsync();
        }
        catch (JsonException)
        {
            await DisplayAlert("Error", "Invalid JSON format", "OK");
        }
    }
}
