using VisionPanelMaster.Utils;

namespace VisionPanelMaster.Routes.Auth {
    public class VerifyEmailRoute {
        public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
            PagesUtils.RegisterPageAssets(app, WebLoginRoot, 
                "/verify_email", WebLoginRoot + "/VerifyEmailIndex.html");

            PagesUtils.RegisterPageAssets(app, WebLoginRoot,
                "/verify_email_code", WebLoginRoot + "/VerifyEmailIndex.html");

        }
    }
}
