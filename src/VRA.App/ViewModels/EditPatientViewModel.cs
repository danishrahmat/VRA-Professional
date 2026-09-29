using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VRA.Common.Enums;
using VRA.Core.Interfaces;
using VRA.Core.Models;

namespace VRA.App.ViewModels;

public partial class EditPatientViewModel : ObservableObject
{
    private readonly IPatientService _patientService;

    public event Action<bool?>? RequestClose;

    private readonly Guid _patientId;

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


    public EditPatientViewModel(
        IPatientService patientService,
        Patient patient)
    {
        _patientService = patientService;

        _patientId = patient.Id;

        MedicalRecordNumber = patient.MedicalRecordNumber;
        IcPassportNumber = patient.IcPassportNumber;
        FirstName = patient.FirstName;
        LastName = patient.LastName;
        DateOfBirth = patient.DateOfBirth;
        Gender = patient.Gender;
        ParentGuardian = patient.ParentGuardian;
        PhoneNumber = patient.PhoneNumber;
        ReferredBy = patient.ReferredBy;
        Diagnosis = patient.Diagnosis;
        Notes = patient.Notes;
    }


    [RelayCommand]
    private async Task SavePatientAsync()
    {
        var patient = await _patientService.GetByIdAsync(_patientId);

        if (patient is null)
        {
            RequestClose?.Invoke(false);
            return;
        }

        patient.MedicalRecordNumber = MedicalRecordNumber.Trim();
        patient.IcPassportNumber = IcPassportNumber.Trim();
        patient.FirstName = FirstName.Trim();
        patient.LastName = LastName.Trim();
        patient.DateOfBirth = DateOfBirth;
        patient.Gender = Gender;
        patient.ParentGuardian = ParentGuardian.Trim();
        patient.PhoneNumber = PhoneNumber.Trim();
        patient.ReferredBy = ReferredBy.Trim();
        patient.Diagnosis = Diagnosis.Trim();
        patient.Notes = Notes.Trim();

        await _patientService.UpdateAsync(patient);

        RequestClose?.Invoke(true);
    }


    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(false);
    }
}