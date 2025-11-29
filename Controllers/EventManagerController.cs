using Assignment01.Data;
using Assignment01.Extensions;
using Assignment01.Models;
using Assignment01.Util;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;


[Authorize(Roles = "Admin, Organizer")]
public class EventManagerController(AppDbContext context, UserManager<User> userManager) : Controller
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
    
    [HttpGet]
    public IActionResult Analytics() => View();

    [HttpGet]
    public async Task<IActionResult> GetTicketData() {
        var user = await userManager.GetUserAsync(User);

        var categorySales = User.IsInRole("Admin")
            ? context.Events
                .Select(e => new {
                    category = e.Category,
                    sold = e.Purchases.Sum(p => (int?)p.Quantity) ?? 0

                })
                .GroupBy(e => e.category)
                .Select(g => new {
                    name = g.Key,
                    sold = g.Sum(s => s.sold)
                })
                .OrderByDescending(s => s.sold)
                .ToList()

            : context.Events
                .Where(e => e.OrganizerId == user.Id)
                .Select(e => new {
                    category = e.Category,
                    sold = e.Purchases.Sum(p => (int?)p.Quantity) ?? 0

                })
                .GroupBy(e => e.category)
                .Select(g => new {
                    name = g.Key,
                    sold = g.Sum(s => s.sold)
                })
                .OrderByDescending(s => s.sold)
                .ToList();
        
        return Json(categorySales); 
    }

    /*[HttpGet]
    public IActionResult GetMonthlyRevenueData() {
        var monthlyRevenue =  context.Events
            .Select
    }*/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event anEvent)
    {
        if (!ModelState.IsValid) return View(anEvent);
        
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized(); // or redirect to login

        anEvent.OrganizerId = user.Id;
        
        anEvent.EventDate = DateFormat.ToUtc(anEvent.EventDate);
        anEvent.PricePerTicket = Math.Round(anEvent.PricePerTicket, 2);
        context.Events.Add(anEvent);
        await context.SaveChangesAsync();
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