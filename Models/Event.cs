using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class Event
{
    [Key] public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    [DataType(DataType.Date)] public DateTime EventDate { get; set; }
    public double PricePerTicket { get; set; }

    // Navigation Props
    public ICollection<Purchase>? Purchases { get; set; }
}