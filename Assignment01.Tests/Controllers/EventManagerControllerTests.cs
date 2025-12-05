using Assignment01.Controllers;
using Assignment01.Models;
using Assignment01.Tests.Helpers;
using Assignment01.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Assignment01.Tests.Controllers;

public class EventManagerControllerTests
{
    [Fact]
    public async Task ManageEvents_ReturnsViewModelWithFilters()
    {
        var context = TestDbContextFactory.Create();
        var controller = new EventManagerController(context, null!);

        var result = await controller.ManageEvents(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ManageEventsViewModel>(viewResult.Model);
        Assert.NotNull(model.Filters);
    }

    [Fact]
    public async Task GetFilteredEvents_ReturnsJsonResult()
    {
        var context = TestDbContextFactory.Create();
        var controller = new EventManagerController(context, null!);

        var result = await controller.GetFilteredEvents(null);

        Assert.IsType<JsonResult>(result);
    }

    [Fact]
    public void Create_ReturnsViewResult()
    {
        var context = TestDbContextFactory.Create();
        var controller = new EventManagerController(context, null!);

        var result = controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Edit_InvalidId_ReturnsNotFound()
    {
        var context = TestDbContextFactory.Create();
        var controller = new EventManagerController(context, null!);

        var result = controller.Edit(99999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Edit_ValidId_ReturnsCorrectEvent()
    {
        var context = TestDbContextFactory.Create();
        var testEvent = new Event 
        { 
            Title = "Test Event", 
            Category = "Test", 
            PricePerTicket = 25, 
            AvailableTickets = 50 
        };
        context.Events.Add(testEvent);
        context.SaveChanges();
        var controller = new EventManagerController(context, null!);

        var result = controller.Edit(testEvent.Id);

        var model = (Event)((ViewResult)result).Model!;
        Assert.Equal("Test Event", model.Title);
    }
}
