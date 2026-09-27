using MovieWatchlist.Models;
using MovieWatchlist.Services;

namespace MovieWatchlist.Tests;

public class MovieServiceTests : IDisposable
{
    private readonly string _testFile;
    private readonly MovieService _service;

    public MovieServiceTests()
    {
        _testFile = Path.Combine(Path.GetTempPath(), $"test_movies_{Guid.NewGuid():N}.json");
        _service = new MovieService(_testFile);
    }

    public void Dispose()
    {
        if (File.Exists(_testFile))
        {
            try { File.Delete(_testFile); } catch { }
        }
    }

    [Fact]
    public async Task AddMovieAsync_ShouldAssignIncrementalId()
    {
        var movie = new Movie
        {
            Title = "Interstellar",
            Director = "Christopher Nolan",
            Genre = "Sci-Fi",
            ReleaseYear = 2014
        };

        var added = await _service.AddMovieAsync(movie);

        Assert.True(added.Id > 0);
        var retrieved = await _service.GetMovieByIdAsync(added.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Interstellar", retrieved.Title);
    }

    [Fact]
    public async Task ToggleWatchedAsync_ShouldInvertWatchStatus()
    {
        var movie = await _service.AddMovieAsync(new Movie
        {
            Title = "Arrival",
            ReleaseYear = 2016,
            IsWatched = false
        });

        var result = await _service.ToggleWatchedAsync(movie.Id);
        Assert.True(result);

        var updated = await _service.GetMovieByIdAsync(movie.Id);
        Assert.NotNull(updated);
        Assert.True(updated.IsWatched);

        // Toggle back
        await _service.ToggleWatchedAsync(movie.Id);
        var reverted = await _service.GetMovieByIdAsync(movie.Id);
        Assert.NotNull(reverted);
        Assert.False(reverted.IsWatched);
    }

    [Fact]
    public async Task GetMoviesAsync_FilterByGenre_ReturnsOnlyMatching()
    {
        await _service.AddMovieAsync(new Movie { Title = "Toy Story", Genre = "Animation" });
        await _service.AddMovieAsync(new Movie { Title = "The Conjuring", Genre = "Horror" });

        var animationMovies = await _service.GetMoviesAsync(genre: "Animation");

        Assert.All(animationMovies, m => Assert.Equal("Animation", m.Genre));
    }

    [Fact]
    public async Task DeleteMovieAsync_RemovesMovieSuccessfully()
    {
        var movie = await _service.AddMovieAsync(new Movie { Title = "Temporary Movie" });
        var deleted = await _service.DeleteMovieAsync(movie.Id);

        Assert.True(deleted);
        var found = await _service.GetMovieByIdAsync(movie.Id);
        Assert.Null(found);
    }

    [Fact]
    public async Task GetStatsAsync_CalculatesCorrectCounts()
    {
        var stats = await _service.GetStatsAsync();
        Assert.Equal(stats.TotalMovies, stats.WatchedCount + stats.UnwatchedCount);
    }
}
