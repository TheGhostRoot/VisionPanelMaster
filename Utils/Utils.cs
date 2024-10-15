using Microsoft.Extensions.Hosting;
using Npgsql;
using NpgsqlTypes;
using System.Net;
using System.Net.Mail;
using VisionPanelMaster.Database;

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

        private void GetFilesRecursive(string currentFolderPath, 
            string relativePath, List<string> filePaths) {
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
            if (Program.utilManager == null) {
                throw new Exception("Program.utilManager is null");
            }

            string subject;
            string body;

            try {
                string configPath = Program.utilManager.GetValue("Verify_Email_File_Path");
                if (configPath == null || !configPath.EndsWith(".html")) {
                    throw new Exception("Invalid email template");
                }
                string emailPath;
                if (configPath.Contains("\\")) {
                    emailPath = configPath.Split("\\").Last();

                } else if (configPath.Contains("/")) {
                    emailPath = configPath.Split("/").Last();

                } else {
                    emailPath = configPath;
                }
                subject = emailPath.Remove(emailPath.Length - 5, 5);
                body = File.ReadAllText(configPath);
            } catch (Exception e) {
                throw new Exception("Can't get Verify_Email_File_Path to work. "+e.Message);
            }


            List<int> codes = Program.databaseMain.emailDatabase.GetAllCodes(
                      new ConnectionManager(Program.databaseMain.ConnectionString));

            int code = Program.random.Next(99999, 10000000);

            while (codes.Contains(code)) {
                code = Program.random.Next(99999, 10000000);
            }

            body = body.Replace("/verify_code/", code.ToString());
            MailMessage emailMsg = new MailMessage(panel_email, email,
                subject, body);
            emailMsg.IsBodyHtml = true;


            try {
                smtp.Send(emailMsg);
                Program.databaseMain.emailDatabase.AddVerifyCode(
                    new ConnectionManager(Program.databaseMain.ConnectionString), 
                    code, email);
            } catch (SmtpException e) {
                throw new Exception("Can't send email");
            }
        }
    }
}
