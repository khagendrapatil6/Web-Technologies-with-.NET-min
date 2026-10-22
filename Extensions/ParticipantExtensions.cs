namespace CollegeFestMVC.Extensions;

public static class ParticipantExtensions
{
    /// <summary>
    /// "Confirmed" when IsRegistrationCompleted is true, otherwise "Pending".
    /// (Free events are completed automatically; paid events are completed once
    /// the fee is confirmed.)
    /// </summary>
    public static string GetRegistrationStatus(this Participant participant) =>
        participant.IsRegistrationCompleted ? "Confirmed" : "Pending";

    /// <summary>
    /// ₹0 = "Free Event", above ₹0 up to ₹500 = "Standard Event", above ₹500 = "Premium Event".
    /// </summary>
    public static string GetFeeCategory(this Participant participant) =>
        participant.RegistrationFee.GetFeeCategory();

    public static string GetFeeCategory(this Event ev) =>
        ev.RegistrationFee.GetFeeCategory();

    public static string GetFeeCategory(this decimal fee) => fee switch
    {
        <= 0m   => "Free Event",
        <= 500m => "Standard Event",
        _       => "Premium Event"
    };

    /// <summary>Indian format with ₹ symbol, e.g. 1200 -> ₹1,200 (lakh grouping: ₹1,00,000).</summary>
    public static string ToRupees(this decimal amount)
    {
        CultureInfo culture;
        try { culture = CultureInfo.GetCultureInfo("en-IN"); }
        catch (CultureNotFoundException) { culture = CultureInfo.InvariantCulture; }
        return "₹" + amount.ToString("N0", culture);
    }

    public static string GetYearLabel(this Participant participant) => participant.Year switch
    {
        1 => "1st Year",
        2 => "2nd Year",
        3 => "3rd Year",
        4 => "4th Year",
        _ => "-"
    };
}
