using Kapal.Models;
using Kapal.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal
{
    public class AppState
    {
        public Supabase.Client Client { get; set; } = null!;
        public VesselRepository VesselRepo { get; set; } = null!;
        public LandingRepository LandingRepo { get; set; } = null!;
        public CatchRepository CatchRepo { get; set; } = null!;

        public Vessel? SelectedVessel { get; set; }
        public Landing? SelectedLanding { get; set; }
    }

    public partial class MainWindow : Window
    {
        public AppState State { get; private set; } = new AppState();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (_, __) => await BootAsync();
        }

        private async Task BootAsync()
        {
            try
            {
                State.Client = await SupabaseService.GetClientAsync();
                State.VesselRepo = new VesselRepository(State.Client);
                State.LandingRepo = new LandingRepository(State.Client);
                State.CatchRepo = new CatchRepository(State.Client);

                RootFrame.Navigate(new Views.HomePage(State, RootFrame));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Init error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
