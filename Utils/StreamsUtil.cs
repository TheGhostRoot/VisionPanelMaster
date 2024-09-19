namespace VisionPanelMaster.Utils {
    public class StreamsUtil {

        public static async Task<string> ReadFullFile(string path) {
            using var reader = File.OpenText(path);
            return await reader.ReadToEndAsync();
        }

    }
}
