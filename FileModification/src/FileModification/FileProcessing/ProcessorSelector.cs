using FileModification.FileProcessing.Processors;

namespace FileModification.FileProcessing;

public static class ProcessorSelector
{
    public static IFileProcessor? GetFileProcessor(string fileTypeCode)
    {
        return fileTypeCode switch
        {
            ProcessorFileCodes.TEST_FILE_CODE => new TestFileProcessor(),
            _ => null!
        };
    }
}