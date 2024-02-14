using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}