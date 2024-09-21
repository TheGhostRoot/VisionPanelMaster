namespace VisionPanelMaster.Utils {
    public class GeneralUtils {

        public static List<string> GetFilesWithPaths(string folderPath) {
            List<string> filePaths = new List<string>();
            GetFilesRecursive(folderPath, "", filePaths);
            return filePaths;
        }

        private static void GetFilesRecursive(string currentFolderPath, string relativePath, List<string> filePaths) {
            string[] files = Directory.GetFiles(currentFolderPath);
            foreach (string file in files) {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".js") || fileName.EndsWith(".css") || 
                    fileName.EndsWith(".html") || fileName.EndsWith(".mp4") || 
                    fileName.EndsWith(".mp3") || fileName.EndsWith(".png") || 
                    fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpg") || 
                    fileName.EndsWith(".ogg") || fileName.EndsWith(".mov")) {
                    filePaths.Add(Path.Combine(relativePath, fileName).Replace("\\", "/"));
                }
                
            }

            string[] subfolders = Directory.GetDirectories(currentFolderPath);
            foreach (string subfolder in subfolders) {
                string subfolderName = Path.GetFileName(subfolder);
                GetFilesRecursive(subfolder, Path.Combine(relativePath, subfolderName), filePaths);
            }
        }

    }
}
