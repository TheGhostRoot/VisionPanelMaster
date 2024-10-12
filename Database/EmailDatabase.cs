using Npgsql;
using NpgsqlTypes;

namespace VisionPanelMaster.Database {
    public class EmailDatabase {
        private Thread VerifyEmailExpiration;


        public EmailDatabase() {
            VerifyEmailExpiration = new Thread(HandleCodeExpiration);
            VerifyEmailExpiration.IsBackground = true;
            VerifyEmailExpiration.Name = "CodeExpiration";
            VerifyEmailExpiration.Start();
        }

        private async void HandleCodeExpiration() {
            while (true) {
                ConnectionManager connection = new ConnectionManager(Program.databaseMain.ConnectionString);
                int rows = await connection.ExecuteNonQueryAsync(
                    connection.BuildCommand(@"DELETE FROM VerifyEmail
                WHERE datetime + INTERVAL '5 minutes' <= CURRENT_TIMESTAMP;", null));
                Thread.Sleep(86400000); // 1 day
            }
        }

        public async void AddVerifyCode(ConnectionManager connection, int code, string email) {
            NpgsqlParameter codeArg = new NpgsqlParameter("@Code", NpgsqlDbType.Integer);
            codeArg.Value = code;

            NpgsqlParameter emailArg = new NpgsqlParameter("@Email", NpgsqlDbType.Text);
            emailArg.Value = email;

            List<NpgsqlParameter> args = new List<NpgsqlParameter>();
            args.Add(emailArg);
            args.Add(codeArg);

            int rows = await connection.ExecuteNonQueryAsync(
                connection.BuildCommand(@"INSERT INTO VerifyEmail (code,email)
                                        VALUES (@Code,@Email);", args));

            if (rows == 0) {
                throw new Exception("Pending verifycation");
            }
        }


        public List<int> GetAllCodes(ConnectionManager connection) {
            List<int> codes = new List<int>();
            connection.ExecuteReaderAsync(
                connection.BuildCommand(@"SELECT code FROM VerifyEmail;", null), 
                (NpgsqlDataReader reader) => {
                    codes.Add(int.Parse(reader["code"].ToString()));
                });
            return codes;
        }
    }
}
