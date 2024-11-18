using AdhanTimingsMAUI.ViewModel;

namespace AdhanTimingsMAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Subscribe to property changes
            (BindingContext as MainPageViewModel).PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainPageViewModel.SelectedLocation))
                {
                    LocationSearchBar.Unfocus();
                }
            };
        }
    }
}
