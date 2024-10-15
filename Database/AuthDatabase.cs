using Npgsql;
using NpgsqlTypes;
using System.Text;

namespace VisionPanelMaster.Database {
    public class AuthDatabase {
        public AuthDatabase() {
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

            ConnectionManager connection = new ConnectionManager(Program.databaseMain.ConnectionString);

            connection.ExecuteReaderAsync(
                connection.BuildCommand(@"SELECT exists 
(SELECT 1 as Login FROM Users WHERE email = @Email AND password = @Password LIMIT 1);", args),
                (NpgsqlDataReader reader) => {
                    if (!bool.Parse(reader.GetValue(0)?.ToString())) {
                        throw new Exception("Invalid login email and password");
                    } else {
                        return;
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

            ConnectionManager connection = new ConnectionManager(Program.databaseMain.ConnectionString);

            int effected_rows = await connection.ExecuteNonQueryAsync(
                connection.BuildCommand(@"INSERT INTO Users (username,email,password) 
VALUES (@Username, @Email, @Password);", args));


            if (effected_rows == 0) {
                throw new Exception("Ether the username exists, the email exists or you have given too long inputs.");
            }
        }


        public async void ChangePassword(string email, string newPassword) {
            string hashed_password =
                Encoding.ASCII.GetString(
                    Program.sha.ComputeHash(Encoding.ASCII.GetBytes(newPassword)));

            NpgsqlParameter emailArg = new NpgsqlParameter("@Email", NpgsqlDbType.Text);
            emailArg.Value = email;

            NpgsqlParameter passwordArg = new NpgsqlParameter("@Password", NpgsqlDbType.Text);
            passwordArg.Value = hashed_password;


            List<NpgsqlParameter> args = new List<NpgsqlParameter>();
            args.Add(emailArg);
            args.Add(passwordArg);

            ConnectionManager connection = new ConnectionManager(Program.databaseMain.ConnectionString);

            int effected_rows = await connection.ExecuteNonQueryAsync(
                connection.BuildCommand(@"UPDATE Users SET password = @Password WHERE email = @Email;",
                args));


            if (effected_rows == 0) {
                throw new Exception("Couldn't change the password or the password is the same.");
            }
        }
    }
}
