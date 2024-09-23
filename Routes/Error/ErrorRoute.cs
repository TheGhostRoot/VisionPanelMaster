using VisionPanelMaster.Utils;
namespace VisionPanelMaster.Routes.Error {
    public class ErrorRoute {
        public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
            PagesUtils.RegisterPageAssets(app, WebLoginRoot, "/error", 
                WebLoginRoot + "/ErrorIndex.html");
        }

    }
}
