using Microsoft.AspNetCore.Identity;

namespace BusinessLogicLayer.Helpers;

public record AuthResult (bool isSuccess, 
                          List<string> errorMessages)
{
    public bool IsSuccessed = isSuccess;
    public List<string> ErrorMessages = errorMessages;

    public static implicit operator AuthResult(IdentityResult result)
        => new(isSuccess: result.Succeeded, 
           errorMessages: result.Errors
                                .Select(error => error.Description)
                                .ToList());
}