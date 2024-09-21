
using Main;

namespace VisionPanelMaster.Routes.Auth {
    public class RegisterRoute {
        private static WebApplication app = Program.app;

        private static string WebLoginRoot = Program.WebRoot + "/Auth/Register";

        public static void RegisterPaths() {
            app.MapGet("/register", (HttpContext context) => Register(context));
        }

        public static IResult Register(HttpContext context) {
            return Results.Content(File.ReadAllText(WebLoginRoot + "/RegisterIndex.html"),
                Program.PageContentType);
        }

    }
}
