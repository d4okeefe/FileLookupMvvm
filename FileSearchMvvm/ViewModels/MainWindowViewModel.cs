using System.ComponentModel;

namespace FileSearchMvvm.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
        }

        public SearchViewModelFolder.SearchViewModel SearchVM { get; set; }
        public PdfViewModel PdfVM { get; set; }
    }
}
