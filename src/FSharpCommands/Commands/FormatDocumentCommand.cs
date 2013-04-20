using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace Hestia.FSharpCommands.Commands
{
    public class FormatDocumentCommand : CommandBase
    {
        public override void Execute()
        {
            string text = TextView.TextSnapshot.GetText();

            ITextBuffer buffer = TextView.TextBuffer;
            //ITextSnapshot snapshot = buffer.CurrentSnapshot;
            //IEditorOptions editorOptions = _controller._provider.EditorOptionsFactory.GetOptions(buffer);
            //int tabSize = editorOptions.GetOptionValue<int>(new TabSize().Key);
            //int indentSize = editorOptions.GetOptionValue<int>(new IndentSize().Key);

            string source;

            using (var writer = new StringWriter())
            {
                buffer.CurrentSnapshot.Write(writer);
                writer.Flush();
                source = writer.ToString();
            }

            var config = Fantomas.FormatConfig.FormatConfig.Default;  // TODO: for now

            var formatted = Fantomas.CodeFormatter.formatSourceString(false /* TODO: handle FSI */, source, config);

            using (var edit = buffer.CreateEdit())
            {
                edit.Replace(0, source.Length, formatted);
                edit.Apply();
            }
        }
    }
}
