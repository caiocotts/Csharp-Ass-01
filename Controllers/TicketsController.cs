using System.Security.Claims;
using Assignment01.Data;
using Assignment01.Models;

namespace Assignment01.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text; 
using QRCoder; 
using System.IO;
using Microsoft.EntityFrameworkCore;

[Authorize] //Ensures only logged-in user can access the controller. 
public class TicketsController(AppDbContext _context): Controller
{



    public IActionResult MyTickets()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }
        
        var userPurchase = _context.Purchases
            .Where(p => p.UserId == userId)
            .Include(p => p.Event)
            .ToList();
        
        return View(userPurchase);
    }
    
    
    public class TicketDataObject
    {
        public DateTime PurchaseDate { get; set; }
        public double TotalCost { get; set; }
        public int Quantity { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string PurchaseFullName { get; set; }
        
    }

    public TicketDataObject getTicketDataFromPurchase(int purchaseID)
    {
        //the data models
        var purchase = _context.Purchases.FirstOrDefault(e => e.Id == purchaseID);
        var thisEvent = _context.Events.FirstOrDefault(e => e.Id == purchase.EventId);
        var user = _context.Users.FirstOrDefault(e => e.Id == purchase.UserId);
        
        return new TicketDataObject {
            
            //the actual data from each table
            PurchaseDate = purchase.Date,
            TotalCost = purchase.Cost,
            Quantity = purchase.Quantity,
            EventTitle = thisEvent.Title,
            EventDate = thisEvent.EventDate,
            PurchaseFullName = user.FullName
        };
    }
    
    //Creating QR Code Generation Method 
    private string GenerateQrCodeBase64(string data)
    {
        using (var generator = new QRCodeGenerator())
        {
            using (var qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
            {
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {  
                    byte[] qrCodeBytes = qrCode.GetGraphic(10);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        }
    }


    public IActionResult DownloadPdf(int purchaseID)
    {
        //make the object
        TicketDataObject ticketInfo = getTicketDataFromPurchase(purchaseID);
        
        //refrence the object and get the data u want.
        DateTime pDate = ticketInfo.PurchaseDate;
        

        if (ticketInfo == null)
        {
            return NotFound();
        }
        
        
        //Get Base64 QR Code Stirng 
        string validationString = $"Ticket_Validate:{purchaseID}";
        string qrCodeBase64 = GenerateQrCodeBase64(validationString);
        
    // 2. Generate the PDF using QuestPDF's fluent API
    var pdfBytes = Document.Create(container =>
    {
        container.Page(page =>
        {
            // Set Page Properties
            page.Size(PageSizes.A5);
            page.Margin(20);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

            page.Content()
                .PaddingVertical(10)
                .Column(column =>
                {
                    // Ticket Header
                    column.Item().Text(ticketInfo.EventTitle)
                        .SemiBold().FontSize(24).FontColor(Colors.Red.Darken2);
                    
                    column.Item().PaddingTop(5).Text("Event Ticket").FontSize(14);
                    
                    column.Item().PaddingTop(20).BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                        .Column(details =>
                        {
                            details.Item().Text($"Ticket Holder: {ticketInfo.PurchaseFullName}");
                            details.Item().PaddingTop(5).Text($"Date: {ticketInfo.EventDate:MM/dd/yyyy}");
                            details.Item().PaddingTop(5).Text($"Tickets: {ticketInfo.Quantity}");
                            details.Item().PaddingTop(5).Text($"Total Cost: {ticketInfo.TotalCost:C}");
                        });

                    // QR Code Section
                    column.Item().PaddingTop(30).AlignCenter()
                        .Column(qrSection =>
                        {
                            // QuestPDF can directly use the Base64 string for images
                            qrSection.Item()
                                .Width(150).Height(150)
                                .AlignCenter()
                                .Image(qrCodeBase64)
                                .FitArea();

                            qrSection.Item().PaddingTop(5).Text("Scan for Entry Validation").Italic();
                        });
                });
        });
    }).GeneratePdf(); // Generate the PDF byte array
        
  
        
        //Return the File for Downloading (it will download the content of the file as a pdf)
        // 3. Return the File for Downloading
        return File(
            pdfBytes,
            "application/pdf",
            $"Ticket-{ticketInfo.EventTitle.Replace(" ", "_")}.pdf"
        );


    }
    
    
}