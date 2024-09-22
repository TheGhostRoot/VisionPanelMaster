
namespace Main;

using VisionPanelMaster.Routes.Auth;
using VisionPanelMaster.Routes.ToS;

class Program {
    public static WebApplication app;
    public static string WebRoot;
    public static string PageContentType = "text/html";

     public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        app = builder.Build();
        WebRoot = app.Configuration.GetValue("root_folder", "/Web");

        LoginRoute.RegisterPaths(app, WebRoot + "/Auth/Login");
        RegisterRoute.RegisterPaths(app, WebRoot + "/Auth/Login");
        ToSRoute.RegisterPaths(app, WebRoot + "/ToS");


        app.Run();
   }
}
