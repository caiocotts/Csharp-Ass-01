namespace Assignment01.ViewModels;

public class CartItem
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public double PricePerTicket { get; set; }
    public int Quantity { get; set; }

    public double Subtotal => Math.Round(PricePerTicket * Quantity, 2);
}