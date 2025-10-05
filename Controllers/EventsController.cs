using Assignment01.Data;
using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Assignment01.Controllers;

public class EventsController : Controller {
    private readonly AppDbContext _context;
    public IActionResult ManageEvents() => View();

    [HttpPost]
    public IActionResult Events(Event events) {
        if (!ModelState.IsValid) return View(events);
    }



}