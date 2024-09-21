
namespace Main;

using Microsoft.AspNetCore;
using Microsoft.Extensions.FileProviders;
using VisionPanelMaster.Routes.Auth;

class Program {
    public static WebApplication app;
    public static string WebRoot;
    public static string PageContentType = "text/html";

     public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        app = builder.Build();
        WebRoot = app.Configuration.GetValue("root_folder", "/Web");

        LoginRoute.RegisterPaths();
        RegisterRoute.RegisterPaths();


        app.Run();
   }
}
