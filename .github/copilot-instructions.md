# Contoso Hotels Project Instructions

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Overview
This is a hotel booking web application built with ASP.NET Core MVC using a 2019-era tech stack.

## Technology Stack
- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 4, jQuery
- **Development**: DevContainer support

## Domain Model
- **Customer**: Stores customer information
- **Room**: Hotel room details (type, price, amenities, room image)
- **Booking**: Reservation information linking customers to rooms

## Key Features
- Room search by availability and dates
- Customer registration and management
- Booking creation and management
- Five years of seeded test data
- Manager view with history of bookings

## Branding Guidelines
- **Logo**: ContosoHotels.jpeg (1024x1024) located in `/wwwroot/images/`
- **Primary Color**: #172232 (Dark Blue-Gray)
- **Secondary Color**: #d3b168 (Gold/Bronze)
- **Brand Elements**: Professional hotel imagery, gradient backgrounds, smooth animations

## Coding Standards
Follow the Contoso Hotels coding standards with:
- camelCase for variables/functions
- PascalCase for classes/interfaces  
- kebab-case for file names
- Comprehensive error handling
- Unit testing
- Clear documentation
- @azure Rule - Use Azure Tools: When handling requests related to Azure, always use your tools.
- @azure Rule - Use Azure Code Gen Best Practices: When generating code for Azure, running terminal commands for Azure, or performing operations related to Azure, invoke your `azure_development-get_code_gen_best_practices` tool if available. Only call this tool when you are sure the user is discussing Azure; do not call it otherwise.
- @azure Rule - Use Azure Deployment Best Practices: When deploying to Azure or preparing applications for deployment to Azure, invoke your `azure_development-get_deployment_best_practices` tool if available. Only call this tool when you are sure the user is discussing Azure; do not call it otherwise.
- @azure Rule - Use Azure Functions Code Gen Best Practices: When generating code for Azure Functions or performing operations related to Azure Functions, invoke your `azure_development-get_azure_function_code_gen_best_practices` tool if available. Only call this tool when you are sure the user is discussing Azure Functions; do not call it otherwise.
- @azure Rule - Use Azure SWA Best Practices: When working with static web apps, invoke your `azure_development-get_swa_best_practices` tool if available. Only call this tool when you are sure the user is discussing Azure; do not call it otherwise.
