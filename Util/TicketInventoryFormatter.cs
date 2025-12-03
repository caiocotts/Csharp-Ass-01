using Microsoft.AspNetCore.Html;

namespace Assignment01.Util;

public static class TicketInventoryFormatter
{
    public static HtmlString FormatAvailability(int availableTickets)
    {
        return availableTickets switch
        {
            <= 0 => new HtmlString("<span class=\"text-danger\">SOLD OUT</span>"),
            < 200 => new HtmlString($"<span class=\"text-warning\">${availableTickets} | ALMOST GONE!</span>"),
            _ => new HtmlString(availableTickets.ToString())
        };
    }

    public static bool IsSoldOut(int availableTickets) => availableTickets <= 0;
}