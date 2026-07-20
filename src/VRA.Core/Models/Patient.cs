using System.Reflection;
using VRA.Common.Enums;

namespace VRA.Core.Models;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Identification
    public string MedicalRecordNumber { get; set; } = string.Empty;
    public string IcPassportNumber { get; set; } = string.Empty;

    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Today;
    public Gender Gender { get; set; } = Gender.Unknown;

    // Contact
    public string ParentGuardian { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // Clinical
    public string ReferredBy { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // -------------------------
    // Computed Properties
    // -------------------------

    public string FullName => $"{FirstName} {LastName}".Trim();
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;

            if (DateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
    public string AgeDisplay
    {
        get
        {
            var today = DateTime.Today;

            int years = today.Year - DateOfBirth.Year;
            int months = today.Month - DateOfBirth.Month;

            if (today.Day < DateOfBirth.Day)
                months--;

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return $"{years}y {months}m";
        }
    }

}


