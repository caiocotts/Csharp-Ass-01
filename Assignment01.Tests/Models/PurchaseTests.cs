using Assignment01.Models;
using Assignment01.Tests.Helpers;
using Xunit;

namespace Assignment01.Tests.Models;

public class PurchaseTests
{
    [Fact]
    public void ValidPurchase_PassesValidation()
    {
        var purchase = new Purchase
        {
            Date = DateTime.UtcNow,
            Cost = 100.00,
            Quantity = 2,
            UserId = "user-123",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.Empty(results);
    }

    [Fact]
    public void Quantity_WhenZero_FailsValidation()
    {
        var purchase = new Purchase
        {
            Cost = 100.00,
            Quantity = 0,
            UserId = "user-123",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.True(ValidationHelper.HasValidationError(results, "Quantity"));
    }

    [Fact]
    public void Quantity_WhenNegative_FailsValidation()
    {
        var purchase = new Purchase
        {
            Cost = 100.00,
            Quantity = -1,
            UserId = "user-123",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.True(ValidationHelper.HasValidationError(results, "Quantity"));
    }

    [Fact]
    public void Quantity_WhenPositive_PassesValidation()
    {
        var purchase = new Purchase
        {
            Cost = 100.00,
            Quantity = 5,
            UserId = "user-123",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.False(ValidationHelper.HasValidationError(results, "Quantity"));
    }

    [Fact]
    public void Cost_WhenNegative_FailsValidation()
    {
        var purchase = new Purchase
        {
            Cost = -50.00,
            Quantity = 1,
            UserId = "user-123",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.True(ValidationHelper.HasValidationError(results, "Cost"));
    }

    [Fact]
    public void Cost_WhenZero_PassesValidation()
    {
        var purchase = new Purchase
        {
            Cost = 0,
            Quantity = 1,
            UserId = "user-123",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.False(ValidationHelper.HasValidationError(results, "Cost"));
    }

    [Fact]
    public void UserId_WhenEmpty_FailsValidation()
    {
        var purchase = new Purchase
        {
            Cost = 100.00,
            Quantity = 1,
            UserId = "",
            EventId = 1
        };

        var results = ValidationHelper.ValidateModel(purchase);

        Assert.True(ValidationHelper.HasValidationError(results, "UserId"));
    }
}