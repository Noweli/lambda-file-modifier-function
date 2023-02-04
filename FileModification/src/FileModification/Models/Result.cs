namespace FileModification.Models;

public class Result<T>
{
    public bool IsSuccessful { get; }
    public string ErrorMessage { get; }
    public T ResultObject { get; }

    public Result(bool isSuccessful, string errorMessage = "", T resultObject = default!)
    {
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
        ResultObject = resultObject;
    }
}