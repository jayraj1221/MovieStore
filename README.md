# MovieStore
The purpose of the MovieStore application is to provide a seamless platform for users to purchase. Users can easily browse through a diverse collection of films, make purchases, and manage their movie  from a user-friendly interface. Additionally, the application empowers administrators to efficiently manage the movie inventory, including adding, updating, and removing titles, ensuring that the platform remains up-to-date with the latest releases and popular films. This dual functionality creates an engaging experience for users while streamlining management for admins.
## Installation
1.) To set up the MovieStore application locally, follow these steps:

2.) Clone the Repository: git clone https://github.com/jayraj1221/moviestore.git

3.) Navigate to the Project Directory: cd moviestore

4.) Restore Dependencies: Run the command to restore the necessary packages: dotnet restore

5.) Set Up the Database: Update the connection string in the appsettings.json file to point to your SQL Server instance,Run migrations to create the database schema.

6.) Run the Application.

## Technologies Used

Frontend: HTML, CSS, Bootstrap, JavaScript

Backend: ASP.NET Core MVC

Database: Entity Framework Core with SQL Server

Session Management: ASP.NET Core session middleware for user authentication and management

## Usage
Register: Click on "Register" to create an account.

Login: Enter your credentials to access your dashboard.

Browse Movies: Explore the movie catalog for purchase.

Admin Access: Log in with admin credentials to manage movies and users.
