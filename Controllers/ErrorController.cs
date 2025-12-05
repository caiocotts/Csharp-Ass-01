using Microsoft.AspNetCore.Mvc;

namespace Assignment01.Controllers;

public class ErrorController : Controller {
    
    // handles 404-page stuff
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        if (statusCode == 404)
        {
            ViewBag.ErrorMessage = "Sorry, due to technical difficulties we cannot load this page right now.";
                
            return View("NotFound"); 
        }
        
        return View("Error");
    }

    // handles all the 500-page stuff
    [Route("Error/500")]
    public IActionResult ServerError()
    {
        return View("ServerError");
    }
}