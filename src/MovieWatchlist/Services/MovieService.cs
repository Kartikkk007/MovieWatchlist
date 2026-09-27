using System.Text.Json;
using MovieWatchlist.Models;

namespace MovieWatchlist.Services;

public class MovieService : IMovieService
{
    private readonly string _storagePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<Movie> _movies = new();
    private bool _initialized = false;

    public MovieService(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        _storagePath = Path.Combine(dataDir, "movies.json");
    }

    // Secondary constructor for testing
    public MovieService(string storagePath)
    {
        _storagePath = storagePath;
        var dir = Path.GetDirectoryName(storagePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_initialized) return;

        await _lock.WaitAsync();
        try
        {
            if (_initialized) return;

            if (File.Exists(_storagePath))
            {
                var json = await File.ReadAllTextAsync(_storagePath);
                _movies = JsonSerializer.Deserialize<List<Movie>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Movie>();
            }
            else
            {
                _movies = GetDefaultSeedData();
                await SaveToFileAsync();
            }

            _initialized = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task SaveToFileAsync()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_movies, options);
        await File.WriteAllTextAsync(_storagePath, json);
    }

    public async Task<IReadOnlyList<Movie>> GetMoviesAsync(string? search = null, string? genre = null, bool? isWatched = null)
    {
        await EnsureLoadedAsync();

        var query = _movies.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(m =>
                m.Title.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                (m.Director != null && m.Director.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                (m.Notes != null && m.Notes.Contains(s, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(genre) && !genre.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(m => m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        }

        if (isWatched.HasValue)
        {
            query = query.Where(m => m.IsWatched == isWatched.Value);
        }

        return query.OrderByDescending(m => m.CreatedAt).ToList();
    }

    public async Task<Movie?> GetMovieByIdAsync(int id)
    {
        await EnsureLoadedAsync();
        return _movies.FirstOrDefault(m => m.Id == id);
    }

    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        await EnsureLoadedAsync();

        await _lock.WaitAsync();
        try
        {
            var nextId = _movies.Count > 0 ? _movies.Max(m => m.Id) + 1 : 1;
            movie.Id = nextId;
            movie.CreatedAt = DateTime.UtcNow;
            _movies.Add(movie);
            await SaveToFileAsync();
            return movie;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateMovieAsync(Movie movie)
    {
        await EnsureLoadedAsync();

        await _lock.WaitAsync();
        try
        {
            var existing = _movies.FirstOrDefault(m => m.Id == movie.Id);
            if (existing == null) return false;

            existing.Title = movie.Title;
            existing.Director = movie.Director;
            existing.Genre = movie.Genre;
            existing.ReleaseYear = movie.ReleaseYear;
            existing.IsWatched = movie.IsWatched;
            existing.Rating = movie.Rating;
            existing.Notes = movie.Notes;

            await SaveToFileAsync();
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        await EnsureLoadedAsync();

        await _lock.WaitAsync();
        try
        {
            var movie = _movies.FirstOrDefault(m => m.Id == id);
            if (movie == null) return false;

            _movies.Remove(movie);
            await SaveToFileAsync();
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> ToggleWatchedAsync(int id)
    {
        await EnsureLoadedAsync();

        await _lock.WaitAsync();
        try
        {
            var movie = _movies.FirstOrDefault(m => m.Id == id);
            if (movie == null) return false;

            movie.IsWatched = !movie.IsWatched;
            await SaveToFileAsync();
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<string>> GetGenresAsync()
    {
        await EnsureLoadedAsync();
        var defaultGenres = new[] { "Action", "Sci-Fi", "Drama", "Comedy", "Horror", "Animation", "Thriller", "Mystery", "Romance", "Adventure" };
        var customGenres = _movies.Select(m => m.Genre).Where(g => !string.IsNullOrWhiteSpace(g));
        return defaultGenres.Union(customGenres, StringComparer.OrdinalIgnoreCase).OrderBy(g => g).ToList();
    }

    public async Task<WatchlistStats> GetStatsAsync()
    {
        await EnsureLoadedAsync();
        var total = _movies.Count;
        var watched = _movies.Count(m => m.IsWatched);
        var unwatched = total - watched;
        var ratedMovies = _movies.Where(m => m.Rating.HasValue && m.Rating.Value > 0).ToList();
        var avgRating = ratedMovies.Count > 0 ? ratedMovies.Average(m => m.Rating!.Value) : 0.0;

        return new WatchlistStats
        {
            TotalMovies = total,
            WatchedCount = watched,
            UnwatchedCount = unwatched,
            AverageRating = Math.Round(avgRating, 1)
        };
    }

    private static List<Movie> GetDefaultSeedData()
    {
        return new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Inception",
                Director = "Christopher Nolan",
                Genre = "Sci-Fi",
                ReleaseYear = 2010,
                IsWatched = true,
                Rating = 5,
                Notes = "A thief who steals corporate secrets through dream-sharing technology.",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Movie
            {
                Id = 2,
                Title = "Dune: Part Two",
                Director = "Denis Villeneuve",
                Genre = "Sci-Fi",
                ReleaseYear = 2024,
                IsWatched = false,
                Rating = null,
                Notes = "Paul Atreides unites with Chani and the Fremen while seeking revenge.",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Movie
            {
                Id = 3,
                Title = "Spirited Away",
                Director = "Hayao Miyazaki",
                Genre = "Animation",
                ReleaseYear = 2001,
                IsWatched = true,
                Rating = 5,
                Notes = "During her family's move to the suburbs, a 10-year-old girl wanders into a world of spirits.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Movie
            {
                Id = 4,
                Title = "Knives Out",
                Director = "Rian Johnson",
                Genre = "Mystery",
                ReleaseYear = 2019,
                IsWatched = false,
                Rating = null,
                Notes = "A detective investigates the death of a patriarch of an eccentric, combative family.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
    }
}
