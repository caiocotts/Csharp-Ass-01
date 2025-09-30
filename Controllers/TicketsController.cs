using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class TicketsController : Controller
{
    public IActionResult TicketPurchasing() => View();
    
    public IActionResult PurchaseConfirm() => View();

}