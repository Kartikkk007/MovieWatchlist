using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieWatchlist.Models;
using MovieWatchlist.Services;

namespace MovieWatchlist.Pages;

public class IndexModel : PageModel
{
    private readonly IMovieService _movieService;

    public IndexModel(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public IReadOnlyList<Movie> Movies { get; set; } = Array.Empty<Movie>();
    public WatchlistStats Stats { get; set; } = new();
    public IReadOnlyList<string> AvailableGenres { get; set; } = Array.Empty<string>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedGenre { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        bool? isWatched = null;
        if (StatusFilter == "watched") isWatched = true;
        else if (StatusFilter == "unwatched") isWatched = false;

        Movies = await _movieService.GetMoviesAsync(SearchTerm, SelectedGenre, isWatched);
        Stats = await _movieService.GetStatsAsync();
        AvailableGenres = await _movieService.GetGenresAsync();
    }

    public async Task<IActionResult> OnPostToggleWatchedAsync(int id)
    {
        var success = await _movieService.ToggleWatchedAsync(id);
        if (success)
        {
            StatusMessage = "Movie status updated successfully!";
        }
        return RedirectToPage(new { SearchTerm, SelectedGenre, StatusFilter });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var success = await _movieService.DeleteMovieAsync(id);
        if (success)
        {
            StatusMessage = "Movie deleted from watchlist.";
        }
        return RedirectToPage(new { SearchTerm, SelectedGenre, StatusFilter });
    }
}
