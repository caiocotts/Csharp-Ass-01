using Microsoft.AspNetCore.Identity;

namespace Assignment01.Models;

public class User : IdentityUser {
    public String? FullName  { get; set; }
    public String? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public String? ProfilePictureUrl { get; set; }
}