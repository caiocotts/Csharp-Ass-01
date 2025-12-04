using Assignment01.Models;

namespace Assignment01.ViewModels;

public class CheckoutViewModel
{
    public List<CartItem> CartItems { get; set; } = [];
    public double Total { get; set; }
    public bool IsEmpty => CartItems.Count == 0;
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}