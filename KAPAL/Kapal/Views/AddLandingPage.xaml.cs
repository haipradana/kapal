using Kapal.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class AddLandingPage : Page
    {
        private readonly AppState _state;

        public AddLandingPage(AppState state)
        {
            InitializeComponent();
            _state = state;

            Loaded += (_, __) =>
            {
                lblVessel.Text = _state.SelectedVessel != null
                    ? $"Vessel: {_state.SelectedVessel.VesselId} - {_state.SelectedVessel.Name}"
                    : "Vessel: (belum dipilih)";
                dpLandedAt.SelectedDate = DateTime.UtcNow;
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

        private async void BtnSaveNext_Click(object sender, RoutedEventArgs e)
        {
            if (_state.SelectedVessel == null)
            {
                MessageBox.Show("Pilih / buat vessel dulu.");
                return;
            }

            var landing = new Landing
            {
                VesselId = _state.SelectedVessel.VesselId,
                LandedAt = (dpLandedAt.SelectedDate ?? DateTime.UtcNow).ToUniversalTime(),
                Notes = string.IsNullOrWhiteSpace(tbNotes.Text) ? "-" : tbNotes.Text.Trim()
            };

            try
            {
                var saved = await _state.LandingRepo.InsertAsync(landing);
                _state.SelectedLanding = saved;
                MessageBox.Show("Landing berhasil disimpan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
                if (frame != null)
                {
                    frame.Navigate(new AddCatchPage(_state)); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan landing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
