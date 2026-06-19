using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FitTrack.Models;

// MODEL GŁÓWNY — pojedynczy trening fitness.
public class Workout
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa treningu jest wymagana.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 120 znaków.")]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;

    // Relacja do kategorii (rodzaj treningu) — klucz obcy.
    [Required(ErrorMessage = "Wybierz kategorię.")]
    [Display(Name = "Rodzaj (kategoria)")]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [ValidateNever]
    public Category? Category { get; set; }

    [Display(Name = "Intensywność")]
    public Intensity Intensity { get; set; } = Intensity.Medium;

    [Range(1, 600, ErrorMessage = "Czas trwania musi być w zakresie 1–600 minut.")]
    [Display(Name = "Czas trwania (min)")]
    public int DurationMinutes { get; set; } = 30;

    [Required(ErrorMessage = "Data treningu jest wymagana.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data treningu")]
    public DateTime ScheduledDate { get; set; } = DateTime.Today;

    [StringLength(1000)]
    [Display(Name = "Notatki")]
    public string? Notes { get; set; }

    // Relacja jeden-do-wielu: jeden trening ma wiele ćwiczeń.
    [ValidateNever]
    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
