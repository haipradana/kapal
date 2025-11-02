using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Kapal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Suppress XAML binding errors di Output window (hanya untuk non-critical errors)
         // Ini akan mengurangi "noise" tapi tetap menampilkan error penting
       PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
 }
    }
}
