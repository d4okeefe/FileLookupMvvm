namespace FileSearchMvvm.Views
{
    public partial class SearchView : System.Windows.Controls.UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Window.GetWindow(this).Close();
        }

        private void EventTrigger_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
    }
}