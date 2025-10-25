using Kapal.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class AddVesselPage : Page
    {
        private readonly AppState _state;
        private readonly Frame _root;

        public AddVesselPage(AppState state, Frame root)
        {
            InitializeComponent();
            _state = state;
            _root = root;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => _root.Navigate(new HomePage(_state, _root));

        private async void BtnSaveNext_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrWhiteSpace(tbReg.Text))
            {
                MessageBox.Show("Name dan Reg Number wajib diisi."); return;
            }

            var v = new Vessel
            {
                Name = tbName.Text.Trim(),
                RegNumber = tbReg.Text.Trim(),
                OwnerName = tbOwner.Text?.Trim() ?? "",
                Gear = tbGear.Text?.Trim() ?? ""
            };

            try
            {
                var saved = await _state.VesselRepo.InsertAsync(v);
                _state.SelectedVessel = saved;
                MessageBox.Show("Vessel tersimpan.");
                _root.Navigate(new AddLandingPage(_state, _root));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Gagal simpan vessel: {ex.Message}");
            }
        }
    }
}
