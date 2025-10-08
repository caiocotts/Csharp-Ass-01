using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class LoginController(AppDbContext context) : Controller {
    // GET
    [HttpGet]
    public IActionResult Login() { 
        return View();
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LoginToSession(string emailAddress) {
        
        var user = context.Users.FirstOrDefault(u => u.Email == emailAddress);
        if (user == null) {
            TempData["ErrorMessage"] = "User not found. Please check your email.";
            return RedirectToAction("Login", "Login");
        }
        
        SetCookie(user.Id.ToString());//because cookies need to be string
        return RedirectToAction("TicketPurchasing", "Tickets");
    }
    public IActionResult SetCookie(string id) {
        
        Response.Cookies.Append("id", id, new CookieOptions {
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        
        return RedirectToAction("Index", "Home"); // Or wherever you want
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateUser(string username, string emailAddress) {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(emailAddress)) {
            TempData["EmailExistsMessage"] = "Username and Email are required.";
            return RedirectToAction("Login");
        }

        // Case-insensitive email check
        var emailAlreadyExists = context.Users.Any(user => user.Email.ToLower() == emailAddress.ToLower());

        // Show alert and do NOT create a new user if email exists
        if (emailAlreadyExists) {
            TempData["EmailExistsMessage"] = "An account with that email already exists. Please log in or use a different email.";
            return RedirectToAction("Login");
        }
        
        // Create and persist the new user
        var tempUser = new User {
            Email = emailAddress,
            Name = username
        };
        
        context.Users.Add(tempUser); // adds the user to the users table
        context.SaveChanges(); //saves the DB
        SetCookie("" + tempUser.Id); // creates cookie  (must be after you save to DB)
        return RedirectToAction("TicketPurchasing", "Tickets"); 
    }
}