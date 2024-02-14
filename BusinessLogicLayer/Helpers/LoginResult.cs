namespace BusinessLogicLayer.Helpers;

public class LoginResult
{
    public bool IsSuccessed { get; set; } = true;
    public List<string> ErrorMessages = new();
    public string Token { get; set; } = string.Empty;

    public LoginResult(bool isSuccessed, List<string> errorMessages,
        string token = "")
    {
        IsSuccessed = isSuccessed;
        ErrorMessages = errorMessages;
        Token = token;
    }
}