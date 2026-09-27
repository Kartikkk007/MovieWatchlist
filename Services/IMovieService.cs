using MovieWatchlist.Models;

namespace MovieWatchlist.Services;

public interface IMovieService
{
    Task<List<Movie>> GetMoviesAsync();
    Task<Movie?> GetMovieByIdAsync(int id);
    Task<Movie> AddMovieAsync(Movie movie);
    Task<Movie> UpdateMovieAsync(Movie movie);
    Task<bool> DeleteMovieAsync(int id);
    Task<List<string>> GetGenresAsync();
}
