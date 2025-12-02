namespace Assignment01.Util;

public static class TicketInventoryFormatter
{
    public static string FormatAvailability(int availableTickets)
    {
        return availableTickets switch
        {
            <= 0 => "SOLD OUT",
            < 200 => $"{availableTickets} | ALMOST GONE!",
            _ => availableTickets.ToString()
        };
    }

    public static bool IsSoldOut(int availableTickets) => availableTickets <= 0;
}