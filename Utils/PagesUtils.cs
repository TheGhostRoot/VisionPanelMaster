using Main;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

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


        public static void RegisterPageAssets(WebApplication app, string webrootPath, string endpointRoot, string mainIndexPath) {
            endpointRoot = endpointRoot == "/" ? "" : endpointRoot;
            app.MapGet(endpointRoot, async (HttpContext context) => 
            await context.Response.SendFileAsync(mainIndexPath));
            foreach (string path in GeneralUtils.GetFilesWithPaths(webrootPath)) {
                app.MapGet(endpointRoot + "/" + path, 
                    async (HttpContext context) =>
                await context.Response.SendFileAsync(webrootPath + "/" + path));
            }
        }
        
    }
}
