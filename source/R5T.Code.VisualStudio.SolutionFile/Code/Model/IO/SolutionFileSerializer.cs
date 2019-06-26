using System;

using R5T.NetStandard.IO;
using R5T.NetStandard.IO.Serialization;

using R5T.Code.VisualStudio.Model;


namespace R5T.Code.VisualStudio.IO
{
    /// <summary>
    /// Special solution file serializer that properly adds byte-order-marks (BOM) to solution file.
    /// </summary>
    public class SolutionFileSerializer : IFileSerializer<SolutionFile>
    {
        public SolutionFile Deserialize(string filePath)
        {
            using (var fileStream = FileStreamHelper.NewRead(filePath))
            using (var textReader = StreamReaderHelper.NewLeaveOpen(fileStream))
            {
                var solutionFileTextSerialializer = new SolutionFileTextSerializer();

                var solutionFile = solutionFileTextSerialializer.Deserialize(textReader);
                return solutionFile;
            }
        }

        public void Serialize(string filePath, SolutionFile obj, bool overwrite = true)
        {
            using (var fileStream = FileStreamHelper.NewWrite(filePath, overwrite))
            using (var textWriter = StreamWriterHelper.NewLeaveOpen(fileStream))
            {
                var solutionFileTextSerialializer = new SolutionFileTextSerializer();

                solutionFileTextSerialializer.Serialize(textWriter, obj);
            }
        }
    }
}
