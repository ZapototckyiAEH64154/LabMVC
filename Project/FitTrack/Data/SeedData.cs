using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FitTrack.Models;

namespace FitTrack.Data;

// Tworzy bazę danych i wczytuje przykładowe dane przy pierwszym uruchomieniu.
public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new FitTrackContext(
            serviceProvider.GetRequiredService<DbContextOptions<FitTrackContext>>());

        // Utwórz bazę danych, jeśli jeszcze nie istnieje.
        context.Database.EnsureCreated();

        if (context.Workout.Any())
        {
            return; // dane już istnieją
        }

        var cardio = new Category { Name = "Cardio", Description = "Trening wytrzymałościowy podnoszący tętno" };
        var strength = new Category { Name = "Siłowy", Description = "Trening z obciążeniem budujący siłę i masę" };
        var stretch = new Category { Name = "Rozciąganie", Description = "Ćwiczenia poprawiające elastyczność" };

        context.Category.AddRange(cardio, strength, stretch);
        context.SaveChanges();

        context.Workout.AddRange(
            new Workout
            {
                Name = "Poranny bieg",
                Category = cardio,
                Intensity = Intensity.High,
                DurationMinutes = 40,
                ScheduledDate = DateTime.Today,
                Notes = "Trasa wokół parku, tempo 5:30/km"
            },
            new Workout
            {
                Name = "Trening klatki piersiowej",
                Category = strength,
                Intensity = Intensity.Medium,
                DurationMinutes = 60,
                ScheduledDate = DateTime.Today.AddDays(1),
                Notes = "Skupienie na wyciskaniu sztangi",
                Exercises = new[]
                {
                    new Exercise { Name = "Wyciskanie sztangi", Sets = 4, Reps = 8 },
                    new Exercise { Name = "Rozpiętki z hantlami", Sets = 3, Reps = 12 }
                }
            },
            new Workout
            {
                Name = "Joga wieczorna",
                Category = stretch,
                Intensity = Intensity.Low,
                DurationMinutes = 30,
                ScheduledDate = DateTime.Today.AddDays(2),
                Notes = "Relaks i rozciąganie po pracy",
                Exercises = new[]
                {
                    new Exercise { Name = "Pozycja psa z głową w dół", Sets = 3, Reps = 5 }
                }
            }
        );

        context.SaveChanges();
    }
}
