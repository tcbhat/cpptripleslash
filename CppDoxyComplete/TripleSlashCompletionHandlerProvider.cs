namespace CppTripleSlash
{
    using EnvDTE;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IVsTextViewCreationListener))]
    [Name("C++ Triple Slash Completion Handler")]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class TripleSlashCompletionHandlerProvider : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService AdapterService = null;

        [Import]
        public ICompletionBroker CompletionBroker { get; set; }

        [Import]
        public SVsServiceProvider ServiceProvider { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            try
            {
                IWpfTextView textView = this.AdapterService.GetWpfTextView(textViewAdapter);
                if (textView == null)
                {
                    return;
                }

                Func<TripleSlashCompletionCommandHandler> createCommandHandler = delegate()
                {
                    var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
                    return new TripleSlashCompletionCommandHandler(textViewAdapter, textView, this, dte);
                };

                textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
            }
            catch
            {
            }
        }
    }
}
