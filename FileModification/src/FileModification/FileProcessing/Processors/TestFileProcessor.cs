using System.IO;
using System.Threading.Tasks;
using FileModification.Models;
using FileModification.Resources;

namespace FileModification.FileProcessing.Processors;

public class TestFileProcessor : IFileProcessor
{
    public async Task<Result<bool>> ProcessFile(Stream fileStream)
    {
        if (!fileStream.CanWrite)
        {
            return new Result<bool>(false, ErrorMessages.FileProcessor_CannotWriteFile);
        }

        const string TEXT_TO_APPEND = "Test line to prove processor is working.";

        await using var streamWriter = new StreamWriter(fileStream);
        await streamWriter.WriteLineAsync(TEXT_TO_APPEND);

        return new Result<bool>(true, string.Empty, true);
    }
}