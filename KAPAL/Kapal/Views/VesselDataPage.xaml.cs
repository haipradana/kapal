using Kapal.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    /// <summary>
    /// VesselDataPage menggunakan polymorphism melalui IRepository interface
    /// </summary>
    public partial class VesselDataPage : Page
    {
        private readonly AppState _state;
  private List<Vessel> _all = new();

        public VesselDataPage(AppState state)
 {
        InitializeComponent();
   _state = state;
       Loaded += async (_, __) => await LoadAsync();
        }

        private async Task LoadAsync()
        {
    try
       {
            // Menggunakan polymorphism - IRepository interface
         _all = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();
         dgVessels.ItemsSource = _all;
     _state.SelectedVessel = null;
                UpdateButtonStates();
            }
    catch (Exception ex)
            {
 MessageBox.Show($"Error loading vessels: {ex.Message}", "Error",
  MessageBoxButton.OK, MessageBoxImage.Error);
   }
        }

        private void DgVessels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
  _state.SelectedVessel = dgVessels.SelectedItem as Vessel;
 UpdateButtonStates();
  }

private void UpdateButtonStates()
   {
 bool hasSelection = _state.SelectedVessel != null;
  btnEdit.IsEnabled = hasSelection;
 btnDelete.IsEnabled = hasSelection;
      }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
     var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
       frame?.Navigate(new AddVesselPage(_state));
        }

   private void BtnEdit_Click(object sender, RoutedEventArgs e)
     {
            if (_state.SelectedVessel == null) return;
       // TODO: Implement edit vessel dialog
   MessageBox.Show($"Edit Vessel: {_state.SelectedVessel.Name}\nTo be implemented", "Info",
    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
 {
       if (_state.SelectedVessel == null) return;

     var result = MessageBox.Show(
           $"Are you sure you want to delete vessel '{_state.SelectedVessel.Name}'?",
      "Confirm Delete",
      MessageBoxButton.YesNo,
        MessageBoxImage.Warning);

     if (result == MessageBoxResult.Yes)
   {
  try
      {
 // Menggunakan polymorphism - DeleteAsync dari IRepository
  await _state.VesselRepo.DeleteAsync(_state.SelectedVessel.VesselId);
    MessageBox.Show("Vessel deleted successfully!", "Success",
  MessageBoxButton.OK, MessageBoxImage.Information);
     await LoadAsync();
    }
      catch (Exception ex)
         {
           MessageBox.Show($"Error deleting vessel: {ex.Message}", "Error",
       MessageBoxButton.OK, MessageBoxImage.Error);
  }
          }
        }
    }
}
