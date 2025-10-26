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
      btnAddLanding.IsEnabled = _state.SelectedVessel != null;
   }

  private void BtnAddVessel_Click(object sender, RoutedEventArgs e)
{
     // TODO: Open Add Vessel dialog or navigate to add vessel page
     MessageBox.Show("Add Vessel feature - To be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
}

        private void BtnAddLanding_Click(object sender, RoutedEventArgs e)
        {
  if (_state.SelectedVessel == null) return;
      // TODO: Open Add Landing dialog or navigate to add landing page
      MessageBox.Show($"Add Landing for Vessel: {_state.SelectedVessel.Name}\nTo be implemented", 
    "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
