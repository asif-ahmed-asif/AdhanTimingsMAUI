using AdhanTimingsMAUI.ViewModel;

namespace AdhanTimingsMAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (BindingContext is MainPageViewModel viewModel)
            {
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(viewModel.SelectedLocation) && viewModel.SelectedLocation != null)
                    {
                        LocationSearchBar.Unfocus();
                    }
                };
            }
        }
    }
}
