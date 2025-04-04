using GestionnaireOrthophonie.Models;
using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestionnaireOrthophonie.Controllers
{
    public class HomeController : Controller
    {
        private readonly PatientsOperationsManager _databaseOperationsManager;
        private readonly PlanningOperationsManager _planningOperationsManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(PatientsOperationsManager databaseOperationsManager, PlanningOperationsManager planningOperationsManager, UserManager<IdentityUser> userManager)
        {
            _databaseOperationsManager = databaseOperationsManager;
            _planningOperationsManager = planningOperationsManager;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult DisplayPatientsInformation()
        {
            string? userId = _userManager.GetUserId(User);
            List<Patient> patients = _databaseOperationsManager.GetAllUserPatients(userId);

            return View(patients);
        }

        [Authorize]
        public IActionResult GetCurrentPeriodView()
        {
            string? userId = _userManager.GetUserId(User);
            Dictionary<int, List<Session>> sessionsByDay = _planningOperationsManager.GetVisualizedPeriodSessionsByDay(userId);
            NamedPeriod? currentPeriodType = _planningOperationsManager.GetCurrentPeriodType(userId);

            ViewBag.PeriodTypes = _planningOperationsManager.GetAllPeriodTypes();
            ViewBag.CurrentPeriodTypeName = currentPeriodType != null ? currentPeriodType.Name : "Semaine";
            ViewBag.CurrentPeriodTypeWeeksLength = currentPeriodType != null ? currentPeriodType.WeeksLength : 1;

            return View("_PlanningView", sessionsByDay);
        }
    }
}
