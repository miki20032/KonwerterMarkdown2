using System;
using System.IO;
using System.Threading;
using Markdig;
using ReverseMarkdown;

namespace ClassLibrary2
{
    public class Class1
    {
        public string ConvertMarkdownToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown);
        }

        public string ConvertHtmlToMarkdown(string html)
        {
            var config = new ReverseMarkdown.Config
            {
                GithubFlavored = true,
                RemoveComments = true,
                SmartHrefHandling = true
            };
            var converter = new ReverseMarkdown.Converter(config);
            return converter.Convert(html);
        }

        public void ConvertDirectoryToHtml(string directoryPath, IProgress<int> progress, CancellationToken cancellationToken)
        {
            string[] files = Directory.GetFiles(directoryPath, "*.md");
            int totalFiles = files.Length;

            for (int i = 0; i < totalFiles; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string file = files[i];
                string markdown = File.ReadAllText(file);
                if (!string.IsNullOrEmpty(markdown))
                {
                    string html = ConvertMarkdownToHtml(markdown);
                    string htmlFile = Path.ChangeExtension(file, ".html");
                    File.WriteAllText(htmlFile, html);
                }

                progress.Report((i + 1) * 100 / totalFiles);
            }
        }

        public void ConvertDirectoryToMarkdown(string directoryPath, IProgress<int> progress, CancellationToken cancellationToken)
        {
            string[] files = Directory.GetFiles(directoryPath, "*.html");
            int totalFiles = files.Length;

            for (int i = 0; i < totalFiles; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string file = files[i];
                string html = File.ReadAllText(file);
                if (!string.IsNullOrEmpty(html))
                {
                    string markdown = ConvertHtmlToMarkdown(html);
                    string mdFile = Path.ChangeExtension(file, ".md");
                    File.WriteAllText(mdFile, markdown);
                }

                progress.Report((i + 1) * 100 / totalFiles);
            }
        }
    }
}
