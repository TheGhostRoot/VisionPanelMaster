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
- They are all in `/Routes` folder
- We use Minimal API for easier endpoints and utility function that
 automaticly adds endpoints for all of the webpage assets as folders and files
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