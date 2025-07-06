## Screenshots

   ### Login Page
![image](https://github.com/user-attachments/assets/ce446743-927f-4646-a051-99c59c2610cd)












# Habbit Tracker

A minimalist habit tracking and reflection web application built with ASP.NET Core Razor Pages.

## Features

- **Habit Tracking**: Add, delete, and mark habits as completed for each day
- **Memorable Moments**: Record daily highlights and notes
- **Visual Analytics**: Chart.js graph showing habit completion trends
- **User Management**: Simple username-based login system
- **Month/Year Navigation**: Navigate between different time periods
- **Responsive Design**: Works on desktop and mobile devices

## Technology Stack

- **Backend**: ASP.NET Core 9.0 with Razor Pages
- **Database**: SQLite with Entity Framework Core
- **Frontend**: Bootstrap 5, Chart.js
- **Deployment**: Docker support with docker-compose

## Quick Start

### Local Development

1. **Prerequisites**
   - .NET 9.0 SDK
   - Git

2. **Clone and Run**
   ```bash
   git clone <repository-url>
   cd HabbitTrackerRazor
   dotnet restore
   dotnet run
   ```

3. **Access the Application**
   - Open http://localhost:5000
   - Login with any username
   - Start tracking your habits!

### Docker Deployment

1. **Build and Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

2. **Or Build Docker Image Manually**
   ```bash
   docker build -t habbittracker .
   docker run -p 5000:80 -p 5001:443 habbittracker
   ```

## Production Deployment

### Environment Variables

Set these environment variables for production:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80;https://+:443
```

### Database

The application uses SQLite by default. For production, you can:

1. **Use SQLite** (current): Database file is stored in the application directory
2. **Use PostgreSQL**: Update connection string in `appsettings.Production.json`
3. **Use SQL Server**: Update connection string and add appropriate NuGet packages

### Security Features

- HTTPS redirection
- Security headers (XSS protection, content type options, etc.)
- Session management with secure cookies
- Antiforgery token protection

## API Endpoints

- `GET /` - Main application
- `GET /Test` - Test page for verifying functionality
- `GET /health` - Health check endpoint

## Project Structure

```
HabbitTrackerRazor/
├── Models/                 # Data models
│   ├── User.cs
│   ├── HabitEntry.cs
│   ├── MemorableMoment.cs
│   └── HabbitTrackerContext.cs
├── Pages/                  # Razor Pages
│   ├── Index.cshtml       # Main application page
│   ├── Index.cshtml.cs    # Main page logic
│   ├── Test.cshtml        # Test page
│   └── Test.cshtml.cs     # Test page logic
├── Migrations/            # Entity Framework migrations
├── wwwroot/              # Static files
├── Program.cs            # Application startup
├── appsettings.json      # Configuration
├── Dockerfile            # Docker configuration
├── docker-compose.yml    # Docker Compose configuration
└── README.md             # This file
```

## Development

### Adding New Features

1. **Database Changes**: Create new migrations
   ```bash
   dotnet ef migrations add MigrationName
   dotnet ef database update
   ```

2. **New Pages**: Add Razor Pages in the `Pages/` directory

3. **Styling**: Update CSS in the page files or add to `wwwroot/`

### Testing

Use the built-in test page at `/Test` to verify:
- User login/logout
- Month/year navigation
- Habit management (add/delete/mark as done)
- Memorable moments
- Graph functionality

## License

This project is open source and available under the MIT License.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## Support

For issues and questions, please create an issue in the repository. 
