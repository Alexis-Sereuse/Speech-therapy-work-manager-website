using GestionnaireOrthophonie.Data;
using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace GestionnaireOrthophonie.Models
{
    public class PatientsOperationsManager
    {
        private readonly DatabaseContext _databaseContext;

        public PatientsOperationsManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<Patient> GetAllUserPatients(string userId)
        {
            return _databaseContext.Patients.Where(patient => patient.UserId == userId).ToList();
        }

        public Patient? GetPatientById(int id)
        {
            return _databaseContext.Patients.FirstOrDefault(patient => patient.Id == id);
        }

        public void AddPatient(Patient newPatient)
        {
            _databaseContext.Patients.Add(newPatient);
            _databaseContext.SaveChanges();
        }

        public void DeletePatient(int patientId)
        {
            Patient patient = _databaseContext.Patients.First(p => p.Id == patientId);
            _databaseContext.Patients.Remove(patient);
            _databaseContext.SaveChanges();
        }

        public void ModifyPatient(Patient modifiedPatient)
        {
            Patient? patient = GetPatientById(modifiedPatient.Id);

            patient.FirstName = modifiedPatient.FirstName;
            patient.Name = modifiedPatient.Name;
            patient.Gender = modifiedPatient.Gender;
            patient.Age = modifiedPatient.Age;
            patient.TherapeuticObjectives = modifiedPatient.TherapeuticObjectives;
            patient.Anamnesis = modifiedPatient.Anamnesis;
            patient.Pathologies = modifiedPatient.Pathologies;
            patient.Progress = modifiedPatient.Progress;
            patient.Difficulties = modifiedPatient.Difficulties;

            _databaseContext.SaveChanges();
        }
    }
}
