using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class Purchase
{
    [Key] public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Cost { get; set; }
    public int Quantity { get; set; }

    // Foreign Keys
    public int UserId { get; set; }
    public int EventId { get; set; }
}