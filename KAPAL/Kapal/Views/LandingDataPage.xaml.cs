using Kapal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class LandingDataPage : Page
    {
        private readonly AppState _state;

        public LandingDataPage(AppState state)
{
       InitializeComponent();
  _state = state;
  Loaded += async (_, __) => await LoadAsync();
        }

        private async Task LoadAsync()
        {
   try
      {
var landings = await _state.LandingRepo.GetAllAsync() ?? new List<Landing>();
    var vessels = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();

   // Join landing with vessel
    var landingsWithVessel = landings.Select(l =>
         {
   var vessel = vessels.FirstOrDefault(v => v.VesselId == l.VesselId);
     return new LandingWithVessel
       {
    LandingId = l.LandingId,
      VesselId = l.VesselId,
VesselName = vessel?.Name ?? "Unknown",
  LandedAt = l.LandedAt,
    Notes = l.Notes
};
        }).ToList();

      dgLandings.ItemsSource = landingsWithVessel;
      }
  catch (Exception ex)
   {
    MessageBox.Show($"Error loading landings: {ex.Message}", "Error", 
  MessageBoxButton.OK, MessageBoxImage.Error);
    }
        }

   private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();
    }
}
