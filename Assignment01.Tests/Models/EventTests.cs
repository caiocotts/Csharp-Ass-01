using Assignment01.Models;
using Assignment01.Tests.Helpers;
using Xunit;

namespace Assignment01.Tests.Models;

public class EventTests
{
    [Fact]
    public void ValidEvent_PassesValidation()
    {
        var anEvent = new Event
        {
            Title = "Concert",
            Category = "Music",
            EventDate = DateTime.UtcNow.AddDays(30),
            PricePerTicket = 50.00,
            AvailableTickets = 100
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("", true)]           // Empty - should fail
    [InlineData("A", false)]         // 1 char - should pass
    [InlineData(null, true)]         // Null - should fail
    public void Title_Validation(string? title, bool expected)
    {
        var anEvent = new Event
        {
            Title = title!,
            Category = "Music",
            PricePerTicket = 50.00,
            AvailableTickets = 100
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Equal(expected, ValidationHelper.HasValidationError(results, "Title"));
    }

    [Theory]
    [InlineData(100, false)]  // Exactly 100 - should pass
    [InlineData(101, true)]   // Over 100 - should fail
    public void Title_LengthValidation(int length, bool expected)
    {
        var anEvent = new Event
        {
            Title = new string('A', length),
            Category = "Music",
            PricePerTicket = 50.00,
            AvailableTickets = 100
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Equal(expected, ValidationHelper.HasValidationError(results, "Title"));
    }

    [Theory]
    [InlineData("", true)]           // Empty - should fail
    [InlineData("Music", false)]     // Valid - should pass
    [InlineData(null, true)]         // Null - should fail
    public void Category_Validation(string? category, bool expected)
    {
        var anEvent = new Event
        {
            Title = "Concert",
            Category = category!,
            PricePerTicket = 50.00,
            AvailableTickets = 100
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Equal(expected, ValidationHelper.HasValidationError(results, "Category"));
    }

    [Theory]
    [InlineData(50, false)]   // Exactly 50 - should pass
    [InlineData(51, true)]    // Over 50 - should fail
    public void Category_LengthValidation(int length, bool expected)
    {
        var anEvent = new Event
        {
            Title = "Concert",
            Category = new string('A', length),
            PricePerTicket = 50.00,
            AvailableTickets = 100
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Equal(expected, ValidationHelper.HasValidationError(results, "Category"));
    }

    [Theory]
    [InlineData(-10.00, true)]   // Negative - should fail
    [InlineData(0, false)]       // Zero (free event) - should pass
    [InlineData(50.00, false)]   // Positive - should pass
    public void PricePerTicket_Validation(double price, bool expected)
    {
        var anEvent = new Event
        {
            Title = "Concert",
            Category = "Music",
            PricePerTicket = price,
            AvailableTickets = 100
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Equal(expected, ValidationHelper.HasValidationError(results, "PricePerTicket"));
    }

    [Theory]
    [InlineData(-5, true)]    // Negative - should fail
    [InlineData(0, false)]    // Zero (sold out) - should pass
    [InlineData(100, false)]  // Positive - should pass
    public void AvailableTickets_Validation(int tickets, bool expected)
    {
        var anEvent = new Event
        {
            Title = "Concert",
            Category = "Music",
            PricePerTicket = 50.00,
            AvailableTickets = tickets
        };

        var results = ValidationHelper.ValidateModel(anEvent);

        Assert.Equal(expected, ValidationHelper.HasValidationError(results, "AvailableTickets"));
    }
}
