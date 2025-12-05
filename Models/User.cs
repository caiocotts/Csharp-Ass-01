using Microsoft.AspNetCore.Identity;

namespace Assignment01.Models;

public class User : IdentityUser
{
    public string? FullName { get; set; }
    public new string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
}