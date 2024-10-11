using VisionPanelMaster.Utils;

namespace VisionPanelMaster.Routes.ToS {
    public class ToSRoute {

        public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
            Program.utilManager.generalUtils.RegisterPageAssets(app, WebLoginRoot, "/tos", WebLoginRoot + "/ToSIndex.html");
        }
    }
}
