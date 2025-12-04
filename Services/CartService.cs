using System.Text.Json;
using Assignment01.Models;
using Assignment01.ViewModels;

namespace Assignment01.Services;

public interface ICartService
{
    List<CartItem> GetCart();
    void AddToCart(CartItem item);
    void UpdateQuantity(int eventId, int quantity);
    void RemoveFromCart(int eventId);
    void ClearCart();
    int GetCartItemCount();
    double GetCartTotal();
}

public class SessionCartService(IHttpContextAccessor httpContextAccessor) : ICartService
{
    private const string CartSessionKey = "ShoppingCart";

    private ISession Session => httpContextAccessor.HttpContext!.Session;

    public List<CartItem> GetCart()
    {
        var cartJson = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
            return [];

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? [];
    }

    private void SaveCart(List<CartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        Session.SetString(CartSessionKey, cartJson);
    }

    public void AddToCart(CartItem item)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(c => c.EventId == item.EventId);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Add(item);
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int eventId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.EventId == eventId);

        if (item == null) return;
        if (quantity <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        SaveCart(cart);
    }

    public void RemoveFromCart(int eventId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.EventId == eventId);

        if (item == null) return;
        cart.Remove(item);
        SaveCart(cart);
    }

    public void ClearCart()
    {
        Session.Remove(CartSessionKey);
    }

    public int GetCartItemCount()
    {
        return GetCart().Sum(c => c.Quantity);
    }

    public double GetCartTotal()
    {
        return Math.Round(GetCart().Sum(c => c.Subtotal), 2);
    }
}