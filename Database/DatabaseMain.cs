using Npgsql;

namespace VisionPanelMaster.Database {
    public class DatabaseMain {

        public string ConnectionString { get; }

        public AuthDatabase authDatabase {
            get;
        }

        public EmailDatabase emailDatabase {
            get;
        }


        public DatabaseMain(WebApplication app) {
            ConnectionString = app.Configuration.GetValue<string>("Connection_Strings") ??
                throw new Exception("No connection string");

            new ConnectionManager(ConnectionString).CheckConnection();
            
            authDatabase = new AuthDatabase();
            emailDatabase = new EmailDatabase();
        }
    }
}
