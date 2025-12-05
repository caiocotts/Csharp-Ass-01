namespace Assignment01.ViewModels;

/// <summary>
/// ViewModel for displaying ticket purchase data in the dashboard.
/// Combines information from Purchase, Event, and User tables.
/// </summary>
public class TicketDataViewModel
{
    public int PurchaseId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public double TotalCost { get; set; }
    public int Quantity { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string PurchaserFullName { get; set; } = string.Empty;
}
