using System;
using System.Collections.Generic;


namespace R5T.Code.VisualStudio.Model
{
    public class GeneralSolutionFileGlobalSection : ISolutionFileGlobalSection, ISerializableSolutionFileGlobalSection
    {
        public string Name { get; set; }
        public PreOrPostSolution PreOrPostSolution { get; set; }
        public List<string> Lines { get; } = new List<string>();
        IEnumerable<string> ISerializableSolutionFileGlobalSection.ContentLines => this.Lines;
    }
}
