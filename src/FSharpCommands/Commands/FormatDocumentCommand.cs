using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.FSharpCommands.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Hestia.FSharpCommands.Commands
{
    public class FormatDocumentCommand : CommandBase
    {
        public override void Execute()
        {
            using (Cursor.Wait())
            {
                ExecuteFormat();
            }
        }

        private void ExecuteFormat()
        {
            string text = TextView.TextSnapshot.GetText();

            ITextBuffer buffer = TextView.TextBuffer;

            IEditorOptions editorOptions = Services.EditorOptionsFactory.GetOptions(buffer);
            int indentSize = editorOptions.GetOptionValue<int>(new IndentSize().Key);
            FantomasOptionsPage customOptions = (FantomasOptionsPage)(Package.GetGlobalService(typeof(FantomasOptionsPage)));

            string source;

            using (var writer = new StringWriter())
            {
                buffer.CurrentSnapshot.Write(writer);
                writer.Flush();
                source = writer.ToString();
            }

            var isSignatureFile = IsSignatureFile(buffer);

            var config = new Fantomas.FormatConfig.FormatConfig(
                indentSpaceNum: indentSize,
                pageWidth: customOptions.PageWidth,
                semicolonAtEndOfLine: customOptions.SemicolonAtEndOfLine,
                spaceBeforeArgument: customOptions.SpaceBeforeArgument,
                spaceBeforeColon: customOptions.SpaceBeforeColon,
                spaceAfterComma: customOptions.SpaceAfterComma,
                spaceAfterSemicolon: customOptions.SpaceAfterSemicolon,
                indentOnTryWith: customOptions.IndentOnTryWith
                );

            var formatted = Fantomas.CodeFormatter.formatSourceString(isSignatureFile, source, config);

            using (var edit = buffer.CreateEdit())
            {
                edit.Replace(0, source.Length, formatted);
                edit.Apply();
            }
        }

        private static bool IsSignatureFile(ITextBuffer buffer)
        {
            ITextDocument document = buffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));
            var fileExtension = Path.GetExtension(document.FilePath);
            var isSignatureFile = ".fsi".Equals(fileExtension, StringComparison.OrdinalIgnoreCase);  // There isn't a distinct content type for FSI files, so we have to use the file extension
            return isSignatureFile;
        }
    }
}
