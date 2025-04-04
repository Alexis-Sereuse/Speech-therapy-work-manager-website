using GestionnaireOrthophonie.Models;
using GestionnaireOrthophonie.Models.Entities;
using GestionnaireOrthophonie.Models.ModelValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestionnaireOrthophonie.Controllers
{
    public class PatientsController : Controller
    {
        private readonly PatientsOperationsManager _databaseOperationsManager;
        private readonly UserManager<IdentityUser> _userManager;

        public PatientsController(PatientsOperationsManager databaseOperationsManager, UserManager<IdentityUser> userManager)
        {
            _databaseOperationsManager = databaseOperationsManager;
            _userManager = userManager;
        }

        public IActionResult DisplayPatientCard(int patientId)
        {
            ViewBag.Action = "Displaying";
            Patient? patient = _databaseOperationsManager.GetPatientById(patientId);
            return View("_PatientCard", patient);
        }

        public IActionResult GetPatientAddingPage()
        {
            ViewBag.UserId = _userManager.GetUserId(User);
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(PatientAddingActionFilterAttribute))]
        public IActionResult AddPatient([FromForm] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _databaseOperationsManager.AddPatient(patient);
                return RedirectToAction("DisplayPatientsInformation", "Home");
            }

            ViewBag.UserId = _userManager.GetUserId(User);
            return View(nameof(GetPatientAddingPage));
        }

        [ServiceFilter(typeof(PatientIdVerificationActionFilterAttribute))]
        public IActionResult DeletePatient(int patientId)
        {
            _databaseOperationsManager.DeletePatient(patientId);
            return RedirectToAction("DisplayPatientsInformation", "Home");
        }

        [ServiceFilter(typeof(PatientIdVerificationActionFilterAttribute))]
        public IActionResult ModifyPatientSection(int patientId)
        {
            ViewBag.Action = "Modifying";
            Patient? patient = _databaseOperationsManager.GetPatientById(patientId);
            return View("_PatientCard", patient);
        }

        [HttpPost]
        public IActionResult ApplyModifications([FromForm] Patient patient)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Action = "Displaying";
                _databaseOperationsManager.ModifyPatient(patient);
                return View("_PatientCard", patient);
            }
            else
            {
                ViewBag.Action = "Modifying";
                return View("_PatientCard", _databaseOperationsManager.GetPatientById(patient.Id));
            }
        }
    }
}
