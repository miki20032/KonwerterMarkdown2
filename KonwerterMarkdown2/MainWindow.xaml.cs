using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Markdig;
using Microsoft.Win32;
using ReverseMarkdown;

namespace MarkdownHtmlConverter
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConvertToHtml_Click(object sender, RoutedEventArgs e)
        {
            string markdown = MarkdownTextBox.Text;
            if (!string.IsNullOrEmpty(markdown))
            {
                string html = Markdown.ToHtml(markdown);
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
                var config = new ReverseMarkdown.Config
                {
                    GithubFlavored = true,
                    RemoveComments = true,
                    SmartHrefHandling = true
                };
                var converter = new ReverseMarkdown.Converter(config);
                string markdown = converter.Convert(html);
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

        private async void ConvertDirectoryToHtml_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                string[] files = Directory.GetFiles(folderDialog.SelectedPath, "*.md");
                ProgressBar.Maximum = files.Length;
                ProgressBar.Value = 0;

                try
                {
                    await Task.Run(() =>
                    {
                        foreach (var file in files)
                        {
                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            string markdown = File.ReadAllText(file);
                            if (!string.IsNullOrEmpty(markdown))
                            {
                                string html = Markdown.ToHtml(markdown);
                                string htmlFile = Path.ChangeExtension(file, ".html");
                                File.WriteAllText(htmlFile, html);
                            }
                            else
                            {
                                Dispatcher.Invoke(() => MessageBox.Show($"File {file} is empty."));
                            }
                            Dispatcher.Invoke(() => ProgressBar.Value += 1);
                        }
                    }, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation canceled.");
                }
            }
        }

        private async void ConvertDirectoryToMarkdown_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                string[] files = Directory.GetFiles(folderDialog.SelectedPath, "*.html");
                ProgressBar.Maximum = files.Length;
                ProgressBar.Value = 0;

                try
                {
                    await Task.Run(() =>
                    {
                        var config = new ReverseMarkdown.Config
                        {
                            GithubFlavored = true,
                            RemoveComments = true,
                            SmartHrefHandling = true
                        };
                        var converter = new ReverseMarkdown.Converter(config);

                        foreach (var file in files)
                        {
                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            string html = File.ReadAllText(file);
                            if (!string.IsNullOrEmpty(html))
                            {
                                string markdown = converter.Convert(html);
                                string mdFile = Path.ChangeExtension(file, ".md");
                                File.WriteAllText(mdFile, markdown);
                            }
                            else
                            {
                                Dispatcher.Invoke(() => MessageBox.Show($"File {file} is empty."));
                            }
                            Dispatcher.Invoke(() => ProgressBar.Value += 1);
                        }
                    }, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation canceled.");
                }
            }
        }

        private void CancelOperation_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
