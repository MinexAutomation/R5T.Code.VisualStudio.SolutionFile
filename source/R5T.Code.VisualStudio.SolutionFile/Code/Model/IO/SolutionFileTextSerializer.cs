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
        public const string GlobalEndLineRegexPattern = @"^EndGlobal($|\s)";
        public const string GlobalSectionRegexPattern = @"^GlobalSection";
        public const string GlobalSectionEndRegexPattern = @"^EndGlobalSection";

        public const string ProjectLineValuesRegexPattern = @"""([^""]*)"""; // Find matches in quotes, but includes the quotes.
        public const string GlobalSectionLineValuesRegexPattern = @"\(.*\)|(?<== ).*";


        #region Static

        private static Regex ProjectLineRegex { get; } = new Regex(SolutionFileTextSerializer.ProjectLineRegexPattern);
        private static Regex ProjectLineEndRegex { get; } = new Regex(SolutionFileTextSerializer.ProjectLineEndRegexPattern);
        private static Regex GlobalLineRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalLineRegexPattern);
        private static Regex GlobalLineEndRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalEndLineRegexPattern);
        private static Regex GlobalSectionRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalSectionRegexPattern);
        private static Regex GlobalSectionEndRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalSectionEndRegexPattern);


        private static void SerializeGlobals(TextWriter writer, SolutionFile solutionFile)
        {
            var tabinatedWriter = new TabinatedWriter(writer);

            tabinatedWriter.WriteLine("Global");

            tabinatedWriter.IncreaseTabination();
            foreach (var globalSection in solutionFile.GlobalSections)
            {
                if(globalSection is ISerializableSolutionFileGlobalSection serializableGlobalSection)
                {
                    SolutionFileTextSerializer.SerializeGlobal(tabinatedWriter, serializableGlobalSection);
                }
                else
                {
                    throw new IOException($"Unable to serialize solution file global section type: {globalSection.GetType().FullName}");
                }
            }
            tabinatedWriter.DecreaseTabination();

            tabinatedWriter.WriteLine("EndGlobal");
        }

        private static void SerializeGlobal(TabinatedWriter writer, ISerializableSolutionFileGlobalSection serializableGlobalSection)
        {
            var globalSectionLine = $"GlobalSection({serializableGlobalSection.Name}) = {Utilities.ToStringStandard(serializableGlobalSection.PreOrPostSolution)}";
            writer.WriteLine(globalSectionLine);

            writer.IncreaseTabination();
            foreach (var line in serializableGlobalSection.ContentLines)
            {
                writer.WriteLine(line);
            }
            writer.DecreaseTabination();

            writer.WriteLine("EndGlobalSection");
        }

        private static void SerializeProjectReferences(TextWriter writer, SolutionFile solutionFile)
        {
            foreach (var solutionProjectFileReference in solutionFile.SolutionFileProjectReferences)
            {
                var projectLine = $@"Project(""{solutionProjectFileReference.ProjectTypeGUID.ToString("B").ToUpperInvariant()}"") = ""{solutionProjectFileReference.ProjectName}"", ""{solutionProjectFileReference.ProjectFileRelativePathValue}"", ""{solutionProjectFileReference.ProjectGUID.ToString("B").ToUpperInvariant()}""";
                writer.WriteLine(projectLine);

                writer.WriteLine("EndProject");
            }
        }

        private static void DeserializeGlobals(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.GlobalLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Global\".\nFound: {currentLine}");
            }

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalLineEndRegex.IsMatch(currentLine))
            {
                SolutionFileTextSerializer.DeserializeGlobal(reader, ref currentLine, solutionFile);

                currentLine = reader.ReadLine().Trim();
            }
        }

        private static void DeserializeGlobal(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.GlobalSectionRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"GlobalSection\".\nFound: {currentLine}");
            }

            var globalSectionMatches = Regex.Matches(currentLine, SolutionFileTextSerializer.GlobalSectionLineValuesRegexPattern);

            var sectionName = globalSectionMatches[0].Value.TrimStart('(').TrimEnd(')');
            var preOrPostSolutionStr = globalSectionMatches[1].Value;

            var preOrPostSolution = Utilities.ToPreOrPostSolution(preOrPostSolutionStr);

            ISolutionFileGlobalSection globalSection;
            if (sectionName == NestedProjectsSolutionFileGlobalSection.NestedProjectsGlobalSectionName)
            {
                globalSection = SolutionFileTextSerializer.DeserializeNestedProjectsGlobalSection(reader, ref currentLine, sectionName, preOrPostSolution);
            }
            else
            {
                globalSection = SolutionFileTextSerializer.DeserializeGeneralGlobal(reader, ref currentLine, sectionName, preOrPostSolution);
            }
            solutionFile.GlobalSections.Add(globalSection);
        }

        private static NestedProjectsSolutionFileGlobalSection DeserializeNestedProjectsGlobalSection(TextReader reader, ref string currentLine, string sectionName, PreOrPostSolution preOrPostSolution)
        {
            var nestedProjectGlobalSection = new NestedProjectsSolutionFileGlobalSection
            {
                Name = sectionName,
                PreOrPostSolution = preOrPostSolution
            };

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                var projectNesting = ProjectNesting.Deserialize(currentLine);
                nestedProjectGlobalSection.ProjectNestings.Add(projectNesting);

                currentLine = reader.ReadLine().Trim();
            }

            return nestedProjectGlobalSection;
        }

        private static GeneralSolutionFileGlobalSection DeserializeGeneralGlobal(TextReader reader, ref string currentLine, string sectionName, PreOrPostSolution preOrPostSolution)
        {
            var globalSection = new GeneralSolutionFileGlobalSection
            {
                Name = sectionName,
                PreOrPostSolution = preOrPostSolution,
            };

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                globalSection.Lines.Add(currentLine);

                currentLine = reader.ReadLine().Trim();
            }

            return globalSection;
        }

        private static void DeserializeProjects(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.ProjectLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Project...\".\nFound: {currentLine}");
            }

            while (!SolutionFileTextSerializer.GlobalLineRegex.IsMatch(currentLine))
            {
                SolutionFileTextSerializer.DeserializeProject(reader, ref currentLine, solutionFile);

                currentLine = reader.ReadLine();
            }
        }

        private static void DeserializeProject(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.ProjectLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Project...\".\nFound: {currentLine}");
            }

            var matches = Regex.Matches(currentLine, SolutionFileTextSerializer.ProjectLineValuesRegexPattern);

            var projectTypeGUIDStr = matches[0].Value.Trim('"');
            var projectName = matches[1].Value.Trim('"');
            var projectFileRelativePathValue = matches[2].Value.Trim('"');
            var projectGUIDStr = matches[3].Value.Trim('"');

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
            if (!SolutionFileTextSerializer.ProjectLineEndRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"EndProject\".\nFound: {currentLine}");
            }
        }

        #endregion


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

            if(SolutionFileTextSerializer.ProjectLineRegex.IsMatch(currentLine))
            {
                SolutionFileTextSerializer.DeserializeProjects(reader, ref currentLine, solutionFile);
            }

            if(!SolutionFileTextSerializer.GlobalLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Global\".\nFound: {currentLine}");
            }

            SolutionFileTextSerializer.DeserializeGlobals(reader, ref currentLine, solutionFile);

            var blankEndLine = reader.ReadLine();

            if(reader.ReadToEnd() != String.Empty)
            {
                throw new Exception("Reader was not at end.");
            }

            return solutionFile;
        }

        

        public void Serialize(TextWriter writer, SolutionFile solutionFile)
        {
            writer.WriteLine(); // Blank first line.

            var formatVersionLine = $"Microsoft Visual Studio Solution File, Format Version {solutionFile.FormatVersion.Major}.{solutionFile.FormatVersion.Minor:00}";
            writer.WriteLine(formatVersionLine);
            writer.WriteLine(solutionFile.VisualStudioMoniker);
            var vsVersionLine = $"VisualStudioVersion = {solutionFile.VisualStudioVersion}";
            writer.WriteLine(vsVersionLine);
            var vsMinimumVersionLine = $"MinimumVisualStudioVersion = {solutionFile.MinimumVisualStudioVersion}";
            writer.WriteLine(vsMinimumVersionLine);

            SolutionFileTextSerializer.SerializeProjectReferences(writer, solutionFile);

            SolutionFileTextSerializer.SerializeGlobals(writer, solutionFile);

            // Blank last line for free due to prior WriteLine().
        }
    }
}
