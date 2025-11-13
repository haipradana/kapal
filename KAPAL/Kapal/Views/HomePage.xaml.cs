using Kapal.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
  public partial class HomePage : Page
    {
        private readonly AppState _state;
        private readonly Frame _root;
 private List<Vessel> _all = new();

  public HomePage(AppState state, Frame root)
     {
  InitializeComponent();
            _state = state;
          _root = root;
       Loaded += async (_, __) => await LoadAsync();
        }

   private async Task LoadAsync()
  {
         _state.SelectedVessel = null;
      _state.SelectedLanding = null;
            btnAddLanding.IsEnabled = btnAddCatch.IsEnabled = false;

      _all = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();
 dgVessels.ItemsSource = _all;

            await LoadAnalyticsAsync();
       
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
      btnAddCatch.IsEnabled = false;
      btnAddCatch.Opacity = 0.5;
     }
        }

   private async Task LoadAnalyticsAsync()
        {
      // Ambil data (pastikan repo punya GetAllAsync)
      var landings = await _state.LandingRepo.GetAllAsync() ?? new List<Landing>();
     var catches = await _state.CatchRepo.GetAllAsync() ?? new List<Catch>();

  BuildSpeciesPie(catches);
       BuildVesselByGear(_all);
     BuildLandingsTrend(landings);
  }

        // ---------- Chart 1: Pie Species by total Weight ----------
        private void BuildSpeciesPie(List<Catch> catches)
        {
       var top = catches
      .GroupBy(c => c.Species ?? "(Unknown)")
           .Select(g => new { Species = g.Key, Weight = g.Sum(x => x.WeightKg) })
    .OrderByDescending(x => x.Weight)
    .Take(5)
      .ToList();

   var model = new PlotModel { Title = "", PlotMargins = new OxyThickness(10) };
         var ps = new PieSeries { StrokeThickness = 0.5, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0 };
      foreach (var t in top)
         ps.Slices.Add(new PieSlice(t.Species, (double)t.Weight) { IsExploded = false });

       model.Series.Add(ps);
        PlotSpeciesPie.Model = model;
        }

        // ---------- Chart 2: Column Vessel count by Gear ----------
        private void BuildVesselByGear(List<Vessel> vessels)
        {
         var agg = vessels
              .GroupBy(v => string.IsNullOrWhiteSpace(v.Gear) ? "(None)" : v.Gear)
      .Select(g => new { Gear = g.Key, Count = g.Count() })
     .OrderByDescending(x => x.Count)
        .Take(6)
         .ToList();

            var model = new PlotModel { PlotMargins = new OxyThickness(40, 10, 10, 40) };

            // FIXED: CategoryAxis harus di Y untuk BarSeries (horizontal)
            // Atau gunakan CategoryAxis di X dan swap ke ColumnSeries
    var cat = new CategoryAxis { Position = AxisPosition.Left };
 foreach (var a in agg) cat.Labels.Add(a.Gear);

            var val = new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, MajorStep = 1, MinorStep = 1 };

    var col = new BarSeries { LabelPlacement = LabelPlacement.Outside, LabelFormatString = "{0}" };
          foreach (var a in agg) col.Items.Add(new BarItem(a.Count));

       model.Axes.Add(cat);
         model.Axes.Add(val);
   model.Series.Add(col);

      PlotVesselByGear.Model = model;
        }

    // ---------- Chart 3: Line Landings per Day (last 14 days) ----------
        private void BuildLandingsTrend(List<Landing> landings)
  {
     var end = DateTime.UtcNow.Date;
 var start = end.AddDays(-13);

       var range = Enumerable.Range(0, 14).Select(i => start.AddDays(i)).ToList();
            var agg = landings
      .Where(l => l.LandedAt.Date >= start && l.LandedAt.Date <= end)
            .GroupBy(l => l.LandedAt.Date)
  .ToDictionary(g => g.Key, g => g.Count());

            var model = new PlotModel { PlotMargins = new OxyThickness(40, 10, 10, 40) };
       var x = new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd MMM", IntervalType = DateTimeIntervalType.Days };
    var y = new LinearAxis { Position = AxisPosition.Left, Minimum = 0, MajorStep = 1, MinorStep = 1 };

            var series = new LineSeries { MarkerType = MarkerType.Circle };
    foreach (var d in range)
            {
    var cnt = agg.TryGetValue(d, out var c) ? c : 0;
    series.Points.Add(DateTimeAxis.CreateDataPoint(d, cnt));
        }

  model.Axes.Add(x); model.Axes.Add(y);
            model.Series.Add(series);

    PlotLandingsTrend.Model = model;
        }

        // ====== existing list actions ======
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
     bool has = _state.SelectedVessel != null;
  
        // Only enable if admin
     if (_state.IsAdmin)
   {
     btnAddLanding.IsEnabled = has;
            }
    btnAddCatch.IsEnabled = false; // aktif setelah ada landing terpilih (flow lanjutan)
        }

        private void BtnAddVessel_Click(object sender, RoutedEventArgs e)
        {
       if (!_state.IsAdmin) return;
            
            _root.Navigate(new AddVesselPage(_state));
        }

        private void BtnAddLanding_Click(object sender, RoutedEventArgs e)
        {
       if (!_state.IsAdmin || _state.SelectedVessel == null) return;
  
         _root.Navigate(new AddLandingPage(_state));
        }

        private void BtnAddCatch_Click(object sender, RoutedEventArgs e)
  {
       if (!_state.IsAdmin) return;
            
            if (_state.SelectedLanding == null)
   {
      MessageBox.Show("Pilih landing terlebih dahulu.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
return;
       };
            
            var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
            if (frame != null)
          {
     // Assuming AddCatchPage exists and follows the new pattern
                // frame.Navigate(new AddCatchPage(_state));
        MessageBox.Show("Fitur Tambah Catch belum diimplementasikan sepenuhnya.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    }
    }
}
