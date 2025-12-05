namespace Assignment01.ViewModels;

/// <summary>
/// Extends Event model with calculated revenue for dashboard display.
/// Used in MyEvents view to show organizer their event earnings.
/// </summary>
public class EventWithRevenueViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public double PricePerTicket { get; set; }
    public int AvailableTickets { get; set; }
    public string? OrganizerId { get; set; }
    public double TotalRevenue { get; set; }
}
