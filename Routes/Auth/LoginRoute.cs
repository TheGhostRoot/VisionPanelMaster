namespace VisionPanelMaster.Routes.Auth;
using Main;
using System.IO;
using VisionPanelMaster.Utils;

public class LoginRoute {
    private static WebApplication app = Program.app;
    private static string WebLoginRoot = Program.WebRoot+"/Auth/Login";

    public static void RegisterPaths() {
        PagesUtils.RegisterPageAssets(app, WebLoginRoot, "/", WebLoginRoot+"/LoginIndex.html");
        PagesUtils.RegisterPageAssets(app, WebLoginRoot, "/login", WebLoginRoot + "/LoginIndex.html");

    }
    // HttpContext context

}

