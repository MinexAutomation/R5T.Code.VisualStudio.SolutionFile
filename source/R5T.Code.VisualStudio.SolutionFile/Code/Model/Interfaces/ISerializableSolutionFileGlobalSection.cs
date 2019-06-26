using System;
using System.Collections.Generic;


namespace R5T.Code.VisualStudio.Model
{
    public interface ISerializableSolutionFileGlobalSection
    {
        IEnumerable<string> ContentLines { get; }
    }
}
