# TellMe-BE

This is the backend API for the TellMe application, built with ASP.NET Core. The project uses Entity Framework Core for database management, ASP.NET Core Identity for authentication, JWT for token-based authorization, SMTP for email services, and Redis for caching.

## Prerequisites

Before setting up the project, ensure you have the following installed:

- **.NET SDK** (version compatible with the project, e.g., .NET 8.0)
- **SQL Server** (e.g., SQL Server 2022)
- **Visual Studio** (with ASP.NET and web development workload) or another IDE like VS Code
- **Git** for cloning the repository
- **Redis** (local or a cloud service like Redis Enterprise Cloud)
- An **SMTP email service** (e.g., Gmail SMTP)

## Setup Instructions

### 1. Clone the Repository

Clone the project from GitHub and navigate to the project directory:

```bash
git clone https://github.com/Dwuangne/TellMe-BE.git
cd TellMe-BE/TellMeSystem
```

### 2. Install Dependencies

Restore NuGet packages to install project dependencies:

- In **Visual Studio**:
  - Go to **Tools &gt; NuGet Package Manager &gt; Manage NuGet Packages for Solution** and click **Restore**.
- Or, in a terminal (within the `TellMeSystem` directory):

```bash
dotnet restore
```

### 3. Create the `.env` File

In the root directory of the project (`TellMeSystem`), create a file named `.env`. Copy the following template and fill in the appropriate values for your environment:

```
# Database Connections
ConnectionStrings__TellMeConnectionString=
ConnectionStrings__TellMeAuthConnectionString=

# JWT Authentication
JwtAuth__Key=
JwtAuth__Issuer=
JwtAuth__Audience=

# Email SMTP Configuration
Email__Host=
Email__Port=
Email__SystemName=
Email__Sender=
Email__Password=

# Redis Configuration
Redis__ConnectionString=
```

**Notes**:

- Replace placeholders with actual values (e.g., SQL Server connection strings, JWT key, SMTP credentials, Redis connection string).
- Example SQL Server connection string: `Server=your_server;Database=your_database;User Id=your_user;Password=your_password;TrustServerCertificate=True`
- For Gmail SMTP, use `smtp.gmail.com` with port `587` and an **App Password** if 2FA is enabled.
- Ensure `.env` is listed in `.gitignore` to prevent committing sensitive information.

### 4. Initialize the Databases

The project uses two Entity Framework Core DbContexts:

- `TellMeAuthDBContext`: For authentication (ASP.NET Core Identity).
- `TellMeDBContext`: For application data.

To create the databases, use the **NuGet Package Manager Console** in Visual Studio:

1. **Open Package Manager Console**:

   - Go to **Tools &gt; NuGet Package Manager &gt; Package Manager Console**.

2. **Apply migrations for TellMeAuthDBContext**:

   - Run the following command to create the `TellMeAuthDB` database:

```powershell
Update-Database -Context TellMeAuthDBContext
```

3. **Apply migrations for TellMeDBContext**:
   - Run the following command to create the `TellMeDB` database:

```powershell
Update-Database -Context TellMeDBContext
```

**Notes**:

- Ensure the SQL Server instance specified in the `.env` file is running and accessible.

- If migrations do not exist, create them first:

  ```powershell
  Add-Migration InitialCreate -Context TellMeAuthDBContext
  Add-Migration InitialCreate -Context TellMeDBContext
  ```

- If you encounter connection errors, verify the connection strings in the `.env` file.

### 5. Run the Application

1. In **Visual Studio**:
   - Set `TellMeSystem` as the startup project.
   - Press `F5` to run the application.
2. Or, in a terminal:

```bash
dotnet run
```

The API will start at `https://localhost:7093` (or the port specified in your configuration).

### 6. Verify the Setup

- Access the API endpoints (e.g., Swagger UI at `https://localhost:7093/swagger` if enabled).
- Test authentication endpoints (e.g., `/api/auth/register` or `/api/auth/login`) to verify database and JWT functionality.
- Check Redis connectivity using a tool like **RedisInsight**.
- Test email functionality by registering a user and confirming email delivery.

## Security Notes

- **Do not commit the** `.env` **file**: Ensure `.env` is listed in `.gitignore`.
- **Secure credentials**: Avoid hardcoding sensitive data (e.g., JWT key, SMTP password, Redis password) in source code.
- **SQL Server TLS**: Use `TrustServerCertificate=True` only in development. In production, configure proper SSL certificates.
- **Gmail SMTP**: Use an **App Password** for `Email__Password` if 2FA is enabled on your Gmail account.

## Troubleshooting

- **Database connection errors**:
  - Verify SQL Server is running and the connection strings in `.env` are correct.
  - Check if the SQL Server instance allows connections from your machine.
- **Migration errors**:
  - Ensure DbContext classes are correctly configured in `Program.cs`.
  - Run `Add-Migration` if migrations are missing.
- **Redis timeout**:
  - Verify the Redis connection string in `.env`.
  - Check firewall settings or TLS requirements for your Redis server.
- **Email issues**:
  - Confirm SMTP settings (`Email__Host`, `Email__Port`, `Email__Sender`, `Email__Password`).
  - For Gmail, ensure **Less secure apps** is disabled and an **App Password** is used.

For further assistance, open an issue on the GitHub repository or contact the project maintainer.

## Contributing

Contributions are welcome! Please submit a pull request or open an issue to discuss improvements.
