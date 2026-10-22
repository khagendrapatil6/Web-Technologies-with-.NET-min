namespace CollegeFestMVC.Models;

/// <summary>In-memory list of technical events (no database needed).</summary>
public static class EventCatalog
{
    public static readonly IReadOnlyList<Event> All = new List<Event>
    {
        new() { EventId = 1, EventName = "Code Sprint",          MaximumParticipants = 60,  RegistrationFee = 250,  Icon = "bi-code-slash",   Description = "Solve timed algorithmic problems against the clock." },
        new() { EventId = 2, EventName = "Debug Arena",          MaximumParticipants = 40,  RegistrationFee = 0,    Icon = "bi-bug",          Description = "Find and fix bugs in unfamiliar code. Free entry." },
        new() { EventId = 3, EventName = "Web Design Challenge", MaximumParticipants = 50,  RegistrationFee = 500,  Icon = "bi-window-stack", Description = "Build a responsive site from a brief in three hours." },
        new() { EventId = 4, EventName = "AI Hackathon",         MaximumParticipants = 80,  RegistrationFee = 750,  Icon = "bi-robot",        Description = "Prototype an AI-powered solution in a 12-hour build." },
        new() { EventId = 5, EventName = "Robo Race",            MaximumParticipants = 30,  RegistrationFee = 1200, Icon = "bi-gear-wide-connected", Description = "Bring your line-following robot and race for the podium." },
        new() { EventId = 6, EventName = "Tech Quiz",            MaximumParticipants = 100, RegistrationFee = 0,    Icon = "bi-lightbulb",    Description = "Rapid-fire questions on computing, electronics and gadgets." },
    };

    public static Event? Find(string? name) =>
        All.FirstOrDefault(e => e.EventName.Equals(name, StringComparison.OrdinalIgnoreCase));
}
