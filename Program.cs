
namespace VisionPanelMaster;

using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Net;
using System.Net.Mail;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using VisionPanelMaster.Database;
using VisionPanelMaster.Routes.Auth;
using VisionPanelMaster.Routes.Error;
using VisionPanelMaster.Routes.ToS;
using VisionPanelMaster.Utils;

class Program {
    public static string WebRoot = "";
    public readonly static string PageContentType = "text/html";

    public static HashAlgorithm sha = SHA256.Create();
    public static Random random = new Random();

    private static DatabaseMain? databaseMain;
    private static UtilManager? utilManager;

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        WebApplication app = builder.Build();

        WebRoot = app.Configuration.GetValue<string>("Web_Root_Folder") ?? "";

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        SetupSMTP(app);
        SetupDatabase(app);



      


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

    private static void SetupSMTP(WebApplication app) {
        string? smtpServer = app.Configuration.GetValue<string>("Email_Smtp_Server");
        try {
            string[] parts = smtpServer.Split(":");

            SmtpClient smtp = new SmtpClient(parts[0], int.Parse(parts[1]));
            smtp.Credentials = new NetworkCredential(
                app.Configuration.GetValue<string>("Panel_Email") ?? 
                throw new Exception(),
                app.Configuration.GetValue<string>("Gmail_App_Password_For_Panel_Email") ?? 
                throw new Exception());

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            utilManager = new UtilManager(app, smtp);
        } catch (Exception ex) {
            throw new Exception("You need SMTP Server configured");
        }
    }

    private static void SetupDatabase(WebApplication app) {
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
        databaseMain = new DatabaseMain(app);
        try {
            sql = File.ReadAllText(app.Configuration.GetValue<string>("Startup_SQL_File_Path")
            ?? throw new Exception("`Startup_SQL_File_Path` is not configured."));
        } catch (Exception e) {
            Console.WriteLine("Error while reading the startup sql: " + e.Message);
            return;
        }

        databaseMain.databaseManager.ExecuteNonQueryAsync(
            databaseMain.databaseManager.BuildCommand(sql, null)).Wait();
    }
}
