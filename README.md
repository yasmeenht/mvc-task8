# University Cafeteria Supply Requests

Open the solution in Visual Studio, or run these commands from the folder containing mvc-task8.csproj:

```powershell
dotnet user-secrets set "SuperAdmin:Email" "your-email@example.com"
dotnet user-secrets set "SuperAdmin:Password" "your-strong-password"
dotnet run
```

The app uses the SQL Server connection in appsettings.json and applies migrations when it starts. Use an email that has not been registered yet for the first SuperAdmin. Sign in with that account, then open Users to assign roles. New registrations receive the User role.

Users can browse products and manage their own requests. Admin and SuperAdmin can manage products and approve or reject pending requests. A product used by a request cannot be deleted while that request exists.
