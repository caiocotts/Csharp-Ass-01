using Assignment01.Data;
using Assignment01.Models;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Controllers;

[Authorize]
public class DashboardController(AppDbContext context, UserManager<User> userManager) : Controller
{
    public class TicketDataObject
    {
        public DateTime PurchaseDate { get; set; }
        public double TotalCost { get; set; }
        public int Quantity { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }

        public string PurchaseFullName { get; set; }

        public int PurchaseID { get; set; }
    }

    //gets all the tickets for this user
    public TicketDataObject GetTicketDataFromPurchase(int purchaseID)
    {
        //the data models
        var purchase = context.Purchases.FirstOrDefault(e => e.Id == purchaseID);
        var thisEvent = context.Events.FirstOrDefault(e => e.Id == purchase.EventId);
        var user = context.Users.FirstOrDefault(e => e.Id == purchase.UserId);

        return new TicketDataObject
        {
            //the actual data from each table
            PurchaseDate = purchase.Date,
            TotalCost = purchase.Cost,
            Quantity = purchase.Quantity,
            EventTitle = thisEvent.Title,
            EventDate = thisEvent.EventDate,
            PurchaseFullName = user.FullName,
            PurchaseID = purchaseID
        };
    }

    public class eRev : Event
    {
        public double revenue { get; set; }
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MyEvents()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user.Id;

        // 1, get every id of an event that a user purchased
        var eventIds = context.Purchases
            .Where(p => p.UserId == userId)
            .Select(p => p.EventId)
            .ToList();
        
            
        var eventsWithRevenue = context.Events
            .Where(e => eventIds.Contains(e.Id))
            .Select(e => new eRev
            {
              // add the old values
                Id = e.Id,
                Title = e.Title,
                Category = e.Category,
                EventDate = e.EventDate,
                PricePerTicket = e.PricePerTicket,
                AvailableTickets = e.AvailableTickets,
                OrganizerId = e.OrganizerId,

                //add our value
                revenue = context.Purchases
                    .Where(p => p.EventId == e.Id)
                    .Sum(p => p.Cost)
            })
            .ToList();

        return View(eventsWithRevenue);
    }

    public async Task<IActionResult> PurchaseHistory()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user.Id;

        // Get all purchases for the current user
        var purchases = context.Purchases.Where(p => p.UserId == userId).ToList();

        // Create a list of TicketDataObject to store the results
        var ticketDataList = purchases.Select(purchase => GetTicketDataFromPurchase(purchase.Id)).ToList();

        // Pass the populated list to the View
        return View(ticketDataList);
    }

    public async Task<IActionResult> MyTickets()
    {
        var user = await userManager.GetUserAsync(User);
        var userId = user.Id;

        // Get all purchases for the current user
        var purchases = context.Purchases.Where(p => p.UserId == userId).ToList();

        // Create a list of TicketDataObject to store the results
        var ticketDataList = purchases.Select(purchase => GetTicketDataFromPurchase(purchase.Id)).ToList();

        return View(ticketDataList);
    }

    /*public async IActionResult downloadPDF() {

    }*/

    public IActionResult viewQR()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SubmitRating(int purchaseId, int rating)
    {
        // Validate the input again on the server side
        if (rating < 1 || rating > 5)
        {
            return BadRequest("Rating must be between 1 and 5");
        }

        var purchase = context.Purchases.FirstOrDefault(e => e.Id == purchaseId);
        purchase.PurchaseRating = rating;
        context.Update(purchase);
        context.SaveChanges();

        return RedirectToAction("PurchaseHistory");
    }
    
}