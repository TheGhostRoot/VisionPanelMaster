using Main;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace VisionPanelMaster.Utils {
    public class PagesUtils {

        public static async Task<string> GetRequestHTML(HttpContext context) {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            return body;
        }

        public static string LoadPage(string path) { 
            try {
                return File.ReadAllText(path);
            } catch (Exception e) {
                return "<script type=\"text/javascript\">\r\n      window.location.href = \"/error\";\r\n   </script>";
            }
        }


        public static void RegisterPageAssets(WebApplication app, string webrootPath, 
            string endpointRoot, string mainIndexPath) {
            app.MapGet(endpointRoot, async (HttpContext context) =>
            await context.Response.WriteAsync(LoadPage(mainIndexPath))); 

            foreach (string path in GeneralUtils.GetFilesWithPaths(webrootPath)) {
                app.MapGet(endpointRoot + "/"+path, 
                    async (HttpContext context) =>
                await context.Response.SendFileAsync(webrootPath + "/" + path));
            }
        }
        
    }
}
