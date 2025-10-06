using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

public class EventManagerController(AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageEvents(string sortOrder, string searchString, string category)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CurrentCategory"] = category;
        ViewData["IdSortParm"] = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
        ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
        ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
        ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
        ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
        ViewData["TicketsSortParm"] = sortOrder == "Tickets" ? "tickets_desc" : "Tickets";

        var eventsQuery = context.Events.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
            eventsQuery = eventsQuery.Where(e => e.Title.ToUpper().Contains(searchString.ToUpper()));

        if (!string.IsNullOrWhiteSpace(category))
            eventsQuery = eventsQuery.Where(e => e.Category.ToUpper() == category.ToUpper());

        eventsQuery = sortOrder switch
        {
            "id_desc" => eventsQuery.OrderByDescending(e => e.Id),
            "Title" => eventsQuery.OrderBy(e => e.Title),
            "title_desc" => eventsQuery.OrderByDescending(e => e.Title),
            "Category" => eventsQuery.OrderBy(e => e.Category),
            "category_desc" => eventsQuery.OrderByDescending(e => e.Category),
            "Date" => eventsQuery.OrderBy(e => e.EventDate),
            "date_desc" => eventsQuery.OrderByDescending(e => e.EventDate),
            "Price" => eventsQuery.OrderBy(e => e.PricePerTicket),
            "price_desc" => eventsQuery.OrderByDescending(e => e.PricePerTicket),
            "Tickets" => eventsQuery.OrderBy(e => e.AvailableTickets),
            "tickets_desc" => eventsQuery.OrderByDescending(e => e.AvailableTickets),
            _ => eventsQuery.OrderBy(e => e.Id)
        };

        var list = await eventsQuery.ToListAsync();
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