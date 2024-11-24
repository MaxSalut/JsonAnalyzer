using Microsoft.Maui.Controls;

namespace JsonAnalyzer;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new MainPage());
    }
}
