# PreRead && Configuration
> Made in 2 days
> 
> For Email confirmation, you can use <b>https://mailtrap.io</b>

Configure
> <code>cd ClinicWebApi</code>
>
> <code>dotnet ef migrations add InitialCreate</code>
>
> <code>dotnet ef database update</code>
>
> Mostly, 97% of work is done

# Structure
<pre>
└───ClinicWebApi
    │   appsettings.Development.json
    │   appsettings.json
    │   ClinicWebApi.csproj
    │   ClinicWebApi.csproj.user
    │   ClinicWebApi.http
    │   Program.cs
    │   WeatherForecast.cs
    │
    │
    ├───Controllers
    │       AccountController .cs
    │       BookingController.cs
    │       CategoryController.cs
    │       DoctorController .cs
    │       WeatherForecastController.cs
    │
    ├───Data
    │       ApplicationDbContext.cs
    │
    ├───DTO
    │       CategoryDTO.cs
    │       CreateBookingDTO.cs
    │       DoctorDTO.cs
    │       ResendConfirmationEmailDTO.cs
    │       UserRegistrationDTO.cs
    │
    ├───Extensions
    │       FluentEmailExtensions.cs
    │
    │
    ├───Models
    │       BaseUser.cs
    │       SeedRoles.cs
    │
    │
    ├───Properties
    │       launchSettings.json
    │
    └───Services
            EmailSender.cs
            EmailService.cs
</pre>
