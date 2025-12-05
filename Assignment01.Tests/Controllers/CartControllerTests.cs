using Assignment01.Controllers;
using Assignment01.Models;
using Assignment01.Tests.Helpers;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;

namespace Assignment01.Tests.Controllers;

public class CartControllerTests
{
    private readonly FakeCartService _cartService;
    private readonly CartController _controller;
    private readonly Event _testEvent;

    public CartControllerTests()
    {
        var context = TestDbContextFactory.Create();
        _cartService = new FakeCartService();

        // seed test data
        _testEvent = new Event
        {
            Title = "Test Concert",
            Category = "Music",
            EventDate = DateTime.UtcNow.AddDays(30),
            PricePerTicket = 50.00,
            AvailableTickets = 100
        };
        var soldOutEvent = new Event
        {
            Title = "Sold Out Show",
            Category = "Theater",
            EventDate = DateTime.UtcNow.AddDays(15),
            PricePerTicket = 75.00,
            AvailableTickets = 0
        };
        context.Events.Add(_testEvent);
        context.Events.Add(soldOutEvent);
        context.SaveChanges();

        _controller = new CartController(context, null!, _cartService);

        // setup TempData
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            new SessionStateTempDataProvider());
    }

    [Fact]
    public void AddToCart_ValidQuantity_ReturnsRedirectToTicketPurchasing()
    {
        var result = _controller.AddToCart(eventId: _testEvent.Id, quantity: 2);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("TicketPurchasing", redirectResult.ActionName);
    }

    [Fact]
    public void AddToCart_ValidQuantity_AddsItemToCart()
    {
        _controller.AddToCart(eventId: _testEvent.Id, quantity: 2);

        Assert.Equal(2, _cartService.GetCart()[0].Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void AddToCart_InvalidQuantity_SetsErrorMessage(int quantity)
    {
        _controller.AddToCart(eventId: _testEvent.Id, quantity: quantity);

        Assert.Equal("Quantity must be at least 1.", _controller.TempData["CartError"]);
    }

    [Fact]
    public void AddToCart_EventNotFound_ReturnsNotFound()
    {
        var result = _controller.AddToCart(eventId: 99999, quantity: 1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void AddToCart_ExceedsAvailableTickets_SetsErrorMessage()
    {
        _controller.AddToCart(eventId: _testEvent.Id, quantity: 101);

        Assert.Contains("Not enough tickets", _controller.TempData["CartError"]?.ToString());
    }

    [Fact]
    public void AddToCart_SameEventTwice_IncrementsQuantity()
    {
        _controller.AddToCart(eventId: _testEvent.Id, quantity: 2);
        _controller.AddToCart(eventId: _testEvent.Id, quantity: 3);

        Assert.Equal(5, _cartService.GetCart()[0].Quantity);
    }

    [Fact]
    public void Checkout_ReturnsCheckoutViewModel()
    {
        var result = _controller.Checkout();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<CheckoutViewModel>(viewResult.Model);
    }

    [Theory]
    [InlineData(0, 0, 0)] // Empty cart
    [InlineData(50, 2, 100)] // 50 * 2 = 100
    [InlineData(30, 3, 90)] // 30 * 3 = 90
    public void Checkout_ReturnsCorrectTotal(double price, int quantity, double expectedTotal)
    {
        if (quantity > 0)
        {
            _cartService.AddToCart(new CartItem
            {
                EventId = _testEvent.Id,
                EventTitle = "Concert",
                PricePerTicket = price,
                Quantity = quantity
            });
        }

        var result = _controller.Checkout();

        var model = Assert.IsType<CheckoutViewModel>(((ViewResult)result).Model);
        Assert.Equal(expectedTotal, model.Total);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(1, 1)]
    public void UpdateQuantity_UpdatesCartQuantity(int newQuantity, int expected)
    {
        _cartService.AddToCart(new CartItem
            { EventId = _testEvent.Id, EventTitle = "Concert", PricePerTicket = 50, Quantity = 2 });

        _controller.UpdateQuantity(eventId: _testEvent.Id, quantity: newQuantity);

        Assert.Equal(expected, _cartService.GetCart()[0].Quantity);
    }

    [Fact]
    public void UpdateQuantity_ZeroQuantity_RemovesItemFromCart()
    {
        _cartService.AddToCart(new CartItem
            { EventId = _testEvent.Id, EventTitle = "Concert", PricePerTicket = 50, Quantity = 2 });

        _controller.UpdateQuantity(eventId: _testEvent.Id, quantity: 0);

        Assert.Empty(_cartService.GetCart());
    }

    [Fact]
    public void RemoveFromCart_ReturnsRedirectToCheckout()
    {
        _cartService.AddToCart(new CartItem
            { EventId = _testEvent.Id, EventTitle = "Concert", PricePerTicket = 50, Quantity = 2 });

        var result = _controller.RemoveFromCart(eventId: _testEvent.Id);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Checkout", redirectResult.ActionName);
    }

    [Fact]
    public void RemoveFromCart_RemovesItemFromCart()
    {
        _cartService.AddToCart(new CartItem
            { EventId = _testEvent.Id, EventTitle = "Concert", PricePerTicket = 50, Quantity = 2 });

        _controller.RemoveFromCart(eventId: _testEvent.Id);

        Assert.Empty(_cartService.GetCart());
    }
}

// TempData provider for testing
public class SessionStateTempDataProvider : ITempDataProvider
{
    private Dictionary<string, object?> _data = new();

    public IDictionary<string, object?> LoadTempData(HttpContext context) => _data;

    public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
    {
        _data = new Dictionary<string, object?>(values);
    }
}