using VisionPanelMaster.Utils;

namespace VisionPanelMaster.Routes.Auth {
    public class VerifyEmailRoute {
        public static void RegisterPaths(WebApplication app, string WebLoginRoot) {
            Program.utilManager.generalUtils.RegisterPageAssets(app, WebLoginRoot, 
                "/verify_email", WebLoginRoot + "/VerifyEmailIndex.html");

            Program.utilManager.generalUtils.RegisterPageAssets(app, WebLoginRoot,
                "/verify_email_code", WebLoginRoot + "/VerifyEmailIndex.html");

        }
    }
}
