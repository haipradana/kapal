// MainWindow.xaml.cs
using Kapal.Models;
using Kapal.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace Kapal
{
    public partial class MainWindow : Window
    {
        // repos
        private Supabase.Client _client;
        private VesselRepository _vesselRepo;
        private LandingRepository _landingRepo;
        private CatchRepository _catchRepo;

        // state terakhir (untuk mengaktifkan langkah berikutnya)
        private Vessel _lastVessel;
        private Landing _lastLanding;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _client = await SupabaseService.GetClientAsync();
                _vesselRepo = new VesselRepository(_client);
                _landingRepo = new LandingRepository(_client);
                _catchRepo = new CatchRepository(_client);

                await RefreshVesselsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Init error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // -------------------- Vessel --------------------
        private async void btnSaveVessel_Click(object sender, RoutedEventArgs e)
        {
            // validasi sederhana
            if (string.IsNullOrWhiteSpace(tbVesselName.Text) ||
                string.IsNullOrWhiteSpace(tbRegNumber.Text))
            {
                MessageBox.Show("Name dan RegNumber wajib diisi.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var vessel = new Vessel
            {
                Name = tbVesselName.Text.Trim(),
                RegNumber = tbRegNumber.Text.Trim(),
                OwnerName = tbOwnerName.Text?.Trim() ?? "",
                Gear = tbGear.Text?.Trim() ?? ""
            };

            try
            {
                var saved = await _vesselRepo.InsertAsync(vessel);
                _lastVessel = saved;

                lblCurrentVesselId.Text = saved.VesselId.ToString();
                btnAddLanding.IsEnabled = true;   // boleh ke langkah Landing

                await RefreshVesselsAsync();

                MessageBox.Show("Vessel tersimpan.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

                // reset input vessel (opsional)
                tbVesselName.Clear(); tbRegNumber.Clear(); tbOwnerName.Clear(); tbGear.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal simpan vessel: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // -------------------- Landing --------------------
        private async void btnAddLanding_Click(object sender, RoutedEventArgs e)
        {
            if (_lastVessel == null)
            {
                MessageBox.Show("Simpan Vessel dulu.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var landedAt = dpLandedAt.SelectedDate?.ToUniversalTime() ?? DateTime.UtcNow;

            var landing = new Landing
            {
                VesselId = _lastVessel.VesselId,
                LandedAt = landedAt,
                Notes = string.IsNullOrWhiteSpace(tbNotes.Text) ? "-" : tbNotes.Text.Trim()
            };

            try
            {
                var saved = await _landingRepo.InsertAsync(landing);
                _lastLanding = saved;

                lblCurrentLandingId.Text = saved.LandingId.ToString();
                btnAddCatch.IsEnabled = true;     // boleh ke langkah Catch

                MessageBox.Show("Landing tersimpan.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

                // reset notes (opsional)
                tbNotes.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal simpan landing: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // -------------------- Catch --------------------
        private async void btnAddCatch_Click(object sender, RoutedEventArgs e)
        {
            if (_lastLanding == null)
            {
                MessageBox.Show("Tambah Landing dulu.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbSpecies.Text) ||
                string.IsNullOrWhiteSpace(tbWeightKg.Text))
            {
                MessageBox.Show("Species dan Weight wajib diisi.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!decimal.TryParse(tbWeightKg.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var weight))
            {
                MessageBox.Show("Format Weight (kg) tidak valid.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var catchItem = new Models.Catch
            {
                LandingId = _lastLanding.LandingId,
                Species = tbSpecies.Text.Trim(),
                WeightKg = weight
            };

            try
            {
                var saved = await _catchRepo.InsertAsync(catchItem);
                MessageBox.Show("Catch tersimpan.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

                // reset input catch (opsional)
                tbSpecies.Clear(); tbWeightKg.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal simpan catch: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // -------------------- Helpers --------------------
        private async Task RefreshVesselsAsync()
        {
            try
            {
                var list = await _vesselRepo.GetAllAsync();
                dgVessels.ItemsSource = null;
                dgVessels.ItemsSource = list ?? new List<Vessel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load vessels: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // extension kecil: DateTime? -> UTC
    internal static class DateTimeExt
    {
        public static DateTime ToUniversalTime(this DateTime date)
            => DateTime.SpecifyKind(date, DateTimeKind.Local).ToUniversalTime();
    }
}
