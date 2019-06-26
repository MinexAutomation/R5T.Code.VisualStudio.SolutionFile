using System;
using System.Collections.Generic;


namespace R5T.Code.VisualStudio.Model
{
    public class NestedProjectsSolutionFileGlobalSection : SolutionFileGlobalSectionBase
    {
        public const string NestedProjectsGlobalSectionName = "NestedProjects";


        public List<ProjectNesting> ProjectNestings { get; } = new List<ProjectNesting>();
        public override IEnumerable<string> ContentLines
        {
            get
            {
                foreach (var projectNesting in this.ProjectNestings)
                {
                    var line = ProjectNesting.Serialize(projectNesting);
                    yield return line;
                }
            }
        }
    }
}
