using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class LoginController : Controller {
    // GET
    public IActionResult Login() {
        return View();
    }
    
}