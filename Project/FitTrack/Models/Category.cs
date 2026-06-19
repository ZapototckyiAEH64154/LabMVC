using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models;

// MODEL DODATKOWY — rodzaj/kategoria treningu (np. Cardio, Siłowy).
public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Nazwa kategorii")]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Opis")]
    public string? Description { get; set; }

    // Relacja jeden-do-wielu: jedna kategoria ma wiele treningów.
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
