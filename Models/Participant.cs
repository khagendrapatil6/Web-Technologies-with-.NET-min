namespace CollegeFestMVC.Models;

public class Participant
{
    public int ParticipantId { get; set; }

    [Required(ErrorMessage = "Please enter the participant name.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 80 characters.")]
    [Display(Name = "Participant Name")]
    public string ParticipantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter an email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(120)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a department.")]
    public string Department { get; set; } = string.Empty;

    [Range(1, 4, ErrorMessage = "Please select an academic year (1 to 4).")]
    [Display(Name = "Academic Year")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Please select an event.")]
    [Display(Name = "Event Name")]
    public string EventName { get; set; } = string.Empty;

    [Display(Name = "Team Event")]
    public bool IsTeamEvent { get; set; }

    // ---- Set by the server (never bound from the form) ----

    /// <summary>Fee copied from the chosen event at registration time.</summary>
    public decimal RegistrationFee { get; set; }

    /// <summary>
    /// Registration rule: free events are completed immediately; paid events stay
    /// pending until the fee is confirmed on the Details page.
    /// </summary>
    public bool IsRegistrationCompleted { get; set; }

    /// <summary>Nullable: null until the participant is saved.</summary>
    public DateTime? RegisteredOn { get; set; }
}
