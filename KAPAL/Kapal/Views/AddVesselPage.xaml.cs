using Kapal.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class AddVesselPage : Page
    {
        private readonly AppState _state;

        public AddVesselPage(AppState state)
        {
            InitializeComponent();
            _state = state;
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
                MessageBox.Show("Vessel berhasil disimpan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
                if (frame != null)
                {
                    // Ganti halaman saat ini dengan halaman AddLandingPage
                    frame.Navigate(new AddLandingPage(_state));
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan vessel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
