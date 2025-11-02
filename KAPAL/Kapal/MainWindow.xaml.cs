using Kapal.Models;
using Kapal.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal
{
    /// <summary>
    /// AppState dengan polymorphism - menggunakan interface IRepository
    /// </summary>
    public class AppState
    {
        public Supabase.Client Client { get; set; } = null!;
     
      // Menggunakan interface untuk polymorphism
        public IRepository<Vessel> VesselRepo { get; set; } = null!;
        public IRepository<Landing> LandingRepo { get; set; } = null!;
        public IRepository<Catch> CatchRepo { get; set; } = null!;

 // Untuk akses method spesifik yang tidak ada di interface
   public VesselRepository VesselRepoImpl => (VesselRepository)VesselRepo;
    public LandingRepository LandingRepoImpl => (LandingRepository)LandingRepo;
        public CatchRepository CatchRepoImpl => (CatchRepository)CatchRepo;

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

                // Navigate to Dashboard by default
                RootFrame.Navigate(new Views.DashboardPage(State));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Init error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNavDashboard_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Views.DashboardPage(State));
        }

        private void BtnNavInputNew_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Views.InputNewPage(State));
        }

        private void BtnNavVessel_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Views.VesselDataPage(State));
        }

        private void BtnNavLanding_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Views.LandingDataPage(State));
        }

        private void BtnNavCatch_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Views.CatchDataPage(State));
        }
    }
}
