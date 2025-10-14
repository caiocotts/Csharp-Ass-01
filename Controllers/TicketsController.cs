using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    

    [HttpPost]
    public IActionResult AddPurchase(int eventId, int quantity, double price) {
        
        string id = Request.Cookies["id"];
        
        if (string.IsNullOrEmpty(id)) { //if theres no cookie send to login page
            return RedirectToAction("Login", "Login");
        }
        
        
        var order = new Purchase() {
            //PK-ID is automatically increased
            Cost = quantity*price,
            Quantity = quantity,
            Date = DateTime.UtcNow,
            UserId = int.Parse(id), //cookies are stored as strings
            EventId = eventId    
        };
        
        var ev = context.Events.Find(eventId);  // Find event by ID
        if (ev != null) ev.AvailableTickets -= quantity;  // remove the tickets that were purchased
        
        context.Purchases.Add(order);
        context.SaveChanges();
        return RedirectToAction("TicketPurchasing", "Tickets");
    }

    
   

}