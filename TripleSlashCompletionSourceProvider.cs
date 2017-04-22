namespace CppTripleSlash
{
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;

    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("code")]
    [Name("xml doc comment completion")]
    public class TripleSlashCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import] 
        internal IGlyphService GlyphService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new TripleSlashCompletionSource(this, textBuffer);
        }
    }
}