using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Mail;

namespace VisionPanelMaster.Utils {
    public class Utils {
        // 465
        // 587

        private readonly SmtpClient smtp;
        private readonly string panel_email;

        public Utils(SmtpClient smtp, string panel_email) {
            this.smtp = smtp;
            this.panel_email = panel_email;
        }

        public List<string> GetFilesWithPaths(string folderPath) {
            List<string> filePaths = new List<string>();
            GetFilesRecursive(folderPath, "", filePaths);
            return filePaths;
        }

        private void GetFilesRecursive(string currentFolderPath, string relativePath, List<string> filePaths) {
            string[] files = Directory.GetFiles(currentFolderPath);
            foreach (string file in files) {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".js") || fileName.EndsWith(".css") || 
                    fileName.EndsWith(".html") || fileName.EndsWith(".mp4") || 
                    fileName.EndsWith(".mp3") || fileName.EndsWith(".png") || 
                    fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpg") || 
                    fileName.EndsWith(".ogg") || fileName.EndsWith(".mov")) {
                    string path = Path.Combine(relativePath, fileName).Replace("\\", "/");
                    if (!filePaths.Contains(path)) {
                        filePaths.Add(path);
                    }
                    
                }
                
            }

            string[] subfolders = Directory.GetDirectories(currentFolderPath);
            foreach (string subfolder in subfolders) {
                string subfolderName = Path.GetFileName(subfolder);
                GetFilesRecursive(subfolder, Path.Combine(relativePath, subfolderName), filePaths);
            }
        }

        public string LoadPage(string path) {
            try {
                return File.ReadAllText(path);
            } catch (Exception e) {
                return "<script type=\"text/javascript\">\r\n      window.location.href = \"/error\";\r\n   </script>";
            }
        }


        public void RegisterPageAssets(WebApplication app, string webrootPath,
            string endpointRoot, string mainIndexPath) {
            app.MapGet(endpointRoot, async (HttpContext context) =>
            await context.Response.WriteAsync(LoadPage(mainIndexPath)));

            foreach (string path in GetFilesWithPaths(webrootPath)) {
                app.MapGet(endpointRoot + "/" + path,
                    async (HttpContext context) =>
                await context.Response.SendFileAsync(webrootPath + "/" + path));
            }
        }

        public void sendEmailToVerifyCode(string email) {
            string code = Program.random.Next(99999, 10000000).ToString();
            MailMessage emailMsg = new MailMessage(panel_email, email, 
                "aaaa", 
                "Hello");
            emailMsg.IsBodyHtml = true;

            smtp.Send(emailMsg);

        }
    }
}
