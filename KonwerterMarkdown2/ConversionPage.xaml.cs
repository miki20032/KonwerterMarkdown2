using System.Windows.Controls;

namespace KonwerterMarkdown2
{
    public partial class ConversionPage : Page
    {
        private MainWindow _mainWindow;

        public ConversionPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void ConvertDirectoryToHtml_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.ConvertDirectoryToHtml_Click(sender, e);
        }

        private void ConvertDirectoryToMarkdown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.ConvertDirectoryToMarkdown_Click(sender, e);
        }

        private void CancelOperation_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.CancelOperation_Click(sender, e);
            _mainWindow.MainFrame.Navigate(null);  // Navigates back to the initial content in MainWindow
        }
    }
}
