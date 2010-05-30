﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Squiggle.UI.Controls
{
    /// <summary>
    /// Interaction logic for ChatTextBox.xaml
    /// </summary>
    public partial class ChatTextBox : UserControl
    {
        static Regex urlRegex = new Regex(@"https?://[\w./]+");

        public ChatTextBox()
        {
            InitializeComponent();

            sentMessages.Document = new FlowDocument();
            sentMessages.Document.Blocks.Add(new Paragraph());
        }

        public void AddError(string error, string detail)
        {
            var para = sentMessages.Document.Blocks.FirstBlock as Paragraph;

            var errorText = new Run(error);
            errorText.Foreground = new SolidColorBrush(Colors.Red);

            var detailText = new Run(detail);
            detailText.Foreground = new SolidColorBrush(Colors.Gray);

            para.Inlines.Add(new LineBreak());
            para.Inlines.Add(errorText);
            para.Inlines.Add(new LineBreak());
            para.Inlines.Add(detailText);
            para.Inlines.Add(new LineBreak());
            para.Inlines.Add(new LineBreak());

            scrollViewer.ScrollToBottom();
        }

        public void AddMessage(string user, string message)
        {
            var para = sentMessages.Document.Blocks.FirstBlock as Paragraph;

            var items = ParseText(user + ": ");
            foreach (var item in items)
            {
                item.FontWeight = FontWeights.Bold;
                para.Inlines.Add(item);
            }
            items = ParseText(message);
            para.Inlines.AddRange(items);
            para.Inlines.Add(new LineBreak());
            scrollViewer.ScrollToBottom();
        }

        static List<Inline> ParseText(string message)
        {
            var items = new List<Inline>();
            int lastIndex = 0;
            foreach (Match match in urlRegex.Matches(message))
            {
                string text = message.Substring(lastIndex, match.Index - lastIndex);
                AddText(items, text);
                AddHyperlink(items, match.Value);
                lastIndex = match.Index + match.Length;
            }
            AddText(items, message.Substring(lastIndex));
            return items;
        }

        static void AddText(List<Inline> items, string text)
        {
            if (!String.IsNullOrEmpty(text))
                items.Add(new Run(text));
        }

        static void AddHyperlink(List<Inline> items, string url)
        {
            var link = new Hyperlink(new Run(url));
            link.NavigateUri = new Uri(url, UriKind.Absolute);
            link.Cursor = Cursors.Hand;
            link.MouseLeftButtonDown += (s, e) =>
            {
                try
                {
                    Process.Start(link.NavigateUri.AbsoluteUri);
                    e.Handled = true;
                }
                catch { }
            };
            items.Add(link);
        }
    }
}
