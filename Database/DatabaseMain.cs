namespace VisionPanelMaster.Database {
    public class DatabaseMain {
        public DatabaseManager databaseManager {
            get;
        }

        public AuthDatabase authDatabase {
            get;
        }
        public DatabaseMain(WebApplication app) {
            databaseManager = new DatabaseManager(app);
            authDatabase = new AuthDatabase(databaseManager);
        }
    }
}
