using Assignment01.Data;
using Assignment01.Models;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

public class TicketsController(AppDbContext context, UserManager<User> userManager) : Controller
{
    public IActionResult TicketPurchasing()
    {
        var events = context.Events.AsNoTracking().OrderBy(e => e.EventDate).ToList();
        var viewModel = new TicketPurchasingViewModel
        {
            Events = events,
            AlertMessage = TempData["TicketAlert"] as string,
            IsError = string.Equals(TempData["TicketAlertType"] as string, "error", StringComparison.OrdinalIgnoreCase)
        };

        return View(viewModel);
    }

    [Authorize]
    [HttpGet]
    public IActionResult PurchaseConfirm(int id)
    {
        var anEvent = context.Events.AsNoTracking().FirstOrDefault(e => e.Id == id);
        if (anEvent == null) return NotFound();

        var viewModel = new PurchaseConfirmationViewModel
        {
            EventId = anEvent.Id,
            EventTitle = anEvent.Title,
            Category = anEvent.Category,
            EventDate = anEvent.EventDate,
            PricePerTicket = anEvent.PricePerTicket,
            AvailableTickets = anEvent.AvailableTickets,
            UserDisplayName = ResolveUserDisplayName()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddPurchase(int eventId, int quantity) {
        
        var userId = userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        if (quantity <= 0)
        {
            SetTicketAlert("Quantity must be at least 1 ticket.", true);
            return RedirectToAction(nameof(PurchaseConfirm), new { id = eventId });
        }

        var anEvent = context.Events.FirstOrDefault(e => e.Id == eventId);
        if (anEvent == null) return NotFound();

        if (quantity > anEvent.AvailableTickets)
        {
            SetTicketAlert("Not enough tickets remain for this event.", true);
            return RedirectToAction(nameof(PurchaseConfirm), new { id = eventId });
        }

        anEvent.AvailableTickets -= quantity;
        var order = new Purchase
        {
            Cost = Math.Round(quantity * anEvent.PricePerTicket, 2),
            Quantity = quantity,
            Date = DateTime.UtcNow,
            UserId = userId,
            EventId = eventId
        };

        context.Purchases.Add(order);
        context.SaveChanges();

        SetTicketAlert("Purchase completed successfully.", false);
        return RedirectToAction(nameof(TicketPurchasing));
    }

    private string ResolveUserDisplayName()
    {
        var user = userManager.GetUserAsync(User).Result;
        return user?.FullName ?? user?.UserName ?? "Guest";
    }

    private void SetTicketAlert(string message, bool isError)
    {
        TempData["TicketAlert"] = message;
        TempData["TicketAlertType"] = isError ? "error" : "success";
    }
}