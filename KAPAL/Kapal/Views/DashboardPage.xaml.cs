using Kapal.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class DashboardPage : Page
    {
        private readonly AppState _state;

        public DashboardPage(AppState state)
{
            InitializeComponent();
      _state = state;
            Loaded += async (_, __) => await LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
       {
           var vessels = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();
        var landings = await _state.LandingRepo.GetAllAsync() ?? new List<Landing>();
       var catches = await _state.CatchRepo.GetAllAsync() ?? new List<Catch>();

       // Update summary cards
          txtTotalVessel.Text = vessels.Count.ToString();
       txtTotalLanding.Text = landings.Count.ToString();
             txtTotalSpecies.Text = catches.Select(c => c.Species).Distinct().Count().ToString();

       // Build charts
              BuildSpeciesPie(catches);
  BuildVesselByGear(vessels);
     BuildLandingsTrend(landings);
            }
    catch (Exception ex)
            {
 MessageBox.Show($"Error loading dashboard: {ex.Message}",
    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
     }
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

            var model = new PlotModel { PlotMargins = new OxyThickness(10) };
          var ps = new PieSeries 
  { 
  StrokeThickness = 1, 
       Stroke = OxyColors.White,
   InsideLabelPosition = 0.8, 
  AngleSpan = 360, 
      StartAngle = 0 
        };

        var colors = new[] { "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6" };
            int idx = 0;

            foreach (var t in top)
{
            var slice = new PieSlice(t.Species, (double)t.Weight) 
      { 
  IsExploded = false,
      Fill = OxyColor.Parse(colors[idx % colors.Length])
  };
           ps.Slices.Add(slice);
      idx++;
     }

       model.Series.Add(ps);
    PlotSpeciesPie.Model = model;
   }

        // ---------- Chart 2: Bar Vessel count by Gear ----------
        private void BuildVesselByGear(List<Vessel> vessels)
        {
    var agg = vessels
     .GroupBy(v => string.IsNullOrWhiteSpace(v.Gear) ? "(None)" : v.Gear)
           .Select(g => new { Gear = g.Key, Count = g.Count() })
       .OrderByDescending(x => x.Count)
         .Take(6)
      .ToList();

      var model = new PlotModel { PlotMargins = new OxyThickness(80, 10, 10, 40) };

  var cat = new CategoryAxis { Position = AxisPosition.Left };
        foreach (var a in agg) cat.Labels.Add(a.Gear);

          var val = new LinearAxis 
    { 
     Position = AxisPosition.Bottom, 
 Minimum = 0, 
      MajorStep = 1, 
                MinorStep = 1 
    };

   var col = new BarSeries 
    { 
    LabelPlacement = LabelPlacement.Outside, 
      LabelFormatString = "{0}",
    FillColor = OxyColor.Parse("#3B82F6")
            };
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
         var x = new DateTimeAxis 
    { 
           Position = AxisPosition.Bottom, 
      StringFormat = "dd MMM", 
           IntervalType = DateTimeIntervalType.Days 
      };
            var y = new LinearAxis 
        { 
    Position = AxisPosition.Left, 
      Minimum = 0, 
          MajorStep = 1, 
           MinorStep = 1 
            };

  var series = new LineSeries 
      { 
     MarkerType = MarkerType.Circle,
                Color = OxyColor.Parse("#10B981"),
      MarkerFill = OxyColor.Parse("#10B981"),
      MarkerSize = 4,
      StrokeThickness = 2
   };

            foreach (var d in range)
         {
  var cnt = agg.TryGetValue(d, out var c) ? c : 0;
      series.Points.Add(DateTimeAxis.CreateDataPoint(d, cnt));
      }

    model.Axes.Add(x); 
    model.Axes.Add(y);
     model.Series.Add(series);

            PlotLandingsTrend.Model = model;
    }
    }
}
