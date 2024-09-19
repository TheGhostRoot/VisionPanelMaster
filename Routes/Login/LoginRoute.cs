namespace VisionPanelMaster.Routes.Login;
using Main;
using System.IO;
using VisionPanelMaster.Utils;

public class LoginRoute {
    private static WebApplication app = Program.app;

    private static string WebLoginRoot = Program.WebRoot+"/Login";

    public static void RegisterPaths() {
        app.MapGet("/", async () =>  await Login());
        app.MapGet("/login", async  () => await Login());
    }

    public static async Task<IResult> Login() {
        string loginPage = await StreamsUtil.ReadFullFile(WebLoginRoot + "/LoginIndex.html");
        return Results.Content(loginPage, "text/html"); ;
    }

}

