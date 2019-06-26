using System;
using System.Collections.Generic;


namespace R5T.Code.VisualStudio.Model
{
    public abstract class SolutionFileGlobalSectionBase : ISerializableSolutionFileGlobalSection
    {
        public string Name { get; set; }
        public PreOrPostSolution PreOrPostSolution { get; set; }

        public abstract IEnumerable<string> ContentLines { get; }


        public override string ToString()
        {
            var representation = this.Name;
            return representation;
        }
    }
}
