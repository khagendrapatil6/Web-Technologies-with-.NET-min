namespace CollegeFestMVC.Models;

public class Event
{
    public int EventId { get; set; }

    [Required, StringLength(80)]
    public string EventName { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Maximum participants must be between 1 and 1000.")]
    public int MaximumParticipants { get; set; }

    [Range(0, 100000, ErrorMessage = "Registration fee must be between ₹0 and ₹1,00,000.")]
    public decimal RegistrationFee { get; set; }

    // Display helpers (optional, used by the cards)
    public string? Description { get; set; }
    public string Icon { get; set; } = "bi-cpu";
}
