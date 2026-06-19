using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FitTrack.Models;

// MODEL DODATKOWY — pojedyncze ćwiczenie należące do treningu.
public class Exercise
{
    public int Id { get; set; }

    // Relacja do treningu — klucz obcy.
    [Display(Name = "Trening")]
    public int WorkoutId { get; set; }

    [ForeignKey(nameof(WorkoutId))]
    [ValidateNever]
    public Workout? Workout { get; set; }

    [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana.")]
    [StringLength(120, MinimumLength = 2)]
    [Display(Name = "Nazwa ćwiczenia")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 50)]
    [Display(Name = "Serie")]
    public int Sets { get; set; } = 3;

    [Range(1, 500)]
    [Display(Name = "Powtórzenia")]
    public int Reps { get; set; } = 10;
}
