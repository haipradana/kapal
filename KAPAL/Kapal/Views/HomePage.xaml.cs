using Kapal.Models;
using System.Collections.Generic;
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
        }

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
            btnAddLanding.IsEnabled = has;
            btnAddCatch.IsEnabled = false; // aktif setelah ada landing terpilih (flow next page)
        }

        private void BtnAddVessel_Click(object sender, RoutedEventArgs e)
            => _root.Navigate(new AddVesselPage(_state, _root));

        private void BtnAddLanding_Click(object sender, RoutedEventArgs e)
        {
            if (_state.SelectedVessel == null) return;
            _root.Navigate(new AddLandingPage(_state, _root));
        }

        private void BtnAddCatch_Click(object sender, RoutedEventArgs e)
        {
            if (_state.SelectedVessel == null) return;
            _root.Navigate(new AddCatchPage(_state, _root));
        }
    }
}
