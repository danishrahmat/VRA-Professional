
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VRA.Core.Interfaces;
using VRA.Common.Enums;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class NewPatientViewModel : ObservableObject
{
    private readonly IPatientService _patientService;

    public event Action<bool?>? RequestClose;

    public ObservableCollection<Gender> GenderOptions { get; } =
        new(Enum.GetValues<Gender>());

    [ObservableProperty]
    private string medicalRecordNumber = string.Empty;

    [ObservableProperty]
    private string icPassportNumber = string.Empty;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private DateTime dateOfBirth = DateTime.Today;

    [ObservableProperty]
    private Gender gender = Gender.Unknown;

    [ObservableProperty]
    private string parentGuardian = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string referredBy = string.Empty;

    [ObservableProperty]
    private string diagnosis = string.Empty;

    [ObservableProperty]
    private string notes = string.Empty;

    [ObservableProperty]
    private string validationMessage = string.Empty;

    public NewPatientViewModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [RelayCommand]
    private async Task SavePatientAsync()
    {
        ValidationMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(MedicalRecordNumber))
        {
            ValidationMessage = "Medical Record Number is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(IcPassportNumber))
        {
            ValidationMessage = "IC / Passport number is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            ValidationMessage = "First Name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            ValidationMessage = "Last Name is required.";
            return;
        }

        if (DateOfBirth.Date > DateTime.Today)
        {
            ValidationMessage = "Date of Birth cannot be in the future.";
            return;
        }

        if (Gender == Gender.Unknown)
        {
            ValidationMessage = "Please select a gender.";
            return;
        }


        if (await _patientService.MedicalRecordNumberExistsAsync(
                MedicalRecordNumber.Trim()))
        {
            ValidationMessage =
                "This Medical Record Number already exists.";
            return;
        }

        if (await _patientService.IcPassportNumberExistsAsync(
                IcPassportNumber.Trim()))
        {
            ValidationMessage =
                "This IC / Passport number already exists.";
            return;
        }


        var patient = new Patient
        {
            MedicalRecordNumber = MedicalRecordNumber.Trim(),
            IcPassportNumber = IcPassportNumber.Trim(),
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            DateOfBirth = DateOfBirth,
            Gender = Gender,
            ParentGuardian = ParentGuardian.Trim(),
            PhoneNumber = PhoneNumber.Trim(),
            ReferredBy = ReferredBy.Trim(),
            Diagnosis = Diagnosis.Trim(),
            Notes = Notes.Trim()
        };

        await _patientService.AddAsync(patient);

        RequestClose?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(false);
    }
}
