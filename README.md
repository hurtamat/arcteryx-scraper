# Arc'teryx Outlet Monitor

A high-performance monitoring system for Arc'teryx outlet stores across multiple geographical zones. The application automatically tracks product availability, sends instant email notifications when new items are released, monitors site downtime, and provides advanced product sorting and filtering capabilities.

## Features

### Real-Time Product Monitoring
- **Multi-Zone Support**: Monitors Arc'teryx outlet stores across different geographical regions
- **Automated Data Collection**: Periodically scrapes product data using headless browser automation
- **Lightning-Fast Notifications**: Notifies registered users within seconds of new product releases
- **Smart Caching**: Efficient caching mechanism to minimize unnecessary requests and improve performance

### User Notifications
- **Email Alerts**: Registered users receive instant email notifications when:
  - New products are added to monitored zones
  - Products matching their preferences become available
  - Price changes occur on watched items
- **Customizable Preferences**: Users can specify which zones and product categories to monitor

### Site Health Monitoring
- **Downtime Tracking**: Automatically logs Arc'teryx outlet site outages and downtime periods
- **Historical Data**: Maintains records of site availability and response times
- **Alert System**: Notifies administrators of prolonged outages

### Advanced Sorting & Filtering
- **Multi-Criteria Sorting**: Sort products by:
  - Price (ascending/descending)
  - Product name
  - Category
  - Release date
  - Availability
- **Dynamic Filtering**: Filter by zone, category, price range, and availability status
- **Search Functionality**: Quick search across all product attributes

## Tech Stack

### Backend
- **.NET 9.0**: Latest .NET framework for high-performance server-side processing
- **ASP.NET Core**: Web API and server-side rendering
- **PuppeteerSharp**: Headless Chromium automation for reliable web scraping
- **Memory Caching**: In-memory caching for optimized performance

### Frontend
- **Blazor WebAssembly**: Interactive SPA with server-side and client-side rendering modes
- **Razor Components**: Reusable UI components
- **Responsive Design**: Mobile-friendly interface

### Infrastructure
- **Docker**: Containerized deployment for consistent environments
- **Scheduled Jobs**: Background workers for automated data collection
- **Email Service**: Automated notification system

## Project Structure

```
arcteryxScraper/
├── arcteryxScraper/              # Main server application
│   ├── Components/               # Blazor server components
│   ├── Controllers/              # API endpoints
│   ├── Models/                   # Data models
│   ├── Parsers/                  # HTML parsing logic
│   ├── HtmlFetcher.cs           # Web scraping service
│   ├── HtmlParser.cs            # HTML content parser
│   └── Program.cs               # Application entry point
├── arcteryxScraper.Client/       # Blazor WebAssembly client
│   ├── Models/                   # Client-side models
│   └── Program.cs               # Client entry point
├── arcteryxScraper.Tests/        # Unit and integration tests
└── arcteryxScraper.sln          # Visual Studio solution
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Docker (optional, for containerized deployment)
- A modern web browser

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd arcteryxScraper
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Install Playwright browsers (required for PuppeteerSharp):
```bash
dotnet build
# Browsers will be installed automatically on first run
```

### Configuration

Update `appsettings.json` with your configuration:

```json
{
  "Monitoring": {
    "Zones": ["US", "CA", "UK", "EU"],
    "UpdateIntervalMinutes": 5
  },
  "Email": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "Username": "your-email@example.com",
    "Password": "your-password"
  }
}
```

### Running the Application

#### Development Mode

```bash
dotnet run --project arcteryxScraper/arcteryxScraper
```

The application will be available at `https://localhost:5001`

#### Using Docker

```bash
docker build -t arcteryx-scraper .
docker run -p 8080:8080 arcteryx-scraper
```

### Running Tests

```bash
dotnet test
```

## Usage

### Registering for Notifications

1. Navigate to the application homepage
2. Create an account or sign in
3. Go to Settings > Notification Preferences
4. Select zones and product categories to monitor
5. Verify your email address
6. Start receiving instant notifications!

### Viewing Products

1. Browse products from all monitored zones
2. Use the advanced filters to narrow down results
3. Sort by price, name, or release date
4. Click on any product to view details

### Monitoring Site Status

1. Navigate to the Status Dashboard
2. View real-time site availability
3. Check historical downtime logs
4. Export reports for analysis

## API Endpoints

### Products

- `GET /api/products` - Retrieve all products
- `GET /api/products/{zone}` - Get products for a specific zone
- `POST /api/products/search` - Advanced search with filters

### Monitoring

- `GET /api/status` - Current site status
- `GET /api/status/history` - Downtime history

### User Management

- `POST /api/users/register` - Register new user
- `POST /api/users/preferences` - Update notification preferences
- `GET /api/users/notifications` - View notification history

## Performance

- **Update Frequency**: Products are checked every 5 minutes by default
- **Notification Latency**: Email notifications sent within 10-30 seconds of detection
- **Cache Duration**: Product data cached for 2 minutes to reduce server load
- **Concurrent Zones**: Monitors multiple zones simultaneously for maximum efficiency

## Roadmap

- [ ] Mobile application (iOS/Android)
- [ ] Push notifications via web push API
- [ ] Price drop alerts
- [ ] Webhook integrations
- [ ] Historical price tracking and charts
- [ ] Product availability predictions using ML
- [ ] Discord/Slack integration
- [ ] Multi-language support

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Disclaimer

This application is for educational and personal use only. Please respect Arc'teryx's terms of service and rate limits. The developers are not responsible for any misuse of this software.

## Support

For issues, questions, or suggestions, please open an issue on GitHub.

---

Built with .NET 9.0 and Blazor WebAssembly
