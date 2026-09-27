using System.Text.Json;
using MovieWatchlist.Models;

namespace MovieWatchlist.Services;

public class MovieService : IMovieService
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private List<Movie>? _cachedMovies;

    public MovieService(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        _filePath = Path.Combine(dataDir, "movies.json");
    }

    private async Task<List<Movie>> LoadMoviesInternalAsync()
    {
        if (_cachedMovies != null)
        {
            return _cachedMovies;
        }

        if (File.Exists(_filePath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var list = JsonSerializer.Deserialize<List<Movie>>(json, options);
                if (list != null && list.Count > 0)
                {
                    _cachedMovies = list;
                    return _cachedMovies;
                }
            }
            catch
            {
                // Fallback to seed on deserialization error
            }
        }

        _cachedMovies = GetSeedMovies();
        await SaveMoviesInternalAsync(_cachedMovies);
        return _cachedMovies;
    }

    private async Task SaveMoviesInternalAsync(List<Movie> movies)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(movies, options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<Movie>> GetMoviesAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var movies = await LoadMoviesInternalAsync();
            return movies.OrderByDescending(m => m.CreatedAt).ToList();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Movie?> GetMovieByIdAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var movies = await LoadMoviesInternalAsync();
            return movies.FirstOrDefault(m => m.Id == id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        await _semaphore.WaitAsync();
        try
        {
            var movies = await LoadMoviesInternalAsync();
            movie.Id = movies.Count > 0 ? movies.Max(m => m.Id) + 1 : 1;
            movie.CreatedAt = DateTime.UtcNow;
            movies.Add(movie);
            await SaveMoviesInternalAsync(movies);
            return movie;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Movie> UpdateMovieAsync(Movie movie)
    {
        await _semaphore.WaitAsync();
        try
        {
            var movies = await LoadMoviesInternalAsync();
            var index = movies.FindIndex(m => m.Id == movie.Id);
            if (index != -1)
            {
                movies[index] = movie;
                await SaveMoviesInternalAsync(movies);
            }
            return movie;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var movies = await LoadMoviesInternalAsync();
            var movie = movies.FirstOrDefault(m => m.Id == id);
            if (movie != null)
            {
                movies.Remove(movie);
                await SaveMoviesInternalAsync(movies);
                return true;
            }
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<List<string>> GetGenresAsync()
    {
        return Task.FromResult(GenreList.All);
    }

    private static List<Movie> GetSeedMovies()
    {
        return new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Inception",
                Director = "Christopher Nolan",
                ReleaseYear = 2010,
                Genre = "Sci-Fi",
                PosterUrl = "https://images.unsplash.com/photo-1536440136628-849c177e76a1?auto=format&fit=crop&w=600&q=80",
                Status = WatchStatus.Completed,
                Rating = 5,
                Notes = "Masterpiece mind-bending thriller. Stunning soundtrack by Hans Zimmer.",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                WatchedDate = DateTime.UtcNow.AddDays(-5)
            },
            new Movie
            {
                Id = 2,
                Title = "Dune: Part Two",
                Director = "Denis Villeneuve",
                ReleaseYear = 2024,
                Genre = "Sci-Fi",
                PosterUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=600&q=80",
                Status = WatchStatus.Completed,
                Rating = 5,
                Notes = "Incredible cinematic spectacle and sound design. Must watch in IMAX.",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                WatchedDate = DateTime.UtcNow.AddDays(-2)
            },
            new Movie
            {
                Id = 3,
                Title = "Spirited Away",
                Director = "Hayao Miyazaki",
                ReleaseYear = 2001,
                Genre = "Animation",
                PosterUrl = "https://images.unsplash.com/photo-1578632767115-351597cf2477?auto=format&fit=crop&w=600&q=80",
                Status = WatchStatus.Watching,
                Rating = 4,
                Notes = "Ghibli magic at its finest. Re-watching with family.",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new Movie
            {
                Id = 4,
                Title = "The Dark Knight",
                Director = "Christopher Nolan",
                ReleaseYear = 2008,
                Genre = "Action",
                PosterUrl = "https://images.unsplash.com/photo-1509281373149-e957c6296406?auto=format&fit=crop&w=600&q=80",
                Status = WatchStatus.Completed,
                Rating = 5,
                Notes = "Heath Ledger gives an all-time legendary performance as Joker.",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                WatchedDate = DateTime.UtcNow.AddDays(-1)
            },
            new Movie
            {
                Id = 5,
                Title = "Oppenheimer",
                Director = "Christopher Nolan",
                ReleaseYear = 2023,
                Genre = "Drama",
                PosterUrl = "https://images.unsplash.com/photo-1440404653325-ab127d49abc1?auto=format&fit=crop&w=600&q=80",
                Status = WatchStatus.PlanToWatch,
                Rating = 0,
                Notes = "Heard intense praise for Cillian Murphy's performance.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Movie
            {
                Id = 6,
                Title = "Spider-Man: Across the Spider-Verse",
                Director = "Joaquim Dos Santos, Kemp Powers",
                ReleaseYear = 2023,
                Genre = "Animation",
                PosterUrl = "https://images.unsplash.com/photo-1635805737707-575885ab0820?auto=format&fit=crop&w=600&q=80",
                Status = WatchStatus.PlanToWatch,
                Rating = 0,
                Notes = "Need to binge both parts on the weekend!",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
    }
}
