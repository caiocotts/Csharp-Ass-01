using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

public class TicketsController (AppDbContext context) : Controller
{
    public async Task<IActionResult> TicketPurchasing(string sortOrder) {
        ViewData["SortOrder"] = sortOrder;
        
        var events = context.Events.AsQueryable();
        
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