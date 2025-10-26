using Kapal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class CatchDataPage : Page
    {
        private readonly AppState _state;

        public CatchDataPage(AppState state)
  {
   InitializeComponent();
      _state = state;
   Loaded += async (_, __) => await LoadAsync();
        }

        private async Task LoadAsync()
        {
  try
      {
    var catches = await _state.CatchRepo.GetAllAsync() ?? new List<Catch>();
    var landings = await _state.LandingRepo.GetAllAsync() ?? new List<Landing>();
   var vessels = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();

    // Join catch with landing and vessel
   var catchesWithVessel = catches.Select(c =>
  {
       var landing = landings.FirstOrDefault(l => l.LandingId == c.LandingId);
  var vessel = landing != null ? vessels.FirstOrDefault(v => v.VesselId == landing.VesselId) : null;
    
    return new CatchWithVessel
  {
          CatchId = c.CatchId,
       LandingId = c.LandingId,
     VesselId = vessel?.VesselId ?? 0,
 VesselName = vessel?.Name ?? "Unknown",
 Species = c.Species,
   WeightKg = c.WeightKg
      };
  }).ToList();

      dgCatches.ItemsSource = catchesWithVessel;
    }
catch (Exception ex)
    {
MessageBox.Show($"Error loading catches: {ex.Message}", "Error", 
     MessageBoxButton.OK, MessageBoxImage.Error);
}
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();
    }
}
