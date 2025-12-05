using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class DashboardController(AppDbContext context, UserManager<User> userManager) : Controller{
    
    public class TicketDataObject {
        public DateTime PurchaseDate { get; set; }
        public double TotalCost { get; set; }
        public int Quantity { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string PurchaseFullName { get; set; }
        public int PurchaseID { get; set; }
    }

    //gets all the tickets for this user
    public TicketDataObject getTicketDataFromPurchase(int purchaseID)
    {
        //the data models
        var purchase = context.Purchases.FirstOrDefault(e => e.Id == purchaseID);
        var thisEvent = context.Events.FirstOrDefault(e => e.Id == purchase.EventId);
        var user = context.Users.FirstOrDefault(e => e.Id == purchase.UserId);
        
        return new TicketDataObject {
            
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
    
    public IActionResult dashboard(){
        
        return View();  
    }
    
    public async Task<IActionResult> myTickets() {
        
        var user = await userManager.GetUserAsync(User);
        var userId = user.Id;

        // Get all purchases for the current user
        var purchases = context.Purchases.Where(p => p.UserId == userId).ToList();

        // Create a list of TicketDataObject to store the results
        var ticketDataList = new List<TicketDataObject>();

        // Iterate through each purchase and create a TicketDataObject
        foreach (var purchase in purchases)
        {
            var ticketData = getTicketDataFromPurchase(purchase.Id);
            ticketDataList.Add(ticketData);
        }

        return View(ticketDataList);
    }

    /*public async IActionResult downloadPDF() {
        
    }*/

    public IActionResult viewQR() {
        return View();
    }
    
}