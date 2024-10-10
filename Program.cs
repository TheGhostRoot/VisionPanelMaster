
namespace VisionPanelMaster;

using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using VisionPanelMaster.Database;
using VisionPanelMaster.Routes.Auth;
using VisionPanelMaster.Routes.Error;
using VisionPanelMaster.Routes.ToS;

class Program {
    public static WebApplication? app;
    public static string WebRoot = "";
    public readonly static string PageContentType = "text/html";

    public static HashAlgorithm sha = SHA256.Create();

    private static DatabaseMain? databaseMain;

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        app = builder.Build();
        WebRoot = app.Configuration.GetValue<string>("Web_Root_Folder", "/Web") 
            ?? throw new ArgumentNullException(nameof(app), "`Web_Root_Folder` is not configured.");

        DatabaseMain databaseMain= new DatabaseMain(app);


        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        string sql = @"
CREATE TABLE IF NOT EXISTS Users (
    user_id BIGSERIAL PRIMARY KEY,
    username VARCHAR(30) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Servers (
    server_id BIGSERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    enable_machine_logs BOOLEAN NOT NULL DEFAULT TRUE,
    enable_user_logs BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS MachineLogs (
    server_id BIGINT REFERENCES Servers(server_id),
    datetime TIMESTAMP NOT NULL DEFAULT NOW(),
    cpu_load TEXT,
    ram_used_mb BIGINT NOT NULL,
    disk_operations_read_and_write_per_second BIGINT NOT NULL,
    network_inbound_mb_per_second BIGINT NOT NULL,
    network_outbound_mb_per_second BIGINT NOT NULL,
    gpu_load BIGINT
);

CREATE TABLE IF NOT EXISTS UserLogs (
    server_id BIGINT REFERENCES Servers(server_id),
    datetime TIMESTAMP NOT NULL DEFAULT NOW(),
    activity TEXT NOT NULL,
    status_of_success BOOLEAN NOT NULL
);

";
        try {
            sql = File.ReadAllText(app.Configuration.GetValue<string>("Startup_SQL_File_Path", "/StartupSql.sql")
            ?? throw new ArgumentNullException(nameof(app), "`Startup_SQL_File_Path` is not configured."));
        } catch (Exception e) {
            Console.WriteLine("Error while reading the startup sql: "+e.Message);
            return;
        }

        databaseMain.databaseManager.ExecuteNonQueryAsync(
            databaseMain.databaseManager.BuildCommand(sql, null)).Wait();

        /*

        List<NpgsqlParameter> sqlArgs = new List<NpgsqlParameter>();
        NpgsqlParameter p = new NpgsqlParameter("@Number", NpgsqlDbType.Integer);
        p.Value = 1;
        sqlArgs.Add(p);

        Action<NpgsqlDataReader> action = (NpgsqlDataReader reader2) => {
            Console.WriteLine(reader2.GetValue(0));
        };

        databaseManager.ExecuteReaderAsync(
            databaseManager.BuildCommand("SELECT @Number;", sqlArgs), action);*/

        LoginRoute.RegisterPaths(app, WebRoot + "/Auth/Login");
        ToSRoute.RegisterPaths(app, WebRoot + "/ToS");
        ErrorRoute.RegisterPaths(app, WebRoot + "/Error");
        VerifyEmailRoute.RegisterPaths(app, WebRoot + "/Auth/VerifyEmail");


        app.Run();
   }


    private static void OnProcessExit(object? sender, EventArgs e) {
        databaseMain?.databaseManager.Dispose();
    }
}
