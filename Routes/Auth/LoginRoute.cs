namespace VisionPanelMaster.Routes.Auth;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VisionPanelMaster.Utils;


public class LoginRoute {
    public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
        Program.utilManager.generalUtils.RegisterPageAssets(app, WebLoginRoot, "", WebLoginRoot+"/LoginIndex.html");
        Program.utilManager.generalUtils.RegisterPageAssets(app, WebLoginRoot, "/login", WebLoginRoot + "/LoginIndex.html");
        onLogin(app, "/login");

    }
    // HttpContext context

    private static void onLogin(WebApplication app, string endpoint) {
        app.MapPost(endpoint, async (HttpContext context) => {
            // context.Session.SetString
            try {
                string s = context.Session.GetString("test");
                Console.WriteLine(s);
            } catch (Exception e) { 
            }
            context.Session.SetString("test", "1");
            try {
                var form = await context.Request.ReadFormAsync();
                Program.databaseMain.authDatabase.Login(form["email"].ToString(), 
                        form["password"].ToString());
                return Results.Accepted();
            } catch (Exception ex) {
                return Results.Unauthorized();
            }
        });
    }

}

