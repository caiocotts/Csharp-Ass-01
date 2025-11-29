namespace Assignment01.Util;

public static class TicketInventoryFormatter
{
    public static string FormatAvailability(int availableTickets)
    {
        if (availableTickets <= 0)
        {
            return "SOLD OUT";
        }

        if (availableTickets < 200)
        {
            return $"{availableTickets} | ALMOST GONE!";
        }

        return availableTickets.ToString();
    }

    public static bool IsSoldOut(int availableTickets) => availableTickets <= 0;
}
