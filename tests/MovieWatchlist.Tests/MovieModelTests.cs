using System.ComponentModel.DataAnnotations;
using MovieWatchlist.Models;

namespace MovieWatchlist.Tests;

public class MovieModelTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void ValidMovie_PassesValidation()
    {
        var movie = new Movie
        {
            Title = "The Matrix",
            Director = "Wachowskis",
            Genre = "Sci-Fi",
            ReleaseYear = 1999,
            Rating = 5
        };

        var errors = ValidateModel(movie);
        Assert.Empty(errors);
    }

    [Fact]
    public void EmptyTitle_FailsValidation()
    {
        var movie = new Movie
        {
            Title = "",
            Genre = "Sci-Fi"
        };

        var errors = ValidateModel(movie);
        Assert.Contains(errors, e => e.MemberNames.Contains("Title"));
    }

    [Fact]
    public void InvalidRating_FailsValidation()
    {
        var movie = new Movie
        {
            Title = "Test Movie",
            Genre = "Action",
            Rating = 7 // Allowed is 1 to 5
        };

        var errors = ValidateModel(movie);
        Assert.Contains(errors, e => e.MemberNames.Contains("Rating"));
    }
}
