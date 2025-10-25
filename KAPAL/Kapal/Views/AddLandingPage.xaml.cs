using Kapal.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class AddLandingPage : Page
    {
        private readonly AppState _state;
        private readonly Frame _root;

        public AddLandingPage(AppState state, Frame root)
        {
            InitializeComponent();
            _state = state;
            _root = root;

            Loaded += (_, __) =>
            {
                lblVessel.Text = _state.SelectedVessel != null
                    ? $"Vessel: {_state.SelectedVessel.VesselId} - {_state.SelectedVessel.Name}"
                    : "Vessel: (belum dipilih)";
                dpLandedAt.SelectedDate = DateTime.UtcNow;
            };
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => _root.Navigate(new HomePage(_state, _root));

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
                MessageBox.Show("Landing tersimpan.");
                _root.Navigate(new AddCatchPage(_state, _root));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal simpan landing: {ex.Message}");
            }
        }
    }
}
