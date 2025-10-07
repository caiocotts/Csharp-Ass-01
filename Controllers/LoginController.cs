using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class LoginController(AppDbContext context) : Controller {
    // GET
    public IActionResult Login() {
        return View();
    }
    public IActionResult SetCookie(string id) {
        
        Response.Cookies.Append("id", id, new CookieOptions {
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        
        return RedirectToAction("Index", "Home"); // Or wherever you want
    }

    public IActionResult CreateUser(string username, string email) {
        //TODO add check too see if the email is already in the DB and handle it if it is
        User tempUser = new User(); //intialize
        tempUser.Email = email; // adds email
        tempUser.Name = username; // adds name
        context.Users.Add(tempUser); // adds the user to the users table
        context.SaveChanges(); //saves the DB
        SetCookie(""+ tempUser.Id); // creates cookie  (must be after you save to DB)
        return RedirectToAction("TicketPurchasing", "Tickets"); 
    }

    
    
}