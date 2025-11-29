using System;

namespace Assignment01.ViewModels;

public class PurchaseConfirmationViewModel
{
    public int EventId { get; init; }
    public string EventTitle { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public DateTime EventDate { get; init; }
    public double PricePerTicket { get; init; }
    public int AvailableTickets { get; init; }
    public string UserDisplayName { get; init; } = "Guest";

    public bool CanPurchase => AvailableTickets > 0;
    public int DefaultQuantity => CanPurchase ? 1 : 0;
}