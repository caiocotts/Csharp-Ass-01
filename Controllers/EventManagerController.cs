using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

public class EventManagerController(AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageEvents(string sortOrder, string searchString, string categoryFilter)
    {
        ViewData["SearchString"] = searchString;
        ViewData["SortOrder"] = sortOrder;
        ViewData["CategoryFilter"] = categoryFilter;

        var events = context.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
            events = events.Where(e => e.Title.ToUpper().Contains(searchString.ToUpper()));

        if (!string.IsNullOrWhiteSpace(categoryFilter))
            events = events.Where(e => e.Category.ToUpper() == categoryFilter.ToUpper());

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

        anEvent.EventDate = ToUtc(anEvent.EventDate);
        anEvent.PricePerTicket = Math.Round(anEvent.PricePerTicket, 2);
        context.Events.Add(anEvent);
        context.SaveChanges();
        return RedirectToAction("ManageEvents");

        DateTime ToUtc(DateTime dt) =>
            dt.Kind switch
            {
                DateTimeKind.Utc => dt,
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
                _ => dt.ToUniversalTime()
            };
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
}