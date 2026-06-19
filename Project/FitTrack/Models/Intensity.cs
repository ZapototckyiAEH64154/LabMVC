using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models;

// Poziom intensywności treningu — używany jako rozwijana lista (enum).
public enum Intensity
{
    [Display(Name = "Niska")]
    Low,

    [Display(Name = "Średnia")]
    Medium,

    [Display(Name = "Wysoka")]
    High
}
