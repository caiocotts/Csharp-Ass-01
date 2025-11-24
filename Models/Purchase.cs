using System;
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
    public int UserId { get; set; }

    [Required]
    public int EventId { get; set; }

    public Event Event { get; set; } = null!;
}