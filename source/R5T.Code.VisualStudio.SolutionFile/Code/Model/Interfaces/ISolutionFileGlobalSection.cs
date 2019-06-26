using System;


namespace R5T.Code.VisualStudio.Model
{
    public interface ISolutionFileGlobalSection
    {
        string Name { get; }
        PreOrPostSolution PreOrPostSolution { get; }
    }
}
