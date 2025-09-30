using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class User
{
    [Key] public int Id { get; set; }
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";

    // Navigation Props
    public ICollection<Purchase>? Purchases { get; set; }
}