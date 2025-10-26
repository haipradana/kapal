using Kapal.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class AddCatchPage : Page
    {
        private readonly AppState _state;

        // list sementara sebelum di-Save
        private readonly ObservableCollection<CatchRow> _rows = new();

        public AddCatchPage(AppState state)
        {
            InitializeComponent();
            _state = state;

            Loaded += (_, __) =>
            {
                lblLanding.Text = _state.SelectedLanding != null
                    ? $"Landing: {_state.SelectedLanding.LandingId} (Vessel {_state.SelectedVessel?.Name})"
                    : "Landing: (belum dipilih)";

                dgSpecies.ItemsSource = _rows;
                Renumber();
            };
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _rows.Clear();
            Renumber();
        }

        private void BtnRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is CatchRow row)
            {
                _rows.Remove(row);
                Renumber();
            }
        }

        private void BtnAddSpecies_Click(object sender, RoutedEventArgs e)
        {
            // validasi input
            var species = (tbSpecies.Text ?? "").Trim();
            var weightText = (tbWeight.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(species) || string.IsNullOrWhiteSpace(weightText))
            {
                MessageBox.Show("Isi Species dan Weight (kg).");
                return;
            }

            if (!decimal.TryParse(weightText, NumberStyles.Number, CultureInfo.InvariantCulture, out var w) || w <= 0)
            {
                MessageBox.Show("Weight (kg) tidak valid.");
                return;
            }

            _rows.Add(new CatchRow { Species = species, WeightKg = w });
            Renumber();

            // reset field agar cepat input banyak
            tbSpecies.Clear();
            tbWeight.Clear();
            tbSpecies.Focus();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_state.SelectedLanding == null)
            {
                MessageBox.Show("Tambah Landing dulu.");
                return;
            }

            if (_rows.Count == 0)
            {
                MessageBox.Show("Belum ada species yang ditambahkan.");
                return;
            }

            try
            {
                // simpan satu per satu ke repo
                foreach (var r in _rows.ToList())
                {
                    var model = new Catch
                    {
                        LandingId = _state.SelectedLanding.LandingId,
                        Species = r.Species,
                        WeightKg = r.WeightKg
                    };
                    await _state.CatchRepo.InsertAsync(model);
                }

                MessageBox.Show("Semua catch tersimpan.");
                _rows.Clear();
                Renumber();

                // balik ke Home
                var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
                frame?.Navigate(new InputNewPage(_state));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal simpan catch: {ex.Message}");
            }
        }

        private void Renumber()
        {
            int i = 1;
            foreach (var r in _rows) r.No = i++;
            dgSpecies.Items.Refresh();
        }
    }

    // Row tampilan (bukan entity DB)
    public class CatchRow
    {
        public int No { get; set; }
        public required string Species { get; set; }
        public decimal WeightKg { get; set; }
    }
}
