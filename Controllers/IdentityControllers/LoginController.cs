using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Assignment01.Controllers.IdentityControllers;

public class LoginController : Controller{
    public IActionResult Login()
    {
        return View();
    }
}