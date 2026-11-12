# Job Automation Bot

A C# and .NET Windows Forms application that aggregates job listings from multiple sources, scores and filters opportunities, and automates parts of the job application process using Playwright.

## Features

- Multi-source job aggregation (Adzuna, LinkedIn, Indeed, and Dice)
- Skill-based job scoring and ranking
- Job filtering by seniority, recruiters, contract roles, and clearance requirements
- Resume uploading and application form completion
- Browser automation using Playwright
- Duplicate job detection
- Daily summaries and application logging
- Environment-variable-based API key management

## Tech Stack

- C# and .NET 10
- Windows Forms
- Microsoft Playwright
- Async/Await
- Adzuna API
- Custom job scoring and filtering engine

## Configuration

The Adzuna API credentials are read from Windows environment variables.

Configure these two user environment variables:

- `ADZUNA_APP_ID`
- `ADZUNA_APP_KEY`

Restart the application after configuring the variables.

Never commit API keys, passwords, browser sessions, or personal application data to GitHub.

## How It Works

1. Retrieves job listings from supported sources.
2. Removes duplicate listings.
3. Filters jobs based on configured criteria.
4. Scores opportunities based on skills, title relevance, seniority, and location.
5. Automates application steps where supported.
6. Records application activity for review.

## Future Improvements

- Add more job sources, such as ZipRecruiter and Glassdoor.
- Add AI-generated cover letters.
- Add a dashboard for application analytics.
- Add a job preview panel in the UI.
