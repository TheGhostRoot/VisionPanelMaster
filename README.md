### Vision Panel
An alternative to pterodactyl panel. 
It uses C# ASP.NET for the website and Postgresql for th database.
The Dotnet or .NET version is .NET 8.0

This docs contains only information about file locations and how the backend works.
You will not find docs about how the HTML or CSS or JS works.

# Webpages
- They are all in `/Web` folder
- All supported assets:
`.jpg, .png, .jpeg, .ogg, .mp4, .mov, .mp3, .css, .js, .html`
- If you want to make templates then here are the requirements:
```
1. The name of the folders and the main HTML file can be changed from `appsettings.json`.
2. You need to include the Terms Of Service in the template and not hide it from the user.
3. You can sell the templates for a price, but you can't sell this panel for a price no
matter if it is modified or not.
```
- By default webpages names:
```
Login: /Auth/Login
Verify Email: /Auth/VerifyEmail
Error: /Error
Terms Of Service: /ToS
```

# Endpoints
- They are all in `/Routes` folder.
- We use Minimal API for easier endpoints and utility function that.
 automaticly adds endpoints for all of the webpage assets as folders and files.
- Here is a list of all endpoints seperated by `;`:
```
Login: / ; /login
Verify Email: /verify_email
Error: /error
Terms Of Service: /tos
```
- Here is a list of their file locations:
```
Login: /Auth/LoginRoute.cs
Verify Email: /Auth/VerifyEmail.cs
Error: /Error/ErrorRoute.cs
Terms Of Service: /ToS/ToSRoute.cs
```

# Dependencies
- Npgsql
- ASP.NET (Minimal API)

# Windows setup for Postgres
- https://www.postgresql.org/download/
- To ensure that postgres server is running on windows 10 follow the steps:
```
1. Go to Task Manager -> Services
2. Search for 'postgresql-x64-16' or whatever the version is
3. Right click and make sure it is started
```
- Download pgAdmin4 to connect to the server
- https://www.pgadmin.org/download/pgadmin-4-windows/

# Database (Default)
- Table Users:
```
user_id big number auto increase
username UNIQUE max length is 30
email UNIQUE max length is 100
password is not UNIQUE and long as it needs  (it is a SHA256 hash)
```

- Table Servers:
```
server_id big number auto increase
name max length is 255
enable_machine_logs boolean default is true
enable_user_logs boolean default is true
```

- Table MachineLogs:
```
server_id referes to server_id in Server
datetime the date and time of the log default is NOW()
cpu_load text as big as it needs
ram_used_mb bigint
disk_operations_read_and_write_per_second bigint
network_inbound_mb_per_second bigint
network_outbound_mb_per_second bigint
gpu_load bigint
```

- Table UserLogs:
```
server_id REFERENCES to table Servers to value server_id
datetime the date and time of the log default is NOW()
activity text describing what the user tried to do
status_of_success BOOLEAN whether the activity was done or not. There is no pending!
```

# Note: 
- The connection string in `appsettings.json` is already configured, but you need to create a database called `VisionPanelMaster`.
- The whole idea of this backend is master-agent. The agents will obay the master and send information to the master, so the master can update the information and save it for the users.
- The communication between the master and agent is with AES encrypted JWT tokens.
- The master and the agent will have the keys for encryption and JWT verification.
- File: `StartupSql.sql` is the sql that will be executed every time the master server starts up. The path of it is in the `appsettings.json`.
- `Gmail_App_Password_For_Panel_Email` can be empty if you don't use Gmail.
- `Panel_Email` the email the panel will use to send messages to notify teh users.
- `Email_Smtp_Server` by default is `"smtp.gmail.com:587"` , 
   but you can change it if you want to use other services. Format: "Address:Port"