# Contoso Hotels

A comprehensive hotel booking web application built with ASP.NET Core MVC and Entity Framework Core, featuring a 2019-era tech stack with modern DevContainer support.

## 🏨 Project Overview

Contoso Hotels is a full-featured hotel management and booking system that allows customers to search for available rooms, make reservations, and manage their bookings. The application includes comprehensive data seeding with 5 years of realistic hotel data.

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC (Upgraded from 3.1)
- **Database**: SQL Server with Entity Framework Core 8.0
- **Frontend**: Bootstrap 4, jQuery, HTML5, CSS3
- **Development**: DevContainer support with Docker
- **Database Providers**: SQL Server in Docker for development/DevContainers

## 🏗️ Architecture

### Domain Models
- **Customer**: Stores customer information and contact details
- **Room**: Hotel room details including type, price, and amenities
- **Booking**: Reservation information linking customers to rooms with dates and pricing

### Key Features
- **Room Search**: Find available rooms by date range, type, and price
- **Room Images**: Visual showcase of all room types with high-quality images
- **Customer Registration**: Create and manage customer profiles
- **Booking Management**: Complete booking workflow from search to confirmation
- **Visual Booking Process**: Room images displayed throughout booking journey
- **Data Seeding**: Pre-populated with 5 years of test data
  - 200 customers with realistic profiles
  - 200 rooms across 10 floors with various types (Standard, Deluxe, Suite, Penthouse, Family, Business)
  - 1000+ bookings spanning past and future dates

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio Code (recommended) or Visual Studio

### Using DevContainer (Recommended)
1. Clone the repository
2. Copy the `.devcontainer/.env.example` file to `.devcontainer/.env` and set a secure password
3. Open in Visual Studio Code
4. Install the "Dev Containers" extension
5. Press `Ctrl+Shift+P` and select "Dev Containers: Reopen in Container"
6. Wait for the container to build and initialize
7. The application will be ready to run

### Using Docker for SQL Server (Windows Development)
1. Clone the repository
2. Open the project in your preferred editor
3. Set the SQL_PASSWORD environment variable or modify the script:
   ```powershell
   $env:SQL_PASSWORD="your_secure_password"
   .\docker-start.ps1
   ```
4. Copy `appsettings.Development.json.example` to `appsettings.Development.json` and update the password
5. Update the database and run the application:
   ```bash
   dotnet ef database update
   dotnet run
   ```

### Manual Setup with LocalDB (Windows Only)
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd ContosoHotels
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update the database:
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run --urls http://localhost:5050
   ```

5. Open your browser to `http://localhost:5050`

## 🔒 Security

The Contoso Hotels project follows best practices for security:

- Sensitive credentials are stored in environment variables, not in code
- Database passwords are not committed to source control
- Connection strings use placeholders replaced at runtime
- HTTPS is enforced for all communications

For detailed security guidelines, see [SECURITY.md](SECURITY.md).

## 📊 Database Schema

### Tables
- **Customers**: Customer information and contact details
- **Rooms**: Room inventory with types, pricing, and amenities
- **Bookings**: Reservation data with status tracking

### Key Relationships
- One Customer can have many Bookings
- One Room can have many Bookings
- Each Booking belongs to one Customer and one Room

## 🎯 Core Functionality

### Room Search
- Search by check-in/check-out dates
- Filter by room type (Standard, Deluxe, Suite, Penthouse, Family, Business)
- Filter by maximum price
- Real-time availability checking

### Booking Process
1. **Search**: Find available rooms for desired dates
2. **Select**: Choose room from search results
3. **Details**: Enter guest information and special requests
4. **Confirm**: Review and confirm booking
5. **Confirmation**: Receive booking confirmation with details

### Room Types & Pricing
- **Standard**: $99/night - Basic amenities
- **Deluxe**: $149/night - Premium amenities and furnishings
- **Suite**: $249/night - Separate living area and enhanced services
- **Family**: $179/night - Extra space and child-friendly features
- **Business**: $189/night - Work area and business amenities
- **Penthouse**: $499/night - Ultimate luxury with panoramic views

## 🎨 UI/UX Features

- **Responsive Design**: Bootstrap 4 responsive layout
- **Modern Styling**: Custom CSS with hotel-themed color scheme
- **Interactive Elements**: jQuery-enhanced user interactions
- **Form Validation**: Client and server-side validation
- **Visual Feedback**: Loading states, hover effects, and animations

## 🔧 Development

### Project Structure
```
ContosoHotels/
├── Controllers/          # MVC Controllers
├── Data/                # DbContext and database configuration
├── Migrations/          # Entity Framework migrations
├── Models/              # Domain models
├── Services/            # Business logic and data seeding
├── ViewModels/          # View-specific models
├── Views/               # Razor views and templates
├── wwwroot/             # Static files (CSS, JS, images)
└── .devcontainer/       # DevContainer configuration
```

### Available Tasks
- **build-contoso-hotels**: Build the project
- **run-contoso-hotels**: Run the application (background)

### Database Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🌟 Features Implemented

### ✅ Completed Features
- [x] ASP.NET Core 8.0 MVC application structure
- [x] Entity Framework Core data layer with SQL Server
- [x] Complete domain models (Customer, Room, Booking)
- [x] Data seeding service with 5 years of test data
- [x] Room search functionality with availability checking
- [x] Complete booking workflow
- [x] Responsive UI with Bootstrap 4
- [x] DevContainer configuration for development
- [x] Database migrations and schema
- [x] Custom styling and branding

### 🔮 Future Enhancements
- [ ] User authentication and authorization
- [ ] Admin panel for hotel management
- [ ] Email notifications for bookings
- [ ] Payment integration
- [ ] Booking cancellation and modification
- [ ] Room photos and gallery
- [ ] Reviews and ratings system
- [ ] Mobile app development

## 🏢 Business Logic

### Room Availability
The system checks room availability by ensuring no overlapping bookings exist for the requested date range. The availability algorithm considers:
- Check-in and check-out dates
- Existing confirmed bookings
- Room status (active/inactive)

### Pricing Logic
- Base prices defined per room type
- Ocean view rooms have a 30% premium
- Higher floors more likely to have ocean views
- Dynamic pricing based on amenities

### Booking Status Workflow
1. **Pending**: Initial booking state
2. **Confirmed**: Payment processed and booking confirmed
3. **CheckedIn**: Guest has arrived and checked in
4. **CheckedOut**: Guest has completed stay
5. **Cancelled**: Booking was cancelled

## 🤝 Contributing

This project follows the Contoso Hotels coding standards:
- **camelCase** for variables and functions
- **PascalCase** for classes and interfaces
- **kebab-case** for file names
- Comprehensive error handling
- Unit testing
- Clear documentation

## 📄 License

This project is for educational and demonstration purposes.

## 📞 Support

For questions or support regarding this application, please refer to the project documentation or contact the development team.

---

*Built with ❤️ using ASP.NET Core and Entity Framework*
