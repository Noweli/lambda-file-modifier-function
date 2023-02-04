using System.IO;
using System.Threading.Tasks;
using FileModification.Models;

namespace FileModification.FileProcessing;

public interface IFileProcessor
{
    Task<Result<bool>> ProcessFile(Stream fileStream);
}