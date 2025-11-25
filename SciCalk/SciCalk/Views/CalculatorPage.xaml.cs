using SciCalk.ViewModels;

namespace SciCalk.Views;

public partial class CalculatorPage : ContentPage
{
	public CalculatorPage()
	{
        InitializeComponent();
        BindingContext = new CalculatorViewModel();
    }
}