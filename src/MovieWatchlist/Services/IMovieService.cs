using MovieWatchlist.Models;

namespace MovieWatchlist.Services;

public interface IMovieService
{
    Task<IReadOnlyList<Movie>> GetMoviesAsync(string? search = null, string? genre = null, bool? isWatched = null);
    Task<Movie?> GetMovieByIdAsync(int id);
    Task<Movie> AddMovieAsync(Movie movie);
    Task<bool> UpdateMovieAsync(Movie movie);
    Task<bool> DeleteMovieAsync(int id);
    Task<bool> ToggleWatchedAsync(int id);
    Task<IReadOnlyList<string>> GetGenresAsync();
    Task<WatchlistStats> GetStatsAsync();
}
