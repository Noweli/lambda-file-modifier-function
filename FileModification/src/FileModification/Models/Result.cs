namespace FileModification.Models;

public class Result
{
    private bool IsSuccessful { get; }
    private string ErrorMessage { get; }

    public Result(bool isSuccessful, string errorMessage = "")
    {
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
    }
}