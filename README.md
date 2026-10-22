# CollegeFestMVC - AMTICS Technical Fest Registration System

ASP.NET Core MVC (.NET 10; for .NET 8 or 9 change TargetFramework in the .csproj and launch.json), C#, Razor, Bootstrap 5. Data stored in **MySQL** (MySqlConnector, plain ADO.NET).

## 1. Install
1. Install the **.NET 10 SDK**: https://dotnet.microsoft.com/download
2. Check: `dotnet --version` (should start with 10).
3. Install **VS Code** + the **C# Dev Kit** (or **C#**) extension.
   Different SDK (8/9)? Set `<TargetFramework>` in the .csproj and the `net10.0` path in
   `.vscode/launch.json` to match (for example net8.0).

## 2. Run
Open the `CollegeFestMVC` folder in VS Code (File > Open Folder), open the terminal (Ctrl + `) and run:

    dotnet restore
    dotnet build
    dotnet run

Or press **F5** in VS Code.

- HTTP : http://localhost:5000  (redirects to HTTPS)
- HTTPS: https://localhost:5001

## 3. HTTPS certificate
If the browser warns about the certificate, run once and restart the browser:

    dotnet dev-certs https --clean
    dotnet dev-certs https --trust

(Windows/macOS show a confirmation dialog. On Linux, click "Advanced > Proceed" in the browser.)

## 4. Common errors
| Problem | Fix |
|---|---|
| `dotnet` not recognised | Reinstall the SDK, then reopen the terminal/VS Code. |
| Port 5000/5001 already in use | Close the other app, or stop the old `dotnet run` with Ctrl+C. |
| Certificate warning | Run the two `dev-certs` commands above. |
| `NETSDK1045` / target framework error | SDK older than the project target: install the .NET 10 SDK or retarget. |
| `DefineStaticWebAssets` JSON error | Corrupt cache: delete the `bin` and `obj` folders, then `dotnet restore`. |
| Page unstyled / icons missing | Bootstrap, fonts and icons load from a CDN: connect to the internet. |
| `Unable to connect to any of the specified MySQL hosts` | MySQL is not running, or host/port in `appsettings.json` is wrong. |
| `Access denied for user 'root'` | Wrong password in `appsettings.json` -> `ConnectionStrings:FestDb`. |

## 5. How status and fee category work
- Free events (fee ₹0) are **Confirmed** immediately. Paid events are **Pending** until
  "Confirm payment" is clicked on the Details page.
- `GetRegistrationStatus()`: Confirmed if `IsRegistrationCompleted`, else Pending.
- `GetFeeCategory()`: ₹0 Free Event, up to ₹500 Standard Event, above ₹500 Premium Event.

## 6. MySQL setup (new)
1. Install **MySQL Server 8+** (https://dev.mysql.com/downloads/installer/) and remember the root password.
2. Open `appsettings.json` and replace `YOUR_MYSQL_PASSWORD` with your password.
3. `dotnet restore` then `dotnet run`. On first start the app **creates the `college_fest` database,
   the `events` and `participants` tables and seeds the 6 events automatically**.
4. Optional: run `Database/schema.sql` in MySQL Workbench, then
   `SELECT * FROM participants;` to see the saved registrations. Data now survives restarts.

### Tables
- `events` (event_id PK, event_name UNIQUE, maximum_participants, registration_fee)
- `participants` (participant_id PK AUTO_INCREMENT from 1001, name, email, department, academic_year,
  event_id FK -> events, is_team_event, registration_fee, is_completed, registered_on,
  UNIQUE(event_id, email))
