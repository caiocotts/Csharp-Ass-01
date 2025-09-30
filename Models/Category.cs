using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class Category
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    // Navigation Props
    public ICollection<Event>? Events { get; set; }
}