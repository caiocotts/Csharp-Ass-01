using System.Linq;
using Assignment01.Models;
using Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Extensions;

public static class EventQueryExtensions
{
    public static IQueryable<Event> ApplyFilters(this IQueryable<Event> source, EventFilterOptions filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.SearchString))
        {
            var search = $"%{filters.SearchString.Trim()}%";
            source = source.Where(e => e.Title != null && EF.Functions.ILike(e.Title, search));
        }

        if (!string.IsNullOrWhiteSpace(filters.CategoryFilter))
        {
            var category = filters.CategoryFilter.Trim();
            source = source.Where(e => e.Category != null && EF.Functions.ILike(e.Category, category));
        }

        var (startUtc, endUtcExclusive) = filters.ToUtcRange();
        if (startUtc.HasValue)
        {
            source = source.Where(e => e.EventDate >= startUtc.Value);
        }

        if (endUtcExclusive.HasValue)
        {
            source = source.Where(e => e.EventDate < endUtcExclusive.Value);
        }

        source = filters.AvailabilityKey switch
        {
            "AVAILABLE" => source.Where(e => e.AvailableTickets > 0),
            "SOLD OUT" => source.Where(e => e.AvailableTickets <= 0),
            _ => source
        };

        return source;
    }

    public static IQueryable<Event> ApplySorting(this IQueryable<Event> source, string? sortOrder)
    {
        return sortOrder switch
        {
            "id_desc" => source.OrderByDescending(e => e.Id),
            "title" => source.OrderBy(e => e.Title),
            "title_desc" => source.OrderByDescending(e => e.Title),
            "category" => source.OrderBy(e => e.Category),
            "category_desc" => source.OrderByDescending(e => e.Category),
            "date" => source.OrderBy(e => e.EventDate),
            "date_desc" => source.OrderByDescending(e => e.EventDate),
            "price" => source.OrderBy(e => e.PricePerTicket),
            "price_desc" => source.OrderByDescending(e => e.PricePerTicket),
            "tickets" => source.OrderBy(e => e.AvailableTickets),
            "tickets_desc" => source.OrderByDescending(e => e.AvailableTickets),
            _ => source.OrderBy(e => e.Id)
        };
    }
}
