using Assignment01.Data;
using Assignment01.Extensions;
using Assignment01.Models;
using Assignment01.Util;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

[Authorize(Roles = "Admin, Organizer")]
public class EventManagerController(AppDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageEvents([FromQuery] EventFilterOptions? filterOptions)
    {
        var filters = (filterOptions ?? new EventFilterOptions()).Normalize();

        var eventsQuery = context.Events.AsNoTracking().AsQueryable();
        eventsQuery = eventsQuery.ApplyFilters(filters).ApplySorting(filters.SortOrder);

        var events = await eventsQuery.ToListAsync();
        var viewModel = new ManageEventsViewModel
        {
            Events = events,
            Filters = filters
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
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
        var anEvent = context.Events.AsNoTracking().FirstOrDefault(e => e.Id == id);
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
        return anEvent == null ? NotFound() : View(anEvent);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id,
        [Bind("Id,Title,Category,EventDate,PricePerTicket,AvailableTickets")]
        Event anEvent)
    {
        if (id != anEvent.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(anEvent);
        try
        {
            anEvent.EventDate = DateFormat.ToUtc(anEvent.EventDate);
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

            throw;
        }
    }
}