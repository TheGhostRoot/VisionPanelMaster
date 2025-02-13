
namespace VisionPanelMaster;

using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using VisionPanelMaster.Database;
using VisionPanelMaster.Routes.Auth;
using VisionPanelMaster.Routes.Error;
using VisionPanelMaster.Routes.ToS;
using VisionPanelMaster.Utils;

class Program {
    public static string WebRoot = "";

    public static HashAlgorithm sha = SHA256.Create();
    public static Random random = new Random();

    public static DatabaseMain databaseMain {
        get; set;
    }
    public static UtilManager utilManager {
        get; set;
    }

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".VisionPanel.Session";
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        WebApplication app = builder.Build();
        app.UseHttpsRedirection();
        app.UseSession();


        WebRoot = app.Configuration.GetValue<string>("Web_Root_Folder") ?? "";

        SetupSMTP(app);
        SetupDatabase(app);

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");

            Console.WriteLine("Method: "+context.Request.Method +" " + 
                context.Request.Host +" Path:"+ 
                context.Request.Path + " ContentType:" + context.Request.ContentType);
            await next();
            Console.WriteLine($"Response: {context.Response.StatusCode}");
        });


        /*
         * <head>
         * <meta http-equiv="Content-Security-Policy" 
      content="base-uri 'self';
               default-src 'self';
               img-src data: https:;
               object-src 'none';
               script-src 'self';
               style-src 'self';
               upgrade-insecure-requests;">
         * </head>
         */



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
        utilManager.smtp.Dispose();
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
            Program.utilManager = new UtilManager(app, smtp);
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

CREATE TABLE IF NOT EXISTS VerifyEmail (
    code INTEGER PRIMARY KEY NOT NULL,
    datetime TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    email TEXT UNIQUE NOT NULL
);

";


        Program.databaseMain = new DatabaseMain(app);

        try {
            sql = File.ReadAllText(app.Configuration.GetValue<string>("Startup_SQL_File_Path")
            ?? throw new Exception("`Startup_SQL_File_Path` is not configured."));
        } catch (Exception e) {
            Console.WriteLine("Error while reading the startup sql: " + e.Message);
        }

        ConnectionManager connection = new ConnectionManager(Program.databaseMain.ConnectionString);
        connection.ExecuteNonQueryAsync(connection.BuildCommand(sql, null)).Wait();
    }
}
