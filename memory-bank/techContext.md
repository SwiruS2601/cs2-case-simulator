# Technical Context: Counter-Strike 2 Case Unboxer

## Technologies Used

### Frontend

-   **Framework**: Nuxt.js 3 (Vue.js-based framework)
-   **Language**: TypeScript
-   **Styling**: Tailwind CSS
-   **State Management**: Vue composables
-   **Build System**: Vite (via Nuxt)
-   **Testing**: Vitest

### Backend

-   **Framework**: ASP.NET Core
-   **Language**: C#
-   **Database ORM**: Entity Framework Core
-   **API Style**: RESTful + SignalR for real-time communication
-   **Authentication**: API keys for admin functions

### Database

-   **DBMS**: PostgreSQL
-   **Migrations**: Handled via Entity Framework Core

### DevOps & Infrastructure

-   **Containerization**: Docker
-   **Orchestration**: Docker Compose
-   **Web Server**: Nginx (reverse proxy)
-   **CI/CD**: Build scripts and deployment automation
-   **Logging**: Structured logging to files

## Development Setup

To set up the development environment:

1. Clone the repository
2. Start the development containers using Docker Compose
3. Run the build script to prepare the environment
4. Start the backend and frontend development servers

Key commands:

-   `./build.sh` - Build and prepare the environment
-   Docker Compose commands to start/stop services

## Technical Constraints

### Performance Requirements

-   Case opening animations must be smooth (60fps)
-   API response times should be under 100ms
-   Application should be responsive on mobile devices

### Browser Compatibility

-   Modern browsers (Chrome, Firefox, Safari, Edge)
-   Mobile browsers on iOS and Android

### Deployment Constraints

-   Must be containerized for easy deployment
-   Database migrations must be automatic during deployment
-   Frontend must be statically built for performance

## Dependencies

### External APIs

-   None currently used - all data is self-contained

### Frontend Dependencies

-   Vue.js ecosystem libraries
-   Tailwind CSS for styling
-   Audio libraries for sound effects

### Backend Dependencies

-   Entity Framework Core for database access
-   SignalR for real-time features
-   ASP.NET Core standard libraries
