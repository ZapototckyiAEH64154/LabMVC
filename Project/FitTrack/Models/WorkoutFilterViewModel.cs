using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitTrack.Models;

// ViewModel dla listy treningów — łączy dane do wyszukiwania, filtrowania i wyniki.
public class WorkoutFilterViewModel
{
    public IList<Workout> Workouts { get; set; } = new List<Workout>();

    // Lista kategorii do filtrowania (rozwijana lista).
    public SelectList? Categories { get; set; }

    // Aktualnie wybrane / wpisane wartości filtrów.
    public string? SearchString { get; set; }
    public int? CategoryId { get; set; }
    public Intensity? Intensity { get; set; }
    public string? Sort { get; set; }
}
