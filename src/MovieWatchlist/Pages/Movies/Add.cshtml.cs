using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieWatchlist.Models;
using MovieWatchlist.Services;

namespace MovieWatchlist.Pages.Movies;

public class AddModel : PageModel
{
    private readonly IMovieService _movieService;

    public AddModel(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [BindProperty]
    public Movie Movie { get; set; } = new()
    {
        ReleaseYear = DateTime.UtcNow.Year,
        Genre = "Drama"
    };

    public IReadOnlyList<string> ExistingGenres { get; set; } = Array.Empty<string>();

    public async Task OnGetAsync()
    {
        ExistingGenres = await _movieService.GetGenresAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ExistingGenres = await _movieService.GetGenresAsync();
            return Page();
        }

        await _movieService.AddMovieAsync(Movie);
        TempData["StatusMessage"] = $"Movie \"{Movie.Title}\" added successfully!";
        return RedirectToPage("/Index");
    }
}
