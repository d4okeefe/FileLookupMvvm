using FileSearchMvvm.ViewModels;
using FileSearchMvvm.Views;

namespace FileSearchMvvm
{
    public partial class App : System.Windows.Application
    {
        public App()
        {
            new MainWindow().Show();
            //new MainWindow
            //{
            //    DataContext = new SearchViewModel()
            //}.Show();
        }
    }
}
