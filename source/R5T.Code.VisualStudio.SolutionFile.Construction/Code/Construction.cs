using System;
using System.Text.RegularExpressions;

using R5T.Code.VisualStudio.IO;
using R5T.Code.VisualStudio.Model;
using R5T.NetStandard.IO;


namespace R5T.Code.VisualStudio.SolutionFile.Construction
{
    public static class Construction
    {
        public static void SubMain()
        {
            //Construction.EnsureBOMProduced();
            //Construction.EnsureNoBOMProduced();
            //Construction.TestRegexes();
            //Construction.DeserializeSolutionFile();
            Construction.RoundTripSerializeSolutionFile();
        }

        private static void RoundTripSerializeSolutionFile()
        {
            var inputSolutionFilePath = @"C:\Organizations\Rivet\Repositories\Libraries\R5T.Code.VisualStudio.Types\source\R5T.Code.VisualStudio.Types.Construction - Copy.sln";

            var solutionFileSerializer = new SolutionFileSerializer();

            var solutionFile = solutionFileSerializer.Deserialize(inputSolutionFilePath);

            var outputSolutionFilePath = @"C:\Temp\R5T.Code.VisualStudio.Types.Construction - Copy.sln";

            solutionFileSerializer.Serialize(outputSolutionFilePath, solutionFile);
        }

        private static void DeserializeSolutionFile()
        {
            var solutionFilePath = @"C:\Organizations\Rivet\Repositories\Libraries\R5T.Code.VisualStudio.Types\source\R5T.Code.VisualStudio.Types.Construction - Copy.sln";

            var solutionFileSerializer = new SolutionFileSerializer();
            var solutionFile = solutionFileSerializer.Deserialize(solutionFilePath);
        }

        private static void TestRegexes()
        {
            string line;
            string pattern;
            MatchCollection matches;

            line = @"Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""R5T.NetStandard.OS"", ""..\..\R5T.NetStandard.OS\source\R5T.NetStandard.OS\R5T.NetStandard.OS.csproj"", ""{84565B22-6C8C-48FE-9CD5-00648C68CD41}""";
            pattern = SolutionFileTextSerializer.ProjectLineValuesRegexPattern;
            matches = Regex.Matches(line, pattern);

            var projectTypeGUIDStr = matches[0].Value.Trim('"');
            var projectTypeGUID = Guid.Parse(projectTypeGUIDStr);

            line = @"GlobalSection(SolutionConfigurationPlatforms) = preSolution";
            pattern = SolutionFileTextSerializer.GlobalSectionLineValuesRegexPattern;
            matches = Regex.Matches(line, pattern);

            line = @"EndGlobal";
            pattern = SolutionFileTextSerializer.GlobalEndLineRegexPattern;
            matches = Regex.Matches(line, pattern);

            line = @"EndGlobalSection";
            pattern = SolutionFileTextSerializer.GlobalEndLineRegexPattern;
            matches = Regex.Matches(line, pattern);
        }

        private static void EnsureNoBOMProduced()
        {
            var tempFilePath = @"C:\Temp\temp.txt";

            using (var fileStream = FileStreamHelper.NewWrite(tempFilePath))
            using (var writer = StreamWriterHelper.NewLeaveOpen(fileStream))
            {
                writer.WriteLine("Hello world! (BOM?)");
            }
        }

        private static void EnsureBOMProduced()
        {
            var tempFilePath = @"C:\Temp\temp.txt";

            using (var fileStream = FileStreamHelper.NewWrite(tempFilePath))
            using (var writer = StreamWriterHelper.NewLeaveOpenAddBOM(fileStream))
            {
                writer.WriteLine("Hello world! (BOM?)");
            }
        }
    }
}
