using Microsoft.EntityFrameworkCore;
using FitTrack.Models;

namespace FitTrack.Data;

// Kontekst bazy danych EF Core — pośredniczy między modelami a bazą.
public class FitTrackContext : DbContext
{
    public FitTrackContext(DbContextOptions<FitTrackContext> options)
        : base(options)
    {
    }

    public DbSet<Workout> Workout { get; set; } = default!;
    public DbSet<Category> Category { get; set; } = default!;
    public DbSet<Exercise> Exercise { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Nie pozwól usunąć kategorii, jeśli ma przypisane treningi.
        modelBuilder.Entity<Workout>()
            .HasOne(w => w.Category)
            .WithMany(c => c.Workouts)
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usunięcie treningu usuwa też jego ćwiczenia.
        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.Workout)
            .WithMany(w => w.Exercises)
            .HasForeignKey(e => e.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
