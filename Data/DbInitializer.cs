using MySqlConnector;

namespace CollegeFestMVC.Data;

/// <summary>Creates the database, tables and seeds the events on application start.</summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(string connectionString)
    {
        var csb = new MySqlConnectionStringBuilder(connectionString);
        var dbName = csb.Database;
        csb.Database = "";   // connect to the server first, the database may not exist yet

        await using (var server = new MySqlConnection(csb.ConnectionString))
        {
            await server.OpenAsync();
            await using var create = new MySqlCommand(
                $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", server);
            await create.ExecuteNonQueryAsync();
        }

        await using var conn = new MySqlConnection(connectionString);
        await conn.OpenAsync();

        const string tables = @"
            CREATE TABLE IF NOT EXISTS events (
                event_id INT NOT NULL PRIMARY KEY,
                event_name VARCHAR(80) NOT NULL UNIQUE,
                maximum_participants INT NOT NULL,
                registration_fee DECIMAL(10,2) NOT NULL DEFAULT 0
            ) ENGINE=InnoDB;

            CREATE TABLE IF NOT EXISTS participants (
                participant_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                participant_name VARCHAR(80) NOT NULL,
                email VARCHAR(120) NOT NULL,
                department VARCHAR(80) NOT NULL,
                academic_year TINYINT NOT NULL,
                event_id INT NOT NULL,
                is_team_event TINYINT(1) NOT NULL DEFAULT 0,
                registration_fee DECIMAL(10,2) NOT NULL,
                is_completed TINYINT(1) NOT NULL DEFAULT 0,
                registered_on DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT fk_participant_event FOREIGN KEY (event_id) REFERENCES events(event_id),
                CONSTRAINT uq_event_email UNIQUE (event_id, email)
            ) ENGINE=InnoDB AUTO_INCREMENT=1001;";
        await using (var cmd = new MySqlCommand(tables, conn))
            await cmd.ExecuteNonQueryAsync();

        // Seed / refresh events from EventCatalog (safe to run every start).
        foreach (var e in EventCatalog.All)
        {
            await using var upsert = new MySqlCommand(@"
                INSERT INTO events (event_id, event_name, maximum_participants, registration_fee)
                VALUES (@id, @name, @max, @fee)
                ON DUPLICATE KEY UPDATE event_name = VALUES(event_name),
                    maximum_participants = VALUES(maximum_participants),
                    registration_fee = VALUES(registration_fee);", conn);
            upsert.Parameters.AddWithValue("@id", e.EventId);
            upsert.Parameters.AddWithValue("@name", e.EventName);
            upsert.Parameters.AddWithValue("@max", e.MaximumParticipants);
            upsert.Parameters.AddWithValue("@fee", e.RegistrationFee);
            await upsert.ExecuteNonQueryAsync();
        }
    }
}
