using System.Windows;
using Emk.UI.Desktop.ViewModels;

namespace Emk.UI.Desktop.Views
{
    public partial class MainWindow : Window
    {
        private readonly AppointmentViewModel _vm;

        public MainWindow(AppointmentViewModel vm)
        {
            InitializeComponent();
            DataContext = _vm = vm;
        }

        private async void Load_Click(object sender, RoutedEventArgs e) => await _vm.LoadAsync();

        private async void Book_Click(object sender, RoutedEventArgs e) => await _vm.BookAsync();
    }
}
