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

    

}