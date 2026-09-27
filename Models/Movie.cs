using System.ComponentModel.DataAnnotations;

namespace MovieWatchlist.Models;

public enum WatchStatus
{
    [Display(Name = "Plan to Watch")]
    PlanToWatch = 0,

    [Display(Name = "Watching")]
    Watching = 1,

    [Display(Name = "Completed")]
    Completed = 2,

    [Display(Name = "Dropped")]
    Dropped = 3
}

public static class GenreList
{
    public static readonly List<string> All = new()
    {
        "Action",
        "Adventure",
        "Animation",
        "Comedy",
        "Crime",
        "Documentary",
        "Drama",
        "Fantasy",
        "Horror",
        "Mystery",
        "Romance",
        "Sci-Fi",
        "Thriller",
        "Western"
    };
}

public class Movie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Movie title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "Director name cannot exceed 80 characters")]
    public string Director { get; set; } = string.Empty;

    [Range(1888, 2100, ErrorMessage = "Please enter a valid release year")]
    public int ReleaseYear { get; set; } = DateTime.UtcNow.Year;

    [Required(ErrorMessage = "Please select a genre")]
    public string Genre { get; set; } = "Drama";

    [Url(ErrorMessage = "Please enter a valid image URL")]
    public string? PosterUrl { get; set; }

    public WatchStatus Status { get; set; } = WatchStatus.PlanToWatch;

    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    public int Rating { get; set; } = 0; // 0 = unrated, 1-5 = stars

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? WatchedDate { get; set; }
}
