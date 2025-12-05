using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public double Cost { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public int EventId { get; set; }

    public Event Event { get; set; } = null!;

    [Range(0, 5)]
    public int PurchaseRating { get; set; }
}