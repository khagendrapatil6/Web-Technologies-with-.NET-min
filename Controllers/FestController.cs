using CollegeFestMVC.Data;

namespace CollegeFestMVC.Controllers;

public class FestController : Controller
{
    // Data now lives in MySQL (see Data/ParticipantRepository.cs).
    private readonly ParticipantRepository _repo;

    public FestController(ParticipantRepository repo) => _repo = repo;

    private static readonly string[] Departments =
    {
        "Computer Science & Engineering", "Information Technology", "AI & Data Science",
        "Electronics & Communication", "Electrical Engineering", "Mechanical Engineering",
        "Civil Engineering", "Computer Applications"
    };

    // GET /Fest/Index  (default route)
    public async Task<IActionResult> Index()
    {
        var all = await _repo.GetAllAsync();
        return View(all);
    }

    // GET /Fest/Register
    [HttpGet]
    public IActionResult Register()
    {
        LoadDropdowns();
        return View(new Participant());
    }

    // POST /Fest/Register
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        [Bind("ParticipantName,Email,Department,Year,EventName,IsTeamEvent")] Participant participant)
    {
        var ev = EventCatalog.Find(participant.EventName);
        if (!string.IsNullOrEmpty(participant.EventName) && ev is null)
            ModelState.AddModelError(nameof(Participant.EventName), "Please choose a valid event.");

        if (ModelState.IsValid && ev is not null)
        {
            var (result, id) = await _repo.AddAsync(participant, ev);
            switch (result)
            {
                case AddResult.DuplicateEmail:
                    ModelState.AddModelError(nameof(Participant.Email),
                        "This email is already registered for the selected event.");
                    break;
                case AddResult.EventFull:
                    ModelState.AddModelError(nameof(Participant.EventName),
                        $"{ev.EventName} is full ({ev.MaximumParticipants} participants). Please choose another event.");
                    break;
                case AddResult.InvalidEvent:
                    ModelState.AddModelError(nameof(Participant.EventName), "Please choose a valid event.");
                    break;
            }
        }

        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(participant);
        }

        TempData["Success"] = $"{participant.ParticipantName} has been registered for {ev!.EventName}.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Fest/Details/1001
    public async Task<IActionResult> Details(int id)
    {
        var participant = await _repo.GetByIdAsync(id);
        if (participant is null)
        {
            TempData["Warning"] = $"No participant found with ID {id}.";
            return RedirectToAction(nameof(Index));
        }
        return View(participant);
    }

    // POST /Fest/Confirm/1001  (marks a pending paid registration as confirmed)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        var participant = await _repo.GetByIdAsync(id);
        if (participant is null)
        {
            TempData["Warning"] = $"No participant found with ID {id}.";
            return RedirectToAction(nameof(Index));
        }
        await _repo.ConfirmAsync(id);
        TempData["Success"] = $"Payment confirmed. {participant.ParticipantName}'s registration is now Confirmed.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET /Fest/Welcome
    public IActionResult Welcome() => View(EventCatalog.All);

    private void LoadDropdowns()
    {
        ViewBag.Departments = Departments;
        ViewBag.Events = EventCatalog.All;
    }
}
