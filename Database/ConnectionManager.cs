using Npgsql;

namespace VisionPanelMaster.Database {
    public class ConnectionManager : IDisposable {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;

        public ConnectionManager(string connection_string) {
            _connectionString = connection_string;
        }

        public async void CheckConnection() {
            if (await GetConnectionAsync() == null) {
                throw new Exception("Connection Invalid");
            }
        }
        private async Task<NpgsqlConnection> GetConnectionAsync() {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open) {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
            return _connection;
        }

        public NpgsqlCommand BuildCommand(string sql, List<NpgsqlParameter>? args = null) {
            var command = new NpgsqlCommand(sql);
            if (args != null) {
                command.Parameters.AddRange(args.ToArray());
            }
            return command;
        }

        public async void ExecuteReaderAsync(NpgsqlCommand cmd, Action<NpgsqlDataReader> func) {
            using var connection = await GetConnectionAsync();
            cmd.Connection = connection;
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                func(reader);
            }
            reader.Close();
        }

        public async Task<int> ExecuteNonQueryAsync(NpgsqlCommand cmd) {
            using var connection = await GetConnectionAsync();
            cmd.Connection = connection;
            return await cmd.ExecuteNonQueryAsync();
        }

        public void Dispose() {
            _connection?.Dispose();
        }
    }
}