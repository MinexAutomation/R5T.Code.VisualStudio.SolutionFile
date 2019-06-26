using System;
using System.Linq;


namespace R5T.Code.VisualStudio.Model
{
    public static class SolutionFileExtensions
    {
        public static bool HasDependenciesSolutionFolder(this SolutionFile solutionFile, out SolutionFileProjectReference dependenciesSolutionFolder)
        {
            dependenciesSolutionFolder = solutionFile.SolutionFileProjectReferences.Where(x => x.ProjectName == Constants.DependenciesSolutionFolderName).SingleOrDefault();

            var hasDependenciesSolutionFolder = dependenciesSolutionFolder == default;
            return hasDependenciesSolutionFolder;
        }

        public static bool HasDependenciesSolutionFolder(this SolutionFile solutionFile)
        {
            var hasDependenciesSolutionFolder = solutionFile.HasDependenciesSolutionFolder(out var dummy);
            return hasDependenciesSolutionFolder;
        }

        public static SolutionFileProjectReference GetDependenciesSolutionFolder(this SolutionFile solutionFile)
        {
            var hasDependenciesSolutionFolder = solutionFile.HasDependenciesSolutionFolder(out var dependenciesSolutionFolder);
            if(!hasDependenciesSolutionFolder)
            {
                throw new InvalidOperationException($"Solution file had no {Constants.DependenciesSolutionFolderName} solution folder.");
            }

            return dependenciesSolutionFolder;
        }

        public static SolutionFileProjectReference AddDependenciesSolutionFolder(this SolutionFile solutionFile)
        {
            var dependenciesSolutionFolder = new SolutionFileProjectReference
            {
                ProjectTypeGUID = Constants.SolutionFolderProjectTypeGUID,
                ProjectName = Constants.DependenciesSolutionFolderName,
                ProjectFileRelativePathValue = Constants.DependenciesSolutionFolderName,
                ProjectGUID = Guid.NewGuid()
            };

            solutionFile.SolutionFileProjectReferences.Add(dependenciesSolutionFolder);

            return dependenciesSolutionFolder;
        }
    }
}
