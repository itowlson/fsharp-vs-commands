using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hestia.FSharpCommands.Utils;
using Microsoft.FSharp.Compiler;

namespace Hestia.FSharpCommands.Commands
{
    public class FormatSelectionCommand : FormatCommand
    {
        public override void Execute()
        {
            if (TextView.Selection.IsEmpty)
            {
                MessageBox.Show("No selection");
                return;
            }

            using (Cursor.Wait())
            {
                ExecuteFormat();
            }
        }

        protected override string GetFormatted(bool isSignatureFile, string source, Fantomas.FormatConfig.FormatConfig config)
        {
            // This still seems to give "The indent level cannot be negative"
            // in a lot of cases that feel like they should work, e.g. 'let' forms

            Range.pos startPos = TextUtils.GetFSharpPos(TextView.Selection.Start);
            Range.pos endPos = TextUtils.GetFSharpPos(TextView.Selection.End);
            Range.range range = Range.mkRange("fsfile", startPos, endPos);

            return Fantomas.CodeFormatter.formatSelectionFromString(isSignatureFile, range, source, config);
        }
    }
}
