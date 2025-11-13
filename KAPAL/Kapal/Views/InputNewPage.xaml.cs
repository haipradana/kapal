using Kapal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class InputNewPage : Page
    {
   private readonly AppState _state;
        private List<Vessel> _all = new();

  public InputNewPage(AppState state)
        {
            InitializeComponent();
     _state = state;
            Loaded += async (_, __) => await LoadAsync();
        }

      private async Task LoadAsync()
        {
    _state.SelectedVessel = null;
    btnAddLanding.IsEnabled = false;

     _all = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();
      dgVessels.ItemsSource = _all;
            
         // Disable CRUD buttons for guest
          ApplyRoleBasedUI();
        }

        private void ApplyRoleBasedUI()
        {
            if (!_state.IsAdmin)
    {
     btnAddVessel.IsEnabled = false;
         btnAddVessel.Opacity = 0.5;
             btnAddLanding.IsEnabled = false;
            btnAddLanding.Opacity = 0.5;
     }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
var q = (tbSearch.Text ?? "").Trim().ToLowerInvariant();
      dgVessels.ItemsSource = string.IsNullOrEmpty(q)
              ? _all
  : _all.Where(v => (v.Name ?? "").ToLowerInvariant().Contains(q)).ToList();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
       tbSearch.Clear();
            dgVessels.ItemsSource = _all;
        }

        private void DgVessels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _state.SelectedVessel = dgVessels.SelectedItem as Vessel;
        
            // Only enable if admin
          if (_state.IsAdmin)
            {
            btnAddLanding.IsEnabled = _state.SelectedVessel != null;
            }
  }

        private void BtnAddVessel_Click(object sender, RoutedEventArgs e)
    {
      if (!_state.IsAdmin) return;
  
            var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
     if (frame != null)
            {
  frame.Navigate(new AddVesselPage(_state));
            }
        }

        private void BtnAddLanding_Click(object sender, RoutedEventArgs e)
        {
            if (!_state.IsAdmin || _state.SelectedVessel == null) return;
            
    var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
            if (frame != null)
            {
     frame.Navigate(new AddLandingPage(_state));
   }
        }

     private async void BtnRemoveVessel_Click(object sender, RoutedEventArgs e)
 {
            if (!_state.IsAdmin) return;
     
         if (_state.SelectedVessel == null)
     {
      MessageBox.Show("Pilih vessel yang ingin dihapus.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        return;
     }

        var result = MessageBox.Show($"Anda yakin ingin menghapus vessel '{_state.SelectedVessel.Name}'?", "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Warning);
   if (result == MessageBoxResult.Yes)
   {
          try
                {
           await _state.VesselRepo.DeleteAsync(_state.SelectedVessel.VesselId);
   MessageBox.Show("Vessel berhasil dihapus.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync(); // Refresh list
     }
      catch (Exception ex)
      {
        MessageBox.Show($"Gagal menghapus vessel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
         }
        }

    private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
      {

   }

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
    // Opsional: tambahkan logika jika ingin mengubah tampilan saat fokus
        }

      private void tbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
     // Opsional: tambahkan logika jika ingin mengubah tampilan saat kehilangan fokus
        }
    }
}
