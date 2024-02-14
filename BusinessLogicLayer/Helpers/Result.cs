namespace BusinessLogicLayer.Helpers;
public class Result<T>
{
    public bool Status { get; }
    public string Message { get; }
    public List<T> Data { get; }

    public Result(bool status, string message, List<T> Data)
    {
        Status = status;
        Message = message;
        this.Data = Data;
    }
}