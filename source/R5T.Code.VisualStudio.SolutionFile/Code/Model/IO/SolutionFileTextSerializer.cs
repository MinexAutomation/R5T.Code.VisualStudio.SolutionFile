using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using R5T.NetStandard.IO.Serialization;

using R5T.Code.VisualStudio.Model;


namespace R5T.Code.VisualStudio.IO
{
    public class SolutionFileTextSerializer : ITextSerializer<SolutionFile>
    {
        public const string ProjectLineRegexPattern = @"^Project";
        public const string ProjectLineEndRegexPattern = @"^EndProject";
        public const string GlobalLineRegexPattern = @"^Global";
        public const string GlobalEndLineRegexPattern = @"^EndGlobal";
        public const string GlobalSectionRegexPattern = @"^GlobalSection";
        public const string GlobalSectionEndRegexPattern = @"^EndGlobalSection";

        public const string ProjectLineValuesRegexPattern = @"""[^""]*"""; // Find matches in quotes.
        public const string GlobalSectionLineValuesRegexPattern = @"\(.*\)|= .*";



        private Regex ProjectLineRegex { get; } = new Regex(SolutionFileTextSerializer.ProjectLineRegexPattern);
        private Regex ProjectLineEndRegex { get; } = new Regex(SolutionFileTextSerializer.ProjectLineEndRegexPattern);
        private Regex GlobalLineRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalLineRegexPattern);
        private Regex GlobalLineEndRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalEndLineRegexPattern);
        private Regex GlobalSectionRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalSectionRegexPattern);
        private Regex GlobalSectionEndRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalSectionEndRegexPattern);


        public SolutionFile Deserialize(TextReader reader)
        {
            var solutionFile = new SolutionFile();

            var blankBeginLine = reader.ReadLine();
            var formatVersionLine = reader.ReadLine();
            var monikerLine = reader.ReadLine();
            var vsVersionLine = reader.ReadLine();
            var vsMinimumVersionLine = reader.ReadLine();

            var formatVersionTokens = formatVersionLine.Split(' ');
            var formatVersionToken = formatVersionTokens.Last();
            solutionFile.FormatVersion = Version.Parse(formatVersionToken);

            solutionFile.VisualStudioMoniker = monikerLine;

            var vsVersionTokens = vsVersionLine.Split(' ');
            var vsVersionToken = vsVersionTokens.Last();
            solutionFile.VisualStudioVersion = Version.Parse(vsVersionToken);

            var vsMinimumVersionTokens = vsMinimumVersionLine.Split(' ');
            var vsMinimumVersionToken = vsMinimumVersionTokens.Last();
            solutionFile.MinimumVisualStudioVersion = Version.Parse(vsMinimumVersionToken);

            var currentLine = reader.ReadLine();

            if(this.ProjectLineRegex.IsMatch(currentLine))
            {
                this.DeserializeProjects(reader, ref currentLine, solutionFile);
            }

            if(!this.GlobalLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Global\".\nFound: {currentLine}");
            }

            this.DeserializeGlobals(reader, ref currentLine, solutionFile);

            var blankEndLine = reader.ReadLine();

            if(reader.ReadToEnd() != String.Empty)
            {
                throw new Exception("Reader was not at end.");
            }

            return solutionFile;
        }

        private void DeserializeGlobals(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!this.GlobalLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Global\".\nFound: {currentLine}");
            }

            currentLine = reader.ReadLine().Trim();

            while(!this.GlobalLineEndRegex.IsMatch(currentLine))
            {
                this.DeserializeGlobal(reader, ref currentLine, solutionFile);
            }
        }

        private void DeserializeGlobal(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!this.GlobalSectionRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"GlobalSection\".\nFound: {currentLine}");
            }

            var globalSectionMatches = Regex.Matches(currentLine, SolutionFileTextSerializer.GlobalSectionLineValuesRegexPattern);

            currentLine = reader.ReadLine().Trim();

            while(!this.GlobalSectionEndRegex.IsMatch(currentLine))
            {

                currentLine = reader.ReadLine().Trim();
            }
        }

        private void DeserializeProjects(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!this.ProjectLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Project...\".\nFound: {currentLine}");
            }

            while (!this.GlobalLineRegex.IsMatch(currentLine))
            {
                this.DeserializeProject(reader, ref currentLine, solutionFile);

                currentLine = reader.ReadLine();
            }
        }

        private void DeserializeProject(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!this.ProjectLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Project...\".\nFound: {currentLine}");
            }

            var matches = Regex.Matches(currentLine, SolutionFileTextSerializer.ProjectLineValuesRegexPattern);

            var projectTypeGUIDStr = matches[0].Value;
            var projectName = matches[1].Value;
            var projectFileRelativePathValue = matches[2].Value;
            var projectGUIDStr = matches[3].Value;

            var projectTypeGUID = Guid.Parse(projectTypeGUIDStr);
            var projectGUID = Guid.Parse(projectGUIDStr);

            var solutionProjectFileReference = new SolutionFileProjectReference
            {
                ProjectTypeGUID = projectTypeGUID,
                ProjectName = projectName,
                ProjectFileRelativePathValue = projectFileRelativePathValue,
                ProjectGUID = projectGUID
            };

            solutionFile.SolutionFileProjectReferences.Add(solutionProjectFileReference);

            currentLine = reader.ReadLine();
            if(!this.ProjectLineEndRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"EndProject\".\nFound: {currentLine}");
            }
        }

        public void Serialize(TextWriter writer, SolutionFile value)
        {
            throw new NotImplementedException();
        }
    }
}
