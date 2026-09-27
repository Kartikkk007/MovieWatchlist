# 🚀 Two-Account GitHub Collaboration & Merge Conflict Lab

Welcome to your hands-on laboratory for mastering GitHub collaboration! This guide teaches you how to use **two GitHub accounts** on your single PC to simulate a realistic team workflow.

---

## 📋 Table of Contents
1. [Overview & Roles](#1-overview--roles)
2. [Step 1: Uploading to Account 1 (Primary Owner)](#step-1-uploading-to-account-1-primary-owner)
3. [Step 2: Setting up Branch Protection (The Real Team Setup)](#step-2-setting-up-branch-protection)
4. [Step 3: Inviting Account 2 as a Collaborator](#step-3-inviting-account-2-as-a-collaborator)
5. [Step 4: Setting Up Account 2 Locally](#step-4-setting-up-account-2-locally)
6. [Step 5: Practice Flow #1 - Feature Branch, Pull Request & Approval](#step-5-practice-flow-1---feature-branch-pull-request--approval)
7. [Step 6: Practice Flow #2 - Intentional Merge Conflict & Resolution](#step-6-practice-flow-2---intentional-merge-conflict--resolution)
8. [Useful Git Collaboration Cheat Sheet](#useful-git-collaboration-cheat-sheet)

---

## 1. Overview & Roles

| Role | Account | Primary Responsibility |
|------|---------|------------------------|
| **Repo Owner** | **Account 1** | Owns the repo, sets branch rules, reviews & approves Pull Requests, merges to `main`. |
| **Contributor** | **Account 2** | Pulls latest code, creates feature branches, submits Pull Requests, responds to review feedback. |

---

## Step 1: Uploading to Account 1 (Primary Owner)

1. Open your browser logged into your **Primary GitHub Account (Account 1)**.
2. Click the **+** (top right) ➔ **New repository**.
3. Name it: `MovieWatchlist` (keep it Public or Private, your choice).
4. **DO NOT** initialize with a README, .gitignore, or license (we already have them!).
5. Click **Create repository**.
6. On your computer, open a terminal in this project folder (`C:\Users\karti\.gemini\antigravity\scratch\MovieWatchlist`):
   ```bash
   git init
   git add .
   git commit -m "feat: Initial commit for .NET 10 Movie Watchlist"
   git branch -M main
   git remote add origin https://github.com/YOUR_ACCOUNT_1_USERNAME/MovieWatchlist.git
   git push -u origin main
   ```

---

## Step 2: Setting up Branch Protection

In real software teams, nobody pushes directly to `main`. Every change goes through a Pull Request and requires code approval.

1. On GitHub, go to your repository: `Settings` ➔ `Branches`.
2. Click **Add branch protection rule** (or **Add rule**).
3. In **Branch name pattern**, enter: `main`
4. Check these boxes:
   - ✅ **Require a pull request before merging**
     - ✅ **Require approvals** (Set to `1`)
   - ✅ **Require status checks to pass before merging** (search for `build-and-test` after CI runs once)
5. Click **Create** / **Save changes**.

---

## Step 3: Inviting Account 2 as a Collaborator

1. Still logged in as **Account 1** on GitHub:
   - Go to `Settings` ➔ `Collaborators` (left sidebar).
   - Click **Add people**.
   - Type the username or email of **Account 2** and click **Add [Account 2] to this repository**.
2. Now, open an **Incognito / Private browser window** (or another browser where you are logged into **Account 2**):
   - Check the notification / email for Account 2 or visit the repository URL.
   - Click **Accept Invitation**.

---

## Step 4: Setting Up Account 2 Locally

To simulate two developers working on the same machine cleanly without mixing up Git identities:

1. Create a separate folder for Account 2:
   ```bash
   mkdir C:\Users\karti\.gemini\antigravity\scratch\MovieWatchlist-Account2
   cd C:\Users\karti\.gemini\antigravity\scratch\MovieWatchlist-Account2
   git clone https://github.com/YOUR_ACCOUNT_1_USERNAME/MovieWatchlist.git .
   ```
2. Configure this local folder with Account 2's name and email:
   ```bash
   git config user.name "Your Account 2 Name"
   git config user.email "your_account_2_email@example.com"
   ```

*(Now, commits in this folder will show up under Account 2!)*

---

## Step 5: Practice Flow #1 - Feature Branch, Pull Request & Approval

Now Account 2 will add a feature, create a PR, and Account 1 will review and approve it.

### As Account 2 (In `MovieWatchlist-Account2` directory):
1. Create and switch to a new branch:
   ```bash
   git checkout -b feature/add-duration-field
   ```
2. Open `src/MovieWatchlist/Models/Movie.cs` and add a new property:
   ```csharp
   public int? DurationMinutes { get; set; }
   ```
3. Commit and push the branch:
   ```bash
   git add .
   git commit -m "feat: add duration in minutes to movie model"
   git push -u origin feature/add-duration-field
   ```
4. In your browser (logged in as **Account 2**):
   - Go to the GitHub repository.
   - You will see a yellow banner: **"feature/add-duration-field had recent pushes"**.
   - Click **Compare & pull request**.
   - Enter title: `feat: Add DurationMinutes property to Movie model`
   - Click **Create pull request**.

### As Account 1 (In primary browser):
1. Go to the **Pull requests** tab. Click on Account 2's PR.
2. Notice the GitHub Actions CI check is running or green!
3. Click on the **Files changed** tab to inspect the diff.
4. Hover over a line, click the blue **+** icon, and leave an inline comment: e.g., *"Looks clean, nice addition!"*
5. Click **Review changes** (top right) ➔ Select **Approve** ➔ Click **Submit review**.
6. Click **Merge pull request** ➔ **Confirm merge**.
7. Delete the feature branch on GitHub (optional button provided).

### Syncing back:
- In Account 1's local directory: `git checkout main && git pull`
- In Account 2's local directory: `git checkout main && git pull`

---

## Step 6: Practice Flow #2 - Intentional Merge Conflict & Resolution

Merge conflicts happen when two developers edit the same lines of code in different branches. Let's create one on purpose and resolve it!

### 1. Account 1 modifies `main` directly:
In Account 1's local folder (`MovieWatchlist`):
1. Open `src/MovieWatchlist/Models/WatchlistStats.cs`.
2. Change:
   ```csharp
   public double AverageRating { get; set; }
   ```
   To:
   ```csharp
   public double AverageRatingScore { get; set; } // Renamed by Account 1
   ```
3. Commit and push:
   ```bash
   git add .
   git commit -m "refactor: rename AverageRating to AverageRatingScore"
   git push origin main
   ```

### 2. Account 2 modifies the SAME line in a branch (without pulling Account 1's changes):
In Account 2's local folder (`MovieWatchlist-Account2`):
1. Create a branch:
   ```bash
   git checkout -b feature/rating-display
   ```
2. Open the same file `src/MovieWatchlist/Models/WatchlistStats.cs`.
3. Change the same line to:
   ```csharp
   public double FormattedAverageRating { get; set; } // Renamed by Account 2
   ```
4. Commit and push:
   ```bash
   git add .
   git commit -m "feat: rename AverageRating to FormattedAverageRating"
   git push -u origin feature/rating-display
   ```
5. Open a Pull Request from `feature/rating-display` to `main`.

### 3. Observe the Conflict!
GitHub will immediately show:
> **"This branch has conflicts that must be resolved."**
> *(The Merge button is disabled!)*

### 4. How to Resolve:

#### Option A: Resolving Locally (Recommended Pro Way)
In Account 2's folder:
```bash
# 1. Fetch latest changes from main
git fetch origin

# 2. Merge main into your feature branch
git merge origin/main
```
Git will output:
`CONFLICT (content): Merge conflict in src/MovieWatchlist/Models/WatchlistStats.cs`

Open `src/MovieWatchlist/Models/WatchlistStats.cs`. You will see conflict markers:
```csharp
<<<<<<< HEAD (Current Change - Account 2)
    public double FormattedAverageRating { get; set; }
=======
    public double AverageRatingScore { get; set; }
>>>>>>> origin/main (Incoming Change - Account 1)
```

**To fix:**
1. Decide which name to keep (or combine them, e.g. `public double AverageRatingScore { get; set; }`).
2. Delete the markers (`<<<<<<<`, `=======`, `>>>>>>>`).
3. Save the file.
4. Stage and finish the merge:
   ```bash
   git add src/MovieWatchlist/Models/WatchlistStats.cs
   git commit -m "fix: resolve merge conflict between branch and main"
   git push
   ```
5. Look at GitHub: the PR will automatically update, show green, and allow merging!

---

## Useful Git Collaboration Cheat Sheet

| Command | Purpose |
|---------|---------|
| `git checkout -b <branch>` | Create and switch to a new branch |
| `git branch -a` | List all local and remote branches |
| `git fetch --all` | Download latest branches and commits from remote |
| `git pull origin main` | Update your current branch with changes from `main` |
| `git push -u origin <branch>` | Push new branch to GitHub and set tracking |
| `git status` | Check status of modified and conflicted files |
| `git log --oneline --graph` | View visual history of commits and branches |
