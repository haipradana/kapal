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
        
     // Login status
        public bool IsAdmin { get; set; } = false;
    }

    public partial class MainWindow : Window
    {
        public AppState State { get; private set; } = new AppState();
        private bool _isInitialized = false;

        public MainWindow()
        {
            InitializeComponent();
         
    // Hide sidebar immediately
            var sidebarColumn = (ColumnDefinition)FindName("SidebarColumn");
        if (sidebarColumn != null)
            {
         sidebarColumn.Width = new GridLength(0);
      }
            
    // Show login page immediately (no delay)
            Loaded += (_, __) => ShowLoginPage();
        }

   private async Task InitializeServicesAsync()
        {
       if (_isInitialized) return;
     
       try
         {
        State.Client = await SupabaseService.GetClientAsync();
                State.VesselRepo = new VesselRepository(State.Client);
                State.LandingRepo = new LandingRepository(State.Client);
        State.CatchRepo = new CatchRepository(State.Client);
    _isInitialized = true;
    }
  catch (Exception ex)
            {
             MessageBox.Show($"Init error: {ex.Message}", "Error",
               MessageBoxButton.OK, MessageBoxImage.Error);
   }
      }

     private void ShowLoginPage()
        {
        RootFrame.Navigate(new Views.LoginPage(async isAdmin =>
            {
                State.IsAdmin = isAdmin;
         await OnLoginSuccessAsync();
        }));
        }

     private async Task OnLoginSuccessAsync()
 {
    // Initialize services after successful login
  await InitializeServicesAsync();
            
            // Show sidebar
            var sidebarColumn = (ColumnDefinition)FindName("SidebarColumn");
            if (sidebarColumn != null)
            {
                sidebarColumn.Width = new GridLength(260);
            }
     
        // Update UI based on role
            UpdateUIForRole();
        
            // Navigate to Dashboard
          RootFrame.Navigate(new Views.DashboardPage(State));
        }

 private void UpdateUIForRole()
     {
   // Disable navigation buttons for guest
            if (!State.IsAdmin)
            {
     btnNavInputNew.IsEnabled = false;
btnNavInputNew.Opacity = 0.5;
     }
            else
   {
           btnNavInputNew.IsEnabled = true;
  btnNavInputNew.Opacity = 1.0;
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
