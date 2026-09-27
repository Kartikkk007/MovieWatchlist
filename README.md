# 🎬 Movie Watchlist (.NET 10)

A clean, modern ASP.NET Core Razor Pages application built with **.NET 10** for managing your personal movie watchlist and serving as an interactive sandbox to learn **GitHub Collaboration, Pull Requests, Approvals, and Merge Conflict Resolution**.

---

## 🌟 Features

- **Movie Management**: Add movies, set release year, genre, director, notes, and 1-5 star ratings.
- **Quick Status Toggle**: Mark movies as Watched / Unwatched with one click.
- **Smart Filtering & Search**: Filter by Genre, Status (Watched / To Watch), and search titles/directors/notes.
- **Summary Dashboard**: Real-time counter for Total Movies, To Watch, Watched, and Average Rating.
- **Persistent JSON Storage**: Changes persist automatically in `Data/movies.json` with pre-seeded popular movies.
- **Automated Testing**: Comprehensive unit tests with xUnit covering services and models.
- **GitHub Actions CI**: Automated workflow configured to build and test on every push and PR.

---

## 🚀 Running Locally

Ensure you have the **.NET 10 SDK** installed:

```bash
# Check your .NET version
dotnet --version

# Run the project
dotnet run --project src/MovieWatchlist
```

Open your browser and navigate to:
```
http://localhost:5000 (or the HTTPS port displayed in the console)
```

### Running Unit Tests:
```bash
dotnet test
```

---

## 🤝 GitHub Collaboration Lab

This repository contains a dedicated guide for learning how to collaborate using **two GitHub accounts**:
- Creating repositories & branch protection rules
- Submitting Pull Requests
- Reviewing diffs and approving PRs
- Simulating and resolving merge conflicts both locally and on GitHub

👉 **Read the full tutorial in [COLLABORATION_GUIDE.md](COLLABORATION_GUIDE.md)** or visit the **GitHub Guide** tab in the web application!
