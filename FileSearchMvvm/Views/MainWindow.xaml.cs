namespace FileSearchMvvm.Views
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
                MoveFocus(
                    new System.Windows.Input.TraversalRequest(
                        System.Windows.Input.FocusNavigationDirection.Next));
        }
    }
}
