using System.ComponentModel.DataAnnotations;

namespace MovieWatchlist.Models;

public class Movie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Movie title is required")]
    [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Director name cannot exceed 100 characters")]
    public string? Director { get; set; }

    [Required(ErrorMessage = "Please select or enter a genre")]
    [StringLength(50)]
    public string Genre { get; set; } = "Drama";

    [Range(1888, 2100, ErrorMessage = "Please enter a valid release year (1888-2100)")]
    public int ReleaseYear { get; set; } = DateTime.UtcNow.Year;

    public bool IsWatched { get; set; } = false;

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
    public int? Rating { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
