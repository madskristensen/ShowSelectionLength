using System;
using System.ComponentModel.Composition;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

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

                foreach (SnapshotSpan snapshotSpan in selection.SelectedSpans)
                {
                    length += snapshotSpan.Length;
                }

                if (length > 0)
                {
                    await VS.StatusBar.ShowMessageAsync($"Selection {length}");
                }

            }).FireAndForget();
        }
    }
}
