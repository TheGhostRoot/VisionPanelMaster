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
