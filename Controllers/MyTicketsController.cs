/*using Assignment01.Data;
using Assignment01.Models;

namespace Assignment01.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text; 
using QRCoder; 
using System.IO;
using Microsoft.EntityFrameworkCore;

[Authorize] //Ensures only logged-in user can access the controller. 
public class MyTicketsController(AppDbContext _context): Controller
{



    public IActionResult MyTickets()
    {
        return View();
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
        
        
        
        string validationString = $"Ticket_Validate:{purchaseID}";
        string qrCodeBase64 = GenerateQrCodeBase64(validationString);
        
        var htmlBuilder = new StringBuilder();
        //The HTML Builder for the PDF when you scan the QR code.
        htmlBuilder.Append($@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial; padding: 20px; }}
                    .ticket-container {{ border: 2px solid #000; padding: 30px; width: 600px; margin: 0 auto; }}
                    h1 {{ color: #CC0000; }}
                    .qr-code {{ text-align: center; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='ticket-container'>
                    <h1>{ticketInfo.EventTitle}</h1>
                    <p><strong>Ticket Holder:</strong> {ticketInfo.PurchaseFullName}</p>
                    <p><strong>Date:</strong>{ticketInfo.EventDate:MM/dd/yyyy}</p>
                    <p><strong>Total Cost of a Ticket:</strong>{ticketInfo.TotalCost}</p>
                    
                    <div class='qr-code'>
                        <img src='data:image/png;base64,{qrCodeBase64}' alt='Ticket QR Code' style='width: 200px; height: 200px;'/>
                        <p>Scan for Entry Validation</p>
                    </div>
                </div>
            </body>
            </html>
        ");
        
        
        /#1#/IRON PDF RENDERIING 
        var renderer = new ChromePdfRenderer();
        PdfDocument pdf = renderer.RenderHtmlAsPdf(htmlBuilder.ToString());#1#
        
        //Return the File for Downloading (it will download the content of the file as a pdf)
        /*return File(
            pdf.BinaryData,
            "application/pdf",
            $"Ticket-{ticketInfo.EventTitle.Replace(" ", "_")}.pdf"
        );#1#

    
    }
    
    
}*/