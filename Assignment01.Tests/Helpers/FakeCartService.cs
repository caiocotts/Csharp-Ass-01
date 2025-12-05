using Assignment01.Services;
using Assignment01.ViewModels;

namespace Assignment01.Tests.Helpers;

public class FakeCartService : ICartService
{
    private readonly List<CartItem> _cart = [];

    public List<CartItem> GetCart() => _cart;

    public void AddToCart(CartItem item)
    {
        var existingItem = _cart.FirstOrDefault(c => c.EventId == item.EventId);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            _cart.Add(item);
        }
    }

    public void UpdateQuantity(int eventId, int quantity)
    {
        var item = _cart.FirstOrDefault(c => c.EventId == eventId);
        if (item != null)
        {
            item.Quantity = quantity;
        }
    }

    public void RemoveFromCart(int eventId)
    {
        var item = _cart.FirstOrDefault(c => c.EventId == eventId);
        if (item != null)
        {
            _cart.Remove(item);
        }
    }

    public void ClearCart() => _cart.Clear();

    public int GetCartItemCount() => _cart.Sum(c => c.Quantity);

    public double GetCartTotal() => _cart.Sum(c => c.PricePerTicket * c.Quantity);
}