using Assignment01.Models;

namespace Assignment01.ViewModels;

public class ManageEventsViewModel
{
    private static readonly string[] DefaultCategoryOptions = ["Webinar", "Concert", "Workshop", "Conference"];
    private static readonly string[] DefaultAvailabilityOptions = ["All", "Available", "Sold Out"];

    public IReadOnlyList<Event> Events { get; init; } = Array.Empty<Event>();
    public EventFilterOptions Filters { get; init; } = new();
    public IReadOnlyList<string> CategoryOptions { get; init; } = DefaultCategoryOptions;
    public IReadOnlyList<string> AvailabilityOptions { get; init; } = DefaultAvailabilityOptions;
}