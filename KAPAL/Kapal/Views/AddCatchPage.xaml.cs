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
                    ? $"Landing #{_state.SelectedLanding.LandingId} - Vessel: {_state.SelectedVessel?.Name}"
                    : "Landing: (belum dipilih)";

                dgSpecies.ItemsSource = _rows;
                Renumber();
                UpdateSpeciesCount();
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
            if (_rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk dihapus.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Apakah Anda yakin ingin menghapus semua data species?",
                "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _rows.Clear();
                Renumber();
                UpdateSpeciesCount();
            }
        }

        private void BtnRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is CatchRow row)
            {
                _rows.Remove(row);
                Renumber();
                UpdateSpeciesCount();
            }
        }

        private void BtnAddSpecies_Click(object sender, RoutedEventArgs e)
        {
            // validasi input
            var species = (tbSpecies.Text ?? "").Trim();
            var weightText = (tbWeight.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(species) || string.IsNullOrWhiteSpace(weightText))
            {
                MessageBox.Show("Species name dan Weight (kg) harus diisi.", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(weightText, NumberStyles.Number, CultureInfo.InvariantCulture, out var w) || w <= 0)
            {
                MessageBox.Show("Weight (kg) harus berupa angka positif.", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _rows.Add(new CatchRow { Species = species, WeightKg = w });
            Renumber();
            UpdateSpeciesCount();

            // reset field agar cepat input banyak
            tbSpecies.Clear();
            tbWeight.Clear();
            tbSpecies.Focus();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_state.SelectedLanding == null)
            {
                MessageBox.Show("Silakan pilih landing terlebih dahulu.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_rows.Count == 0)
            {
                MessageBox.Show("Belum ada species yang ditambahkan.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
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

                MessageBox.Show($"{_rows.Count} catch data berhasil disimpan!", "Sukses",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _rows.Clear();
                Renumber();
                UpdateSpeciesCount();

                // balik ke Home
                var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
                frame?.Navigate(new InputNewPage(_state));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan catch data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Renumber()
        {
            int i = 1;
            foreach (var r in _rows) r.No = i++;
            dgSpecies.Items.Refresh();
        }

        private void UpdateSpeciesCount()
        {
            txtTotalSpecies.Text = _rows.Count == 0
                ? "(no species)"
                : $"({_rows.Count} species)";
        }

        private void dgSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
