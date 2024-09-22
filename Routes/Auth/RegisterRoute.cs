
using Main;
using VisionPanelMaster.Utils;

namespace VisionPanelMaster.Routes.Auth {
    public class RegisterRoute {
        public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
            PagesUtils.RegisterPageAssets(app, WebLoginRoot, "/register", 
                WebLoginRoot + "/RegisterIndex.html");
        }


    }
}
