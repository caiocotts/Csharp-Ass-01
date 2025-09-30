using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class EventsController : Controller
{
    public IActionResult ManageEvents() => View();
    
    public IActionResult PurchaseTicket() => View();
}