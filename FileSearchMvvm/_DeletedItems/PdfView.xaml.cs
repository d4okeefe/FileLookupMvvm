namespace FileSearchMvvm.Views
{
    public partial class PdfView : System.Windows.Controls.UserControl
    {
        public PdfView()
        {
            InitializeComponent();
        }
        public PdfView(FileSearchMvvm.ViewModels.SearchViewModelFolder.SearchViewModel searchViewModel)
        {
            InitializeComponent();
        }

        //public PdfView(FileSearchMvvm.ViewModels.PdfViewModel pdfViewModel)
        //{
        //    InitializeComponent();
        //    DataContext = pdfViewModel;
        //}
        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Window.GetWindow(this).Close();
        }
    }
}
