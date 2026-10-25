
using System.Linq;
using System.Windows.Controls;
using VRA.App.Display;
using VRA.App.ViewModels;

namespace VRA.App.Views.Pages;

public partial class ReinforcementView : UserControl
{
    private bool _isRestoringSelection;

    public ReinforcementView(ReinforcementViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }


    private void ComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_isRestoringSelection ||
            sender is not ComboBox comboBox ||
            DataContext is not ReinforcementViewModel viewModel)
        {
            return;
        }

        string? targetName = comboBox.Tag?.ToString();
        string? selectedDeviceName = comboBox.SelectedValue?.ToString();

        if (string.IsNullOrWhiteSpace(targetName) ||
            string.IsNullOrWhiteSpace(selectedDeviceName))
        {
            return;
        }

        // Get the previously selected device.
        string? previousDeviceName = e.RemovedItems
            .OfType<DisplayInfo>()
            .FirstOrDefault()
            ?.DeviceName;

        // TryAssignDisplay checks duplicates and saves valid assignments.
        if (viewModel.TryAssignDisplay(targetName, selectedDeviceName))
        {
            return;
        }

        // Restore the previous mapping if the selection was rejected.
        _isRestoringSelection = true;

        try
        {
            comboBox.SelectedValue = previousDeviceName;

            // Restore the ViewModel mapping too, because the ComboBox
            // uses a two-way SelectedValue binding.
            viewModel.TryAssignDisplay(targetName, previousDeviceName);
        }
        finally
        {
            _isRestoringSelection = false;
        }
    }

}
