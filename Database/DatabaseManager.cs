using Npgsql;

namespace VisionPanelMaster.Database {
    public class DatabaseManager : IDisposable {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;

        public DatabaseManager(WebApplication app) {
            _connectionString = app.Configuration.GetValue<string>("Connection_Strings")
                ?? throw new ArgumentNullException(nameof(app), "`Connection_Strings` is not configured.");
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