using System;

using SolutionUtilitiesExtra = R5T.Code.VisualStudio.Model.SolutionFileSpecific.UtilitiesExtra;


namespace R5T.Code.VisualStudio.Model
{
    public static class SolutionFileExtensions
    {
        /// <summary>
        /// Adds a project file and all its dependencies to a solution in the dependencies solution folder.
        /// </summary>
        public static void AddProjectReferenceDependencyAndAllDependenciesChecked(this SolutionFile solutionFile, string solutionFilePath, string dependencyProjectFilePath)
        {
            SolutionUtilitiesExtra.AddProjectReferenceDependencyAndAllDependenciesChecked(solutionFile, solutionFilePath, dependencyProjectFilePath);
        }
    }
}
