using VisionPanelMaster.Utils;

namespace VisionPanelMaster.Routes.ToS {
    public class ToSRoute {

        public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
            PagesUtils.RegisterPageAssets(app, WebLoginRoot, "/tos", WebLoginRoot + "/ToSIndex.html");
        }
    }
}
