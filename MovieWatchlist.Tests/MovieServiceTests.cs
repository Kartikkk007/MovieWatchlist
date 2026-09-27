using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using MovieWatchlist.Models;
using MovieWatchlist.Services;
using Xunit;

namespace MovieWatchlist.Tests;

public class TestWebHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; } = string.Empty;
    public IFileProvider WebRootFileProvider { get; set; } = default!;
    public string ApplicationName { get; set; } = "MovieWatchlist";
    public IFileProvider ContentRootFileProvider { get; set; } = default!;
    public string ContentRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = "Development";
}

public class MovieServiceTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly MovieService _movieService;

    public MovieServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "MovieWatchlistTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);

        var env = new TestWebHostEnvironment
        {
            ContentRootPath = _tempDirectory
        };

        _movieService = new MovieService(env);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup exceptions in test temp dir
        }
    }

    [Fact]
    public async Task GetMoviesAsync_ShouldReturnSeedMovies_WhenFirstInitialized()
    {
        var movies = await _movieService.GetMoviesAsync();

        Assert.NotNull(movies);
        Assert.True(movies.Count >= 5);
        Assert.Contains(movies, m => m.Title == "Inception");
    }

    [Fact]
    public async Task AddMovieAsync_ShouldAddNewMovieWithGeneratedId()
    {
        var newMovie = new Movie
        {
            Title = "The Matrix",
            Director = "The Wachowskis",
            ReleaseYear = 1999,
            Genre = "Sci-Fi",
            Status = WatchStatus.PlanToWatch,
            Rating = 5,
            Notes = "Classic cyberpunk film."
        };

        var added = await _movieService.AddMovieAsync(newMovie);

        Assert.True(added.Id > 0);
        Assert.Equal("The Matrix", added.Title);

        var retrieved = await _movieService.GetMovieByIdAsync(added.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("The Matrix", retrieved.Title);
        Assert.Equal(5, retrieved.Rating);
    }

    [Fact]
    public async Task UpdateMovieAsync_ShouldUpdateExistingMovie()
    {
        var movies = await _movieService.GetMoviesAsync();
        var movie = movies.First();

        movie.Title = "Updated Title";
        movie.Rating = 4;
        movie.Status = WatchStatus.Completed;

        var updated = await _movieService.UpdateMovieAsync(movie);

        Assert.Equal("Updated Title", updated.Title);
        Assert.Equal(4, updated.Rating);
        Assert.Equal(WatchStatus.Completed, updated.Status);

        var retrieved = await _movieService.GetMovieByIdAsync(movie.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Updated Title", retrieved.Title);
    }

    [Fact]
    public async Task DeleteMovieAsync_ShouldRemoveMovie()
    {
        var movies = await _movieService.GetMoviesAsync();
        var movieToDelete = movies.First();
        var initialCount = movies.Count;

        var result = await _movieService.DeleteMovieAsync(movieToDelete.Id);

        Assert.True(result);

        var updatedList = await _movieService.GetMoviesAsync();
        Assert.Equal(initialCount - 1, updatedList.Count);
        Assert.DoesNotContain(updatedList, m => m.Id == movieToDelete.Id);
    }

    [Fact]
    public async Task GetGenresAsync_ShouldReturnDefaultGenres()
    {
        var genres = await _movieService.GetGenresAsync();

        Assert.NotNull(genres);
        Assert.Contains("Sci-Fi", genres);
        Assert.Contains("Action", genres);
        Assert.Contains("Drama", genres);
    }
}
