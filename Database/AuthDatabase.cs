using Npgsql;
using NpgsqlTypes;
using System.Text;

namespace VisionPanelMaster.Database {
    public class AuthDatabase {

        private readonly DatabaseManager databaseManager;
        public AuthDatabase(DatabaseManager givenDatabaseManager) {
            databaseManager = givenDatabaseManager;
        }


        public void Login(string email, string password) {
            string hashed_password =
                Encoding.ASCII.GetString(
                    Program.sha.ComputeHash(Encoding.ASCII.GetBytes(password)));

            NpgsqlParameter passwordArg = new NpgsqlParameter("@Password", NpgsqlDbType.Text);
            passwordArg.Value = hashed_password;

            NpgsqlParameter emailArg = new NpgsqlParameter("@Email", NpgsqlDbType.Text);
            emailArg.Value = email;

            List<NpgsqlParameter> args = new List<NpgsqlParameter>();
            args.Add(emailArg);
            args.Add(passwordArg);

            databaseManager.ExecuteReaderAsync(
                databaseManager.BuildCommand(@"SELECT exists 
(SELECT 1 as Login FROM Users WHERE email = @Email AND password = @Password LIMIT 1);", args), 
                (NpgsqlDataReader reader) => {
                    if (bool.Parse(reader.GetValue(0)?.ToString())) {
                        throw new Exception("Success");
                    } else {
                        throw new Exception("Invalid login email and password");
                    }
                });
        }


        public async void Register(string username, string email, string password) {
            string hashed_password = 
                Encoding.ASCII.GetString(
                    Program.sha.ComputeHash(Encoding.ASCII.GetBytes(password)));

            NpgsqlParameter usernameArg = new NpgsqlParameter("@Username", NpgsqlDbType.Text);
            usernameArg.Value = username;

            NpgsqlParameter emailArg = new NpgsqlParameter("@Email", NpgsqlDbType.Text);
            emailArg.Value = email;

            NpgsqlParameter passwordArg = new NpgsqlParameter("@Password", NpgsqlDbType.Text);
            passwordArg.Value = hashed_password;


            List<NpgsqlParameter> args = new List<NpgsqlParameter>();
            args.Add(usernameArg);
            args.Add(emailArg);
            args.Add(passwordArg);

            int effected_rows = await databaseManager.ExecuteNonQueryAsync(
                databaseManager.BuildCommand(@"INSERT INTO Users (username,email,password) 
VALUES (@Username, @Email, @Password);", args));

            if (effected_rows == 0) {
                throw new Exception("Ether the username exists, the email exists or you have given too long inputs.");
            }
        }
    }
}
