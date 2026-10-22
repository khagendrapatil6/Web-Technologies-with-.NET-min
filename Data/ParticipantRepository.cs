using System.Data;
using MySqlConnector;

namespace CollegeFestMVC.Data;

public enum AddResult { Success, DuplicateEmail, EventFull, InvalidEvent }

/// <summary>All MySQL access for participants (parameterised queries only).</summary>
public class ParticipantRepository
{
    private readonly string _cs;

    public ParticipantRepository(IConfiguration config)
    {
        _cs = config.GetConnectionString("FestDb")
              ?? throw new InvalidOperationException("Connection string 'FestDb' is missing in appsettings.json");
    }

    private const string SelectSql = @"
        SELECT p.participant_id, p.participant_name, p.email, p.department, p.academic_year,
               e.event_name, p.is_team_event, p.registration_fee, p.is_completed, p.registered_on
        FROM participants p
        JOIN events e ON e.event_id = p.event_id";

    private static Participant Map(MySqlDataReader r) => new()
    {
        ParticipantId = r.GetInt32("participant_id"),
        ParticipantName = r.GetString("participant_name"),
        Email = r.GetString("email"),
        Department = r.GetString("department"),
        Year = r.GetInt32("academic_year"),
        EventName = r.GetString("event_name"),
        IsTeamEvent = r.GetBoolean("is_team_event"),
        RegistrationFee = r.GetDecimal("registration_fee"),
        IsRegistrationCompleted = r.GetBoolean("is_completed"),
        RegisteredOn = r.GetDateTime("registered_on")
    };

    public async Task<List<Participant>> GetAllAsync()
    {
        var list = new List<Participant>();
        await using var conn = new MySqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(SelectSql + " ORDER BY p.participant_id", conn);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(Map(r));
        return list;
    }

    public async Task<Participant?> GetByIdAsync(int id)
    {
        await using var conn = new MySqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(SelectSql + " WHERE p.participant_id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? Map(r) : null;
    }

    /// <summary>Checks duplicate + capacity and inserts inside one transaction (row-locks the event).</summary>
    public async Task<(AddResult Result, int Id)> AddAsync(Participant p, Event ev)
    {
        await using var conn = new MySqlConnection(_cs);
        await conn.OpenAsync();
        await using var tx = await conn.BeginTransactionAsync();

        // Lock the event row so two people can't take the last seat at the same time.
        await using (var lockCmd = new MySqlCommand(
            "SELECT event_id FROM events WHERE event_id = @eid FOR UPDATE", conn, tx))
        {
            lockCmd.Parameters.AddWithValue("@eid", ev.EventId);
            if (await lockCmd.ExecuteScalarAsync() is null) return (AddResult.InvalidEvent, 0);
        }

        await using (var dup = new MySqlCommand(
            "SELECT COUNT(*) FROM participants WHERE event_id = @eid AND LOWER(email) = LOWER(@email)", conn, tx))
        {
            dup.Parameters.AddWithValue("@eid", ev.EventId);
            dup.Parameters.AddWithValue("@email", p.Email);
            if (Convert.ToInt32(await dup.ExecuteScalarAsync()) > 0) return (AddResult.DuplicateEmail, 0);
        }

        await using (var cnt = new MySqlCommand(
            "SELECT COUNT(*) FROM participants WHERE event_id = @eid", conn, tx))
        {
            cnt.Parameters.AddWithValue("@eid", ev.EventId);
            if (Convert.ToInt32(await cnt.ExecuteScalarAsync()) >= ev.MaximumParticipants)
                return (AddResult.EventFull, 0);
        }

        await using var ins = new MySqlCommand(@"
            INSERT INTO participants
              (participant_name, email, department, academic_year, event_id, is_team_event,
               registration_fee, is_completed, registered_on)
            VALUES (@name, @email, @dept, @year, @eid, @team, @fee, @done, @on);
            SELECT LAST_INSERT_ID();", conn, tx);
        ins.Parameters.AddWithValue("@name", p.ParticipantName);
        ins.Parameters.AddWithValue("@email", p.Email);
        ins.Parameters.AddWithValue("@dept", p.Department);
        ins.Parameters.AddWithValue("@year", p.Year);
        ins.Parameters.AddWithValue("@eid", ev.EventId);
        ins.Parameters.AddWithValue("@team", p.IsTeamEvent);
        ins.Parameters.AddWithValue("@fee", ev.RegistrationFee);
        ins.Parameters.AddWithValue("@done", ev.RegistrationFee == 0);
        ins.Parameters.AddWithValue("@on", DateTime.Now);
        var newId = Convert.ToInt32(await ins.ExecuteScalarAsync());

        await tx.CommitAsync();
        return (AddResult.Success, newId);
    }

    /// <summary>Marks the registration as paid/confirmed. Returns false if the id doesn't exist.</summary>
    public async Task<bool> ConfirmAsync(int id)
    {
        await using var conn = new MySqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(
            "UPDATE participants SET is_completed = 1 WHERE participant_id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();

        await using var chk = new MySqlCommand("SELECT COUNT(*) FROM participants WHERE participant_id = @id", conn);
        chk.Parameters.AddWithValue("@id", id);
        return Convert.ToInt32(await chk.ExecuteScalarAsync()) > 0;
    }
}
