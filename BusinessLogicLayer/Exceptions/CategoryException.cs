namespace BusinessLogicLayer.Exceptions;
public class CategoryException : Exception
{
    public readonly string ErrorMessage;

    public CategoryException(string message)
    {
        ErrorMessage = message;
    }
}