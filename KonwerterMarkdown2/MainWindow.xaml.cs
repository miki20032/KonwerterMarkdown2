using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Markdig;
using HtmlAgilityPack;
using Microsoft.Win32;
using ReverseMarkdown;

namespace MarkdownHtmlConverter
{
    public partial class MainWindow : Window
    {
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

        private void ConvertDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderDialog.SelectedPath, "*.md");
                ProgressBar.Maximum = files.Length;
                ProgressBar.Value = 0;

                foreach (var file in files)
                {
                    string markdown = File.ReadAllText(file);
                    if (!string.IsNullOrEmpty(markdown))
                    {
                        string html = Markdown.ToHtml(markdown);
                        string htmlFile = Path.ChangeExtension(file, ".html");
                        File.WriteAllText(htmlFile, html);
                    }
                    else
                    {
                        MessageBox.Show($"File {file} is empty.");
                    }
                    ProgressBar.Value += 1;
                }
            }
        }

        private void CancelOperation_Click(object sender, RoutedEventArgs e)
        {
            // Logic to cancel the operation (if applicable)
        }
    }
}
