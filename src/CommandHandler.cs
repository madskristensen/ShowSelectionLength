using EnvDTE;
using Microsoft;
using Microsoft.Build.Framework.XamlTypes;
using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace ShowSelectionLength
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class CommandHandler : IWpfTextViewCreationListener
    {
        private static DTE _dte;

        public void TextViewCreated(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_dte == null)
            {
                _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
                Assumes.Present(_dte);
            }

            textView.Selection.SelectionChanged += SelectionChanged;
            textView.Closed += TextView_Closed;
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            var textView = (IWpfTextView)sender;
            textView.Selection.SelectionChanged -= SelectionChanged;
            textView.Closed -= TextView_Closed;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                var selection = (ITextSelection)sender;

                if (selection.IsEmpty)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    _dte.StatusBar.Clear();

                    return;
                }

                int length = 0;

                foreach (SnapshotSpan snapshotSpan in selection.SelectedSpans)
                {
                    length += snapshotSpan.Length;
                }

                if (length > 0)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    _dte.StatusBar.Text = $"Selection {length}";
                }
            }).FileAndForget(nameof(ShowSelectionLength));
        }
    }
}
