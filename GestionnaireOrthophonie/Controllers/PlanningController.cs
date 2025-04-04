using GestionnaireOrthophonie.Models;
using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace GestionnaireOrthophonie.Controllers
{
    public class PlanningController : Controller
    {
        private readonly PlanningOperationsManager _planningOperationsManager;
        private readonly PatientsOperationsManager _patientsOperationsManager;
        private readonly UserManager<IdentityUser> _userManager;

        public PlanningController(PlanningOperationsManager planningOperationsManager, PatientsOperationsManager patientsOperationsManager, UserManager<IdentityUser> userManager)
        {
            _planningOperationsManager = planningOperationsManager;
            _patientsOperationsManager = patientsOperationsManager;
            _userManager = userManager;
        }

        public IActionResult GetPreviousPeriodInformation()
        {
            string? userId = _userManager.GetUserId(User);
            NamedPeriod? currentPeriodType = _planningOperationsManager.GetCurrentPeriodType(userId);
            PeriodsCalculationManager.OnPreviousPeriodVisualizationButtonPressed(currentPeriodType);

            return RedirectToAction("GetCurrentPeriodView", "Home");
        }

        public IActionResult GetNextPeriodInformation()
        {
            string? userId = _userManager.GetUserId(User);
            NamedPeriod? currentPeriodType = _planningOperationsManager.GetCurrentPeriodType(userId);
            PeriodsCalculationManager.OnNextPeriodVisualizationButtonPressed(currentPeriodType);

            return RedirectToAction("GetCurrentPeriodView", "Home");
        }

        public IActionResult GetCurrentPeriodInformation()
        {
            PeriodsCalculationManager.OnCurrentPeriodVisualizationButtonPressed();

            return RedirectToAction("GetCurrentPeriodView", "Home");
        }

        public IActionResult GetAddSessionPage()
        {
            ViewBag.EndDateWanted = false;
            string? userId = _userManager.GetUserId(User);
            ViewBag.UserId = userId;
            ViewBag.Patients = _patientsOperationsManager.GetAllUserPatients(userId);
            ViewBag.Action = "Adding";
            return View();
        }

        public IActionResult TriggerEndDatePresence(string endDatePresent, string actionType, int sessionId, string startDate, string? endDate, string? title, string? preparationInformation, string? progressInformation, int patientId)
        {
            ViewBag.EndDateWanted = !bool.Parse(endDatePresent);
            string? userId = _userManager.GetUserId(User);
            ViewBag.Patients = _patientsOperationsManager.GetAllUserPatients(userId);
            ViewBag.Action = actionType;

            if (actionType == "Adding")
            {
                ViewBag.UserId = userId;
                return View(nameof(GetAddSessionPage));
            }
            else
            {
                DateTime startDateTime = DateTime.Parse(startDate);
                DateTime? endDateTime = endDate == null ? null : DateTime.Parse(endDate);
                Session session = new Session(sessionId, startDateTime, endDateTime, title, preparationInformation, progressInformation, patientId);
                return View("_SessionView", session);
            }
        }

        [HttpPost]
        public IActionResult AddSession([FromForm] Session session, string endDatePresent)
        {
            if (ModelState.IsValid)
            {
                if (session.Patient == null)
                    session.Patient = _patientsOperationsManager.GetPatientById(session.PatientId);

                if (session.EndDate == null)
                    session.EndDate = session.StartDate.AddHours(PeriodsCalculationManager.NO_END_DATE_FILLED_HOURS_COUNT);

                _planningOperationsManager.AddSession(session);
                return RedirectToAction("GetCurrentPeriodView", "Home");
            }

            ViewBag.EndDateWanted = bool.Parse(endDatePresent);
            string? userId = _userManager.GetUserId(User);
            ViewBag.UserId = userId;
            ViewBag.Patients = _patientsOperationsManager.GetAllUserPatients(userId);
            ViewBag.Action = "Adding";
            return View(nameof(GetAddSessionPage));
        }

        public IActionResult GetSessionView(int sessionId)
        {
            Session? session = _planningOperationsManager.GetSessionById(sessionId);
            ViewBag.Action = "Displaying";
            return View("_SessionView", session);
        }

        public IActionResult GetSessionModificationView(int sessionId)
        {
            Session? session = _planningOperationsManager.GetSessionById(sessionId);

            if (session == null)
            {
                return new BadRequestObjectResult(ModelState)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            ViewBag.Action = "Modifying";
            ViewBag.EndDateWanted = session.EndDate != null;
            string? userId = _userManager.GetUserId(User);
            ViewBag.Patients = _patientsOperationsManager.GetAllUserPatients(userId);
            return View("_SessionView", session);
        }

        [HttpPost]
        public IActionResult ModifySession(Session session, string endDateWanted)
        {
            if (ModelState.IsValid)
            {
                if (session.EndDate == null)
                    session.EndDate = session.StartDate.AddHours(PeriodsCalculationManager.NO_END_DATE_FILLED_HOURS_COUNT);

                _planningOperationsManager.ModifySession(session);
                int sessionId = session.Id;
                return RedirectToAction(nameof(GetSessionView), new { sessionId });
            }

            ViewBag.Action = "Modifying";
            ViewBag.EndDateWanted = bool.Parse(endDateWanted);
            string? userId = _userManager.GetUserId(User);
            ViewBag.Patients = _patientsOperationsManager.GetAllUserPatients(userId);
            return View("_SessionView", session);
        }

        public IActionResult DeleteSession(int sessionId)
        {
            _planningOperationsManager.DeleteSession(sessionId);
            return RedirectToAction("GetCurrentPeriodView", "Home");
        }
    }
}
