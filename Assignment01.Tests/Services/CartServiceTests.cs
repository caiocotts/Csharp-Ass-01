using Assignment01.Tests.Helpers;
using Assignment01.ViewModels;
using Xunit;

namespace Assignment01.Tests.Services;

public class CartServiceTests
{
    private readonly FakeCartService _cartService = new();

    [Fact]
    public void GetCart_WhenEmpty_ReturnsEmptyList()
    {
        var cart = _cartService.GetCart();

        Assert.Empty(cart);
    }

    [Fact]
    public void AddToCart_NewItem_StoresCorrectQuantity()
    {
        var item = new CartItem
        {
            EventId = 1,
            EventTitle = "Concert",
            PricePerTicket = 50.00,
            Quantity = 2
        };

        _cartService.AddToCart(item);

        Assert.Equal(2, _cartService.GetCart()[0].Quantity);
    }

    [Fact]
    public void AddToCart_ExistingItem_IncrementsQuantity()
    {
        var item1 = new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 };
        var item2 = new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 3, PricePerTicket = 50 };

        _cartService.AddToCart(item1);
        _cartService.AddToCart(item2);

        Assert.Equal(5, _cartService.GetCart()[0].Quantity);
    }

    [Fact]
    public void AddToCart_DifferentItems_AddsBoth()
    {
        var item1 = new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 };
        var item2 = new CartItem { EventId = 2, EventTitle = "Play", Quantity = 1, PricePerTicket = 30 };

        _cartService.AddToCart(item1);
        _cartService.AddToCart(item2);

        Assert.Equal(2, _cartService.GetCart().Count);
    }

    [Theory]
    [InlineData(1, 5, 5)]   // Valid item - updates to 5
    [InlineData(1, 10, 10)] // Valid item - updates to 10
    [InlineData(999, 5, 2)] // Non-existent item - stays at 2
    public void UpdateQuantity_UpdatesCorrectly(int eventId, int newQuantity, int expected)
    {
        var item = new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 };
        _cartService.AddToCart(item);

        _cartService.UpdateQuantity(eventId, newQuantity);

        Assert.Equal(expected, _cartService.GetCart()[0].Quantity);
    }

    [Theory]
    [InlineData(1, 0)]    // Existing item - cart becomes empty
    [InlineData(999, 1)]  // Non-existent item - cart still has 1 item
    public void RemoveFromCart_AffectsCartCorrectly(int eventIdToRemove, int expectedCount)
    {
        var item = new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 };
        _cartService.AddToCart(item);

        _cartService.RemoveFromCart(eventIdToRemove);

        Assert.Equal(expectedCount, _cartService.GetCart().Count);
    }

    [Fact]
    public void ClearCart_RemovesAllItems()
    {
        _cartService.AddToCart(new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 });
        _cartService.AddToCart(new CartItem { EventId = 2, EventTitle = "Play", Quantity = 1, PricePerTicket = 30 });

        _cartService.ClearCart();

        Assert.Empty(_cartService.GetCart());
    }

    [Fact]
    public void GetCartItemCount_ReturnsCorrectCount()
    {
        _cartService.AddToCart(new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 });
        _cartService.AddToCart(new CartItem { EventId = 2, EventTitle = "Play", Quantity = 3, PricePerTicket = 30 });

        var count = _cartService.GetCartItemCount();

        Assert.Equal(5, count);
    }

    [Fact]
    public void GetCartTotal_CalculatesCorrectly()
    {
        _cartService.AddToCart(new CartItem { EventId = 1, EventTitle = "Concert", Quantity = 2, PricePerTicket = 50 }); // 100
        _cartService.AddToCart(new CartItem { EventId = 2, EventTitle = "Play", Quantity = 3, PricePerTicket = 30 });    // 90

        var total = _cartService.GetCartTotal();

        Assert.Equal(190, total);
    }

    [Fact]
    public void GetCartTotal_WhenEmpty_ReturnsZero()
    {
        var total = _cartService.GetCartTotal();

        Assert.Equal(0, total);
    }
}
