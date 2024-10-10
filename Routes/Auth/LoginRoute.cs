namespace VisionPanelMaster.Routes.Auth;
using VisionPanelMaster.Utils;

public class LoginRoute {
    public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
        PagesUtils.RegisterPageAssets(app, WebLoginRoot, "", WebLoginRoot+"/LoginIndex.html");
        PagesUtils.RegisterPageAssets(app, WebLoginRoot, "/login", WebLoginRoot + "/LoginIndex.html");
        onLogin(app, "/login");

    }
    // HttpContext context

    private static void onLogin(WebApplication app, string endpoint) {
        app.MapPost(endpoint, () => { });
    }

}

