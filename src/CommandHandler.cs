﻿using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace ShowSelectionLength
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class CommandHandler : WpfTextViewCreationListener
    {
        protected override void Created(DocumentView docView)
        {
            docView.TextView.Selection.SelectionChanged += SelectionChanged;
        }

        protected override void Closed(IWpfTextView textView)
        {
            textView.Selection.SelectionChanged -= SelectionChanged;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                var selection = (ITextSelection)sender;

                if (selection.IsEmpty)
                {
                    await VS.StatusBar.ClearAsync();

                    return;
                }

                var length = 0;
                var linesCount = 0;
                var lastLine = -1;

                foreach (SnapshotSpan snapshotSpan in selection.SelectedSpans)
                {
                    length += snapshotSpan.Length;
                    var snapshot = snapshotSpan.Snapshot;
                    var startLine = snapshot.GetLineNumberFromPosition(snapshotSpan.Start);
                    var endLine = snapshot.GetLineNumberFromPosition(snapshotSpan.End);

                    linesCount += endLine - startLine;
                    // if previous selection was not on the same line as start of this selection add 1 else the line will be counted twice
                    if (lastLine != startLine) linesCount++;
                    lastLine = endLine;
                }

                if (length > 0)
                {
                    await VS.StatusBar.ShowMessageAsync($"Selection Ln: {linesCount} Ch: {length}");
                }

            }).FireAndForget();
        }
    }
}
