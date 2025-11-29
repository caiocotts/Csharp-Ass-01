using Assignment01.Models;

namespace Assignment01.ViewModels;

public class TicketPurchasingViewModel
{
    public IReadOnlyList<Event> Events { get; init; } = Array.Empty<Event>();
    public string? AlertMessage { get; init; }
    public bool IsError { get; init; }
}