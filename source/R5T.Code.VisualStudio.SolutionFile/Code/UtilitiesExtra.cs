using System;

using ProjectFileUtilities = R5T.Code.VisualStudio.ProjectFile.Types.Utilities;


namespace R5T.Code.VisualStudio.Model.SolutionFileSpecific
{
    public static class UtilitiesExtra
    {
        /// <summary>
        /// Adds a project file and all its dependencies to a solution in the dependencies solution folder.
        /// </summary>
        public static void AddProjectReferenceDependencyAndAllDependenciesChecked(string solutionFilePath, string dependencyProjectFilePath)
        {
            var solutionFile = SolutionFile.Load(solutionFilePath);

            UtilitiesExtra.AddProjectReferenceDependencyAndAllDependenciesChecked(solutionFile, solutionFilePath, dependencyProjectFilePath);

            solutionFile.Save(solutionFilePath);
        }

        /// <summary>
        /// Adds a project file and all its dependencies to a solution in the dependencies solution folder.
        /// </summary>
        public static void AddProjectReferenceDependencyAndAllDependenciesChecked(SolutionFile solutionFile, string solutionFilePath, string dependencyProjectFilePath)
        {
            var dependencyProjectFilePaths = ProjectFileUtilities.GetProjectReferenceDependencyFilePathsRecursive(dependencyProjectFilePath);

            solutionFile.AddProjectReferenceDependencyAndAllDependenciesChecked(solutionFilePath, dependencyProjectFilePath, dependencyProjectFilePaths);
        }
    }
}
