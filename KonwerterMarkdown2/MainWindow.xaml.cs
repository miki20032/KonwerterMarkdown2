using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using ClassLibrary2;

namespace KonwerterMarkdown2
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Class1 _converter = new Class1();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConvertToHtml_Click(object sender, RoutedEventArgs e)
        {
            string markdown = MarkdownTextBox.Text;
            if (!string.IsNullOrEmpty(markdown))
            {
                string html = _converter.ConvertMarkdownToHtml(markdown);
                HtmlTextBox.Text = html;
            }
            else
            {
                MessageBox.Show("Markdown text is empty.");
            }
        }

        private void ConvertToMarkdown_Click(object sender, RoutedEventArgs e)
        {
            string html = HtmlTextBox.Text;
            if (!string.IsNullOrEmpty(html))
            {
                string markdown = _converter.ConvertHtmlToMarkdown(html);
                MarkdownTextBox.Text = markdown;
            }
            else
            {
                MessageBox.Show("HTML text is empty.");
            }
        }

        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            MarkdownTextBox.Clear();
            HtmlTextBox.Clear();
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MarkdownTextBox.Text))
            {
                Clipboard.SetText(MarkdownTextBox.Text);
            }
            else
            {
                MessageBox.Show("Markdown text is empty.");
            }
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(MarkdownTextBox.Text))
                {
                    File.WriteAllText(saveFileDialog.FileName, MarkdownTextBox.Text);
                }
                else
                {
                    MessageBox.Show("Markdown text is empty.");
                }
            }
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string content = File.ReadAllText(openFileDialog.FileName);
                if (!string.IsNullOrEmpty(content))
                {
                    MarkdownTextBox.Text = content;
                }
                else
                {
                    MessageBox.Show("File is empty.");
                }
            }
        }

        public async void ConvertDirectoryToHtml_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var progress = new Progress<int>(value => ProgressBar.Value = value);

                try
                {
                    await Task.Run(() =>
                    {
                        _converter.ConvertDirectoryToHtml(folderDialog.SelectedPath, progress, _cancellationTokenSource.Token);
                    }, _cancellationTokenSource.Token);
                    MessageBox.Show("Directory conversion to HTML completed.");
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation canceled.");
                }
            }
        }

        public async void ConvertDirectoryToMarkdown_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var progress = new Progress<int>(value => ProgressBar.Value = value);

                try
                {
                    await Task.Run(() =>
                    {
                        _converter.ConvertDirectoryToMarkdown(folderDialog.SelectedPath, progress, _cancellationTokenSource.Token);
                    }, _cancellationTokenSource.Token);
                    MessageBox.Show("Directory conversion to Markdown completed.");
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation canceled.");
                }
            }
        }

        public void CancelOperation_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        public void OpenConversionPage_Click(object sender, RoutedEventArgs e)
        {
            var conversionPage = new ConversionPage(this);
            MainFrame.Navigate(conversionPage);
        }
    }
}
