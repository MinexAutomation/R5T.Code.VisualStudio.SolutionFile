using System;


namespace R5T.Code.VisualStudio.Model
{
    /// <summary>
    /// Example:
    /// Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "R5T.Private.SimpleConsoleAndLib", "R5T.Private.SimpleConsoleAndLib\R5T.Private.SimpleConsoleAndLib.csproj", "{9DAD5F24-3C22-47C9-8D69-3C7D72C62DAD}"
    /// EndProject
    /// </summary>
    public class SolutionFileProjectReference
    {
        public Guid ProjectTypeGUID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectFileRelativePathValue { get; set; }
        public Guid ProjectGUID { get; set; }
    }
}
