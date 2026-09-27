using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieWatchlist.Models;
using MovieWatchlist.Services;

namespace MovieWatchlist.Pages.Movies;

public class EditModel : PageModel
{
    private readonly IMovieService _movieService;

    public EditModel(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [BindProperty]
    public Movie Movie { get; set; } = default!;

    public IReadOnlyList<string> ExistingGenres { get; set; } = Array.Empty<string>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        Movie = movie;
        ExistingGenres = await _movieService.GetGenresAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ExistingGenres = await _movieService.GetGenresAsync();
            return Page();
        }

        var updated = await _movieService.UpdateMovieAsync(Movie);
        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = $"Movie \"{Movie.Title}\" updated successfully!";
        return RedirectToPage("/Index");
    }
}
