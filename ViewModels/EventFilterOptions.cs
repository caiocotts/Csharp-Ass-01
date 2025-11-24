using System;
using Assignment01.Util;

namespace Assignment01.ViewModels;

public class EventFilterOptions
{
    public string? SortOrder { get; set; }
    public string? SearchString { get; set; }
    public string? CategoryFilter { get; set; }
    public string? AvailabilityFilter { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public EventFilterOptions Normalize()
    {
        var normalized = new EventFilterOptions
        {
            SortOrder = NormalizeString(SortOrder)?.ToLowerInvariant(),
            SearchString = NormalizeString(SearchString),
            CategoryFilter = NormalizeString(CategoryFilter),
            AvailabilityFilter = NormalizeString(AvailabilityFilter),
            StartDate = StartDate?.Date,
            EndDate = EndDate?.Date
        };

        if (normalized.StartDate.HasValue && normalized.EndDate.HasValue &&
            normalized.EndDate < normalized.StartDate)
        {
            (normalized.StartDate, normalized.EndDate) = (normalized.EndDate, normalized.StartDate);
        }

        return normalized;
    }

    public string StartDateInput => StartDate?.ToString("yyyy-MM-dd") ?? string.Empty;
    public string EndDateInput => EndDate?.ToString("yyyy-MM-dd") ?? string.Empty;

    public (DateTime? StartUtc, DateTime? EndUtcExclusive) ToUtcRange()
    {
        var startUtc = StartDate.HasValue ? DateFormat.ToUtc(StartDate.Value) : (DateTime?)null;
        var endUtcExclusive = EndDate.HasValue ? DateFormat.ToUtc(EndDate.Value.AddDays(1)) : (DateTime?)null;
        return (startUtc, endUtcExclusive);
    }

    public string AvailabilityKey => (AvailabilityFilter ?? string.Empty).Trim().ToUpperInvariant();

    private static string? NormalizeString(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}