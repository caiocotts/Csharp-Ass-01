using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime EventDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public double PricePerTicket { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableTickets { get; set; }

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}