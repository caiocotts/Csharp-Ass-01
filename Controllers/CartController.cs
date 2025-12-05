using Assignment01.Areas.Identity.Pages.Account;
using Assignment01.Data;
using Assignment01.Models;
using Assignment01.Services;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

namespace Assignment01.Controllers;

public class CartController(
    AppDbContext context,
    UserManager<User> userManager,
    ILogger<LoginModel> logger,
    ICartService cartService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddToCart(int eventId, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["CartError"] = "Quantity must be at least 1.";
            return RedirectToAction("TicketPurchasing", "Tickets");
        }

        var anEvent = context.Events.Find(eventId);
        if (anEvent == null) return NotFound();

        var cart = cartService.GetCart();
        var existingItem = cart.FirstOrDefault(c => c.EventId == eventId);
        var currentInCart = existingItem?.Quantity ?? 0;

        if (currentInCart + quantity > anEvent.AvailableTickets)
        {
            TempData["CartError"] =
                $"Not enough tickets available. Only {anEvent.AvailableTickets - currentInCart} more can be added.";
            return RedirectToAction("TicketPurchasing", "Tickets");
        }

        var cartItem = new CartItem
        {
            EventId = anEvent.Id,
            EventTitle = anEvent.Title,
            Category = anEvent.Category,
            EventDate = anEvent.EventDate,
            PricePerTicket = anEvent.PricePerTicket,
            Quantity = quantity
        };

        cartService.AddToCart(cartItem);
        TempData["CartSuccess"] = $"Added {quantity} ticket(s) to cart.";

        return RedirectToAction("TicketPurchasing", "Tickets");
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = cartService.GetCart();
        var viewModel = new CheckoutViewModel
        {
            CartItems = cart,
            Total = cartService.GetCartTotal()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int eventId, int quantity)
    {
        if (quantity <= 0)
        {
            cartService.RemoveFromCart(eventId);
        }
        else
        {
            var anEvent = context.Events.Find(eventId);
            if (anEvent != null && quantity <= anEvent.AvailableTickets)
            {
                cartService.UpdateQuantity(eventId, quantity);
            }
            else
            {
                TempData["CartError"] = "Not enough tickets available.";
            }
        }

        return RedirectToAction(nameof(Checkout));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFromCart(int eventId)
    {
        cartService.RemoveFromCart(eventId);
        return RedirectToAction(nameof(Checkout));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CompletePurchase()
    {
        var userId = userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var cart = cartService.GetCart();
        if (cart.Count == 0)
        {
            TempData["CartError"] = "Your cart is empty.";
            return RedirectToAction(nameof(Checkout));
        }

        foreach (var item in cart)
        {
            var anEvent = context.Events.Find(item.EventId);
            if (anEvent == null)
            {
                TempData["CartError"] = $"Event '{item.EventTitle}' no longer exists.";
                cartService.RemoveFromCart(item.EventId);
                return RedirectToAction(nameof(Checkout));
            }

            if (item.Quantity <= anEvent.AvailableTickets) continue;
            TempData["CartError"] =
                $"Not enough tickets for '{item.EventTitle}'. Only {anEvent.AvailableTickets} available.";
            return RedirectToAction(nameof(Checkout));
        }

        int uID = -1;
        string title = "defualt";
        double price = 1;
        foreach (var item in cart)
        {
            var anEvent = context.Events.Find(item.EventId)!;
            anEvent.AvailableTickets -= item.Quantity;
            
            uID = anEvent.Id;
            title = anEvent.Title;
            price = anEvent.PricePerTicket;
            var purchase = new Purchase
            {
                Cost = item.Subtotal,
                Quantity = item.Quantity,
                Date = DateTime.UtcNow,
                UserId = userId,
                EventId = item.EventId
            };

            context.Purchases.Add(purchase);
        }

        context.SaveChanges();
        cartService.ClearCart();

        logger.LogInformation("UserID: {userID}, purchased from '{eventTitle}', cost: ${price} ", uID, title, price );
        TempData["PurchaseSuccess"] = "true"; 
        return RedirectToAction(nameof(Checkout));
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        return Json(new { count = cartService.GetCartItemCount() });
    }
}