using System;
using System.Text.RegularExpressions;


namespace R5T.Code.VisualStudio.Model
{
    public class ProjectNesting
    {
        #region Static

        public static ProjectNesting Deserialize(string line)
        {
            var matches = Regex.Matches(line, @"{[^}]*}");

            var projectGuidStr = matches[0].Value;
            var projectParentGuidStr = matches[1].Value;

            var projectGUID = Guid.Parse(projectGuidStr);
            var projectParentGuid = Guid.Parse(projectParentGuidStr);

            var projectNesting = new ProjectNesting
            {
                ProjectGUID = projectGUID,
                ParentProjectGUID = projectParentGuid
            };
            return projectNesting;
        }

        public static string Serialize(ProjectNesting projectNesting)
        {
            var line = projectNesting.ToString();
            return line;
        }

        #endregion


        public Guid ProjectGUID { get; set; }
        public Guid ParentProjectGUID { get; set; }


        public override string ToString()
        {
            var representation = $"{this.ProjectGUID.ToString("B").ToUpperInvariant()} = {this.ParentProjectGUID.ToString("B").ToUpperInvariant()}";
            return representation;
        }
    }
}
