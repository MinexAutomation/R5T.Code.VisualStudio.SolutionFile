using System;
using System.Collections.Generic;


namespace R5T.Code.VisualStudio.Model
{
    public class SolutionFile
    {
        /// <summary>
        /// Example: the "12.00" in "Microsoft Visual Studio Solution File, Format Version 12.00".
        /// </summary>
        public Version FormatVersion { get; set; }
        /// <summary>
        /// Example: "# Visual Studio 15".
        /// </summary>
        public string VisualStudioMoniker { get; set; }
        /// <summary>
        /// Example: "VisualStudioVersion = 15.0.26124.0".
        /// </summary>
        public Version VisualStudioVersion { get; set; }
        /// <summary>
        /// Example: "MinimumVisualStudioVersion = 15.0.26124.0".
        /// </summary>
        public Version MinimumVisualStudioVersion { get; set; }
        /// <summary>
        /// List of solution project references.
        /// </summary>
        public List<SolutionFileProjectReference> SolutionFileProjectReferences { get; } = new List<SolutionFileProjectReference>();
        /// <summary>
        /// List of solution file global sections.
        /// </summary>
        public List<ISolutionFileGlobalSection> GlobalSections { get; } = new List<ISolutionFileGlobalSection>();
    }
}
