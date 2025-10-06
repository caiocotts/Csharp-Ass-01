using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class TicketsController (AppDbContext context) : Controller
{
    public IActionResult TicketPurchasing() {
        var events = context.Events.ToList();
        return View(events);
    }

    public IActionResult PurchaseConfirm(int id) {
        var anEvent = context.Events.Find(id);
        if (anEvent == null) return NotFound();
        return View(anEvent);
    }
    
/*
    [HttpPost]
    public IActionResult confirmPurchase(int eventId, int quantity, double price)
    {
        var order = new Purchase() {
            //PK-ID is automatically increased
            Cost = quantity*price,
            Date = DateTime.Now,
            // FK for UserID
            EventId = eventId     //FK
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        return RedirectToAction("Confirmation", new { id = order.Id });
    }
*/
    

}