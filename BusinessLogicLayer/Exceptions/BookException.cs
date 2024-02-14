public class BookException : Exception
{
    public readonly string ErrorMessage;

    public BookException(string message)
    {
        ErrorMessage = message;
    }
}