using System.Net.Mail;

namespace VisionPanelMaster.Utils {
    public class UtilManager {

        private WebApplication app;

        public Utils generalUtils;

        public UtilManager(WebApplication app, SmtpClient smtp) { 
            this.app = app;
            generalUtils = new Utils(smtp, GetValue("Panel_Email") ?? throw new Exception());
        }

        public string? GetValue(string key) {
            object? v = app.Configuration.GetValue<string>(key);
            return v == null ? throw new Exception("Invalid value for "+key) : v.ToString();
        }
    }
}
