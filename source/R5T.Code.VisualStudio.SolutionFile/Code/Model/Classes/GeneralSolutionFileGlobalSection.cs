using System;
using System.Collections.Generic;


namespace R5T.Code.VisualStudio.Model
{
    public class GeneralSolutionFileGlobalSection : SolutionFileGlobalSectionBase
    {
        public List<string> Lines { get; } = new List<string>();
        public override IEnumerable<string> ContentLines => this.Lines;
    }
}
