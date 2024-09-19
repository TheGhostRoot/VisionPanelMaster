
namespace Main;

using Microsoft.AspNetCore;
using VisionPanelMaster.Routes.Login;

class Program {
    public static WebApplication app;
    public static string WebRoot;

     public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        app = builder.Build();
        WebRoot = app.Configuration.GetValue("root_folder", "/Web");

        LoginRoute.RegisterPaths();
        
        app.Run();
   }
}
