using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitTrack.Controllers;
using FitTrack.Data;
using FitTrack.Models;
using Xunit;

namespace FitTrack.Tests;

public class WorkoutsControllerTests
{
    // Tworzy kontekst EF Core w pamięci (InMemory) z przykładowymi danymi.
    private static FitTrackContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<FitTrackContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new FitTrackContext(options);

        var cardio = new Category { Id = 1, Name = "Cardio" };
        var strength = new Category { Id = 2, Name = "Siłowy" };
        context.Category.AddRange(cardio, strength);

        context.Workout.AddRange(
            new Workout { Id = 1, Name = "Poranny bieg", CategoryId = 1, Intensity = Intensity.High, ScheduledDate = DateTime.Today },
            new Workout { Id = 2, Name = "Trening nóg", CategoryId = 2, Intensity = Intensity.Medium, ScheduledDate = DateTime.Today },
            new Workout { Id = 3, Name = "Joga", CategoryId = 2, Intensity = Intensity.Low, ScheduledDate = DateTime.Today }
        );
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task Index_ReturnsAllWorkouts_WhenNoFilter()
    {
        using var context = CreateContext(nameof(Index_ReturnsAllWorkouts_WhenNoFilter));
        var controller = new WorkoutsController(context);

        var result = await controller.Index(null, null, null, null) as ViewResult;
        var model = Assert.IsType<WorkoutFilterViewModel>(result!.Model);

        Assert.Equal(3, model.Workouts.Count);
    }

    [Fact]
    public async Task Index_FiltersBySearchString()
    {
        using var context = CreateContext(nameof(Index_FiltersBySearchString));
        var controller = new WorkoutsController(context);

        var result = await controller.Index("Joga", null, null, null) as ViewResult;
        var model = Assert.IsType<WorkoutFilterViewModel>(result!.Model);

        Assert.Single(model.Workouts);
        Assert.Equal("Joga", model.Workouts[0].Name);
    }

    [Fact]
    public async Task Index_FiltersByCategory()
    {
        using var context = CreateContext(nameof(Index_FiltersByCategory));
        var controller = new WorkoutsController(context);

        var result = await controller.Index(null, 2, null, null) as ViewResult;
        var model = Assert.IsType<WorkoutFilterViewModel>(result!.Model);

        Assert.Equal(2, model.Workouts.Count);
        Assert.All(model.Workouts, w => Assert.Equal(2, w.CategoryId));
    }

    [Fact]
    public async Task Details_ReturnsNotFound_ForMissingId()
    {
        using var context = CreateContext(nameof(Details_ReturnsNotFound_ForMissingId));
        var controller = new WorkoutsController(context);

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_AddsWorkout_AndRedirects()
    {
        using var context = CreateContext(nameof(Create_AddsWorkout_AndRedirects));
        var controller = new WorkoutsController(context);

        var newWorkout = new Workout
        {
            Name = "Nowy trening",
            CategoryId = 1,
            Intensity = Intensity.Medium,
            DurationMinutes = 45,
            ScheduledDate = DateTime.Today
        };

        var result = await controller.Create(newWorkout);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(4, context.Workout.Count());
    }
}
