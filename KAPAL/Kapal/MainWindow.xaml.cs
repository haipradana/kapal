// MainWindow.xaml.cs
using Kapal.Models;
using Kapal.Services;
using System;
using System.Windows;  // penting buat Window & RoutedEventArgs

namespace Kapal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var client = await SupabaseService.GetClientAsync();

            // PASS the Supabase.Client, not client.Postgrest (update library baru co)
            var vesselRepo = new VesselRepository(client);
            var landingRepo = new LandingRepository(client);
            var catchRepo   = new CatchRepository(client);

            // Contoh insert
            var kapal = await vesselRepo.InsertAsync(new Vessel
            {
                Name = "KM IKN",
                RegNumber = "REG-002",
                OwnerName = "Pak Jokowi",
                Gear = "Pukat Harimau dan Bom air"
            });

            var landing = await landingRepo.InsertAsync(new Landing
            {
                VesselId = kapal.VesselId,
                LandedAt = DateTime.UtcNow,
                Notes = "-"
            });

            var tangkapan = await catchRepo.InsertAsync(new Models.Catch
            {
                LandingId = landing.LandingId,
                Species = "Tuna",
                WeightKg = 12245.45m
            });

            var semua = await vesselRepo.GetAllAsync();
            foreach (var v in semua)
                Console.WriteLine($"{v.VesselId} - {v.Name} ({v.RegNumber})");
        }
    }
}
