using Assignment01.Data;
using Assignment01.Models;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

[Authorize]
public class DashboardController(AppDbContext context, UserManager<User> userManager) : Controller
{
    /// <summary>
    /// Converts a purchase record into a display-friendly TicketDataViewModel.
    /// Joins data from Purchases, Events, and Users tables.
    /// </summary>
    private TicketDataViewModel GetTicketDataFromPurchase(int purchaseId)
    {
        var purchase = context.Purchases.First(p => p.Id == purchaseId);
        var eventEntity = context.Events.First(e => e.Id == purchase.EventId);
        var user = context.Users.First(u => u.Id == purchase.UserId);

        return new TicketDataViewModel
        {
            PurchaseId = purchaseId,
            PurchaseDate = purchase.Date,
            TotalCost = purchase.Cost,
            Quantity = purchase.Quantity,
            EventTitle = eventEntity.Title,
            EventDate = eventEntity.EventDate,
            PurchaserFullName = user.FullName ?? ""
        };
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MyEvents()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user!.Id;

        // Get all event IDs that this user has purchased tickets for
        var purchasedEventIds = context.Purchases
            .Where(p => p.UserId == userId)
            .Select(p => p.EventId)
            .ToList();

        // Build view models with calculated revenue for each event
        var eventsWithRevenue = context.Events
            .Where(e => purchasedEventIds.Contains(e.Id))
            .Select(e => new EventWithRevenueViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.Category,
                EventDate = e.EventDate,
                PricePerTicket = e.PricePerTicket,
                AvailableTickets = e.AvailableTickets,
                OrganizerId = e.OrganizerId,
                TotalRevenue = context.Purchases
                    .Where(p => p.EventId == e.Id)
                    .Sum(p => p.Cost)
            })
            .ToList();

        return View(eventsWithRevenue);
    }

    public async Task<IActionResult> PurchaseHistory()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user!.Id;

        var purchases = context.Purchases
            .Where(p => p.UserId == userId)
            .ToList();

        var ticketDataList = purchases
            .Select(p => GetTicketDataFromPurchase(p.Id))
            .ToList();

        return View(ticketDataList);
    }

    public async Task<IActionResult> MyTickets()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user!.Id;

        var purchases = context.Purchases
            .Where(p => p.UserId == userId)
            .ToList();

        var ticketDataList = purchases
            .Select(p => GetTicketDataFromPurchase(p.Id))
            .ToList();

        return View(ticketDataList);
    }
    
    
    // Partial View Endpoints for AJAX loading
    [HttpGet]
    public async Task<IActionResult> GetPurchaseHistoryPartial()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user!.Id;

        var purchases = context.Purchases
            .Where(p => p.UserId == userId)
            .ToList();

        var ticketDataList = purchases
            .Select(p => GetTicketDataFromPurchase(p.Id))
            .ToList();

        return PartialView("_PurchaseHistoryPartial", ticketDataList);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTicketsPartial()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user!.Id;

        var purchases = context.Purchases
            .Where(p => p.UserId == userId)
            .ToList();

        var ticketDataList = purchases
            .Select(p => GetTicketDataFromPurchase(p.Id))
            .ToList();

        return PartialView("_MyTicketsPartial", ticketDataList);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyEventsPartial()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user!.Id;

        var purchasedEventIds = context.Purchases
            .Where(p => p.UserId == userId)
            .Select(p => p.EventId)
            .ToList();

        var eventsWithRevenue = context.Events
            .Where(e => purchasedEventIds.Contains(e.Id))
            .Select(e => new EventWithRevenueViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.Category,
                EventDate = e.EventDate,
                PricePerTicket = e.PricePerTicket,
                AvailableTickets = e.AvailableTickets,
                OrganizerId = e.OrganizerId,
                TotalRevenue = context.Purchases
                    .Where(p => p.EventId == e.Id)
                    .Sum(p => p.Cost)
            })
            .ToList();

        return PartialView("_MyEventsPartial", eventsWithRevenue);
    }
// ---------------------------------------------

    public IActionResult viewQR()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SubmitRating(int purchaseId, int rating)
    {
        if (rating is < 1 or > 5)
        {
            return BadRequest("Rating must be between 1 and 5");
        }

        var purchase = context.Purchases.First(p => p.Id == purchaseId);
        purchase.PurchaseRating = rating;
        context.Update(purchase);
        context.SaveChanges();

        return RedirectToAction("Dashboard");
    }
}