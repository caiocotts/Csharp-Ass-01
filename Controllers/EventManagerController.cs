using Assignment01.Data;
using Assignment01.Models;
using Assignment01.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

public class EventManagerController(AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageEvents(string sortOrder, string searchString, string categoryFilter,
        string availabilityFilter, DateTime? startDate, DateTime? endDate)
    {
        var normalizedStart = startDate?.Date;
        var normalizedEnd = endDate?.Date;
        if (normalizedEnd < normalizedStart) // swap if needed
        {
            (normalizedStart, normalizedEnd) = (normalizedEnd, normalizedStart);
        }

        ViewData["SearchString"] = searchString;
        ViewData["SortOrder"] = sortOrder;
        ViewData["CategoryFilter"] = categoryFilter;
        ViewData["AvailabilityFilter"] = availabilityFilter;
        ViewData["StartDate"] = normalizedStart?.ToString("yyyy-MM-dd") ?? string.Empty;
        ViewData["EndDate"] = normalizedEnd?.ToString("yyyy-MM-dd") ?? string.Empty;

        var events = context.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
            events = events.Where(e => e.Title.ToUpper().Contains(searchString.ToUpper()));

        if (!string.IsNullOrWhiteSpace(categoryFilter))
            events = events.Where(e => e.Category.ToUpper() == categoryFilter.ToUpper());
        
        
        if (normalizedStart.HasValue && normalizedEnd.HasValue) // filter with both start and end date
        {
            var startUtc = DateFormat.ToUtc(normalizedStart.Value);
            var endExclusiveUtc = DateFormat.ToUtc(normalizedEnd.Value.AddDays(1));
            events = events.Where(e => e.EventDate >= startUtc && e.EventDate < endExclusiveUtc);
        }
        else if (normalizedStart.HasValue) // no end date
        {
            var startUtc = DateFormat.ToUtc(normalizedStart.Value);
            events = events.Where(e => e.EventDate >= startUtc);
        }
        else if (normalizedEnd.HasValue) // no start date
        {
            var endExclusiveUtc = DateFormat.ToUtc(normalizedEnd.Value.AddDays(1));
            events = events.Where(e => e.EventDate < endExclusiveUtc);
        }

        if (!string.IsNullOrWhiteSpace(availabilityFilter))
        {
            events = availabilityFilter.ToUpper() switch
            {
                "ALL" => events.OrderBy(e => e.Title),
                "AVAILABLE" => events.Where(e => e.AvailableTickets > 0).OrderBy(e => e.Title),
                "SOLD OUT" => events.Where(e => e.AvailableTickets == 0).OrderBy(e => e.Title),
                _ => events
            };
        }

        events = sortOrder switch
        {
            "id_desc" => events.OrderByDescending(e => e.Id),
            "title" => events.OrderBy(e => e.Title),
            "title_desc" => events.OrderByDescending(e => e.Title),
            "category" => events.OrderBy(e => e.Category),
            "category_desc" => events.OrderByDescending(e => e.Category),
            "date" => events.OrderBy(e => e.EventDate),
            "date_desc" => events.OrderByDescending(e => e.EventDate),
            "price" => events.OrderBy(e => e.PricePerTicket),
            "price_desc" => events.OrderByDescending(e => e.PricePerTicket),
            "tickets" => events.OrderBy(e => e.AvailableTickets),
            "tickets_desc" => events.OrderByDescending(e => e.AvailableTickets),
            _ => events.OrderBy(e => e.Id)
        };

        var list = await events.ToListAsync();
        return View(list);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Event anEvent)
    {
        if (!ModelState.IsValid) return View(anEvent);

        anEvent.EventDate = DateFormat.ToUtc(anEvent.EventDate);
        anEvent.PricePerTicket = Math.Round(anEvent.PricePerTicket, 2);
        context.Events.Add(anEvent);
        context.SaveChanges();
        return RedirectToAction("ManageEvents");
    }

    [HttpGet]
    public IActionResult DeleteConfirmation(int id)
    {
        var anEvent = context.Events.FirstOrDefault(e => e.Id == id);
        return anEvent == null ? NotFound() : View(anEvent);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var anEvent = context.Events.Find(id);
        if (anEvent == null) return NotFound();
        context.Events.Remove(anEvent);
        context.SaveChanges();
        return RedirectToAction("ManageEvents");
    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        var anEvent = context.Events.Find(id);
        if (anEvent == null)
        {
            return NotFound();
        }

        return View(anEvent);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Title,Category,EventDate,PricePerTicket,AvailableTickets")] Event anEvent)
    {
        if (id != anEvent.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                anEvent.EventDate = DateTime.SpecifyKind(anEvent.EventDate, DateTimeKind.Utc);
                context.Update(anEvent);
                context.SaveChanges();
                return RedirectToAction("ManageEvents");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Events.Any(e => e.Id == anEvent.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        return View(anEvent);
    }
}