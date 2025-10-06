using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

public class EventManagerController(AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageEvents(string searchString)
    {
        var events = from e in context.Events select e;
        if (!string.IsNullOrEmpty(searchString))
        {
            events = events.Where(e => e.Title.ToUpper().Contains(searchString.ToUpper()));
        }

        return View(await events.ToListAsync());
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