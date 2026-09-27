namespace MovieWatchlist.Models;

public class WatchlistStats
{
    public int TotalMovies { get; set; }
    public int WatchedCount { get; set; }
    public int UnwatchedCount { get; set; }
    public double AverageRating { get; set; }
}
