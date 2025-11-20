using Kapal.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    /// <summary>
    /// VesselDataPage menggunakan polymorphism melalui IRepository interface
    /// </summary>
    public partial class VesselDataPage : Page
    {
        private readonly AppState _state;
        private List<Vessel> _all = new();

        public VesselDataPage(AppState state)
        {
            InitializeComponent();
            _state = state;
            Loaded += async (_, __) => await LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                // Menggunakan polymorphism - IRepository interface
                _all = await _state.VesselRepo.GetAllAsync() ?? new List<Vessel>();
                dgVessels.ItemsSource = _all;
                _state.SelectedVessel = null;
                UpdateButtonStates();

                // Disable CRUD buttons for guest
                ApplyRoleBasedUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vessels: {ex.Message}", "Error",
      MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyRoleBasedUI()
        {
            if (!_state.IsAdmin)
            {
                // Find Add button from XAML
                var addButton = FindName("btnAdd") as Button;
                if (addButton != null)
                {
                    addButton.IsEnabled = false;
                    addButton.Opacity = 0.5;
                }

                btnEdit.IsEnabled = false;
                btnEdit.Opacity = 0.5;
                btnDelete.IsEnabled = false;
                btnDelete.Opacity = 0.5;
            }
        }

        private void DgVessels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _state.SelectedVessel = dgVessels.SelectedItem as Vessel;
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = _state.SelectedVessel != null;

            // Only enable if admin and has selection
            if (_state.IsAdmin)
            {
                btnEdit.IsEnabled = hasSelection;
                btnDelete.IsEnabled = hasSelection;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!_state.IsAdmin) return;

            var frame = Application.Current.MainWindow.FindName("RootFrame") as Frame;
            frame?.Navigate(new AddVesselPage(_state));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!_state.IsAdmin || _state.SelectedVessel == null) return;

            // Create edit dialog window with Material Design styling
            var editWindow = new Window
            {
                Title = $"Edit Vessel - {_state.SelectedVessel.Name}",
                Width = 550,
                Height = 620,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                ResizeMode = ResizeMode.NoResize,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(248, 250, 252)), // #F8FAFC
                WindowStyle = WindowStyle.SingleBorderWindow
            };

            var mainStackPanel = new StackPanel
            {
                Margin = new Thickness(32)
            };

            // Title
            var titleBlock = new TextBlock
            {
                Text = "Edit Vessel Details",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 23, 42)), // #0F172A
                Margin = new Thickness(0, 0, 0, 8)
            };

            var subtitleBlock = new TextBlock
            {
                Text = "Update vessel information below",
                FontSize = 13,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 116, 139)), // #64748B
                Margin = new Thickness(0, 0, 0, 24)
            };

            mainStackPanel.Children.Add(titleBlock);
            mainStackPanel.Children.Add(subtitleBlock);

            // Form fields with proper styling
            var nameField = CreateFormField("Vessel Name", _state.SelectedVessel.Name);
            var regField = CreateFormField("Registration Number", _state.SelectedVessel.RegNumber);
            var gearField = CreateFormField("Fishing Gear", _state.SelectedVessel.Gear);
            var ownerField = CreateFormField("Owner Name", _state.SelectedVessel.OwnerName);

            mainStackPanel.Children.Add(nameField.Container);
            mainStackPanel.Children.Add(regField.Container);
            mainStackPanel.Children.Add(gearField.Container);
            mainStackPanel.Children.Add(ownerField.Container);

            // Button panel
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 32, 0, 0)
            };

            // Save button - matching "Add Vessel" blue (#3B82F6)
            var saveButton = CreateButton("Save", "#3B82F6", "#2563EB", "#FFFFFF");
            // Cancel button - matching "Delete" gray (#E5E7EB)
            var cancelButton = CreateButton("Cancel", "#E5E7EB", "#D1D5DB", "#1F2937");

            saveButton.Click += async (s, args) =>
            {
                await HandleSaveAsync(
                    nameField.TextBox.Text,
                    regField.TextBox.Text,
                    gearField.TextBox.Text,
                    ownerField.TextBox.Text,
                    editWindow
                );
            };
            cancelButton.Click += (s, args) => editWindow.Close();

            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(cancelButton);

            mainStackPanel.Children.Add(buttonPanel);

            editWindow.Content = mainStackPanel;
            editWindow.ShowDialog();
        }

        private (StackPanel Container, TextBox TextBox) CreateFormField(string label, string value)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 20)
            };

            var labelBlock = new TextBlock
            {
                Text = label,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 41, 59)), // #1E293B
                Margin = new Thickness(0, 0, 0, 8)
            };

            var textBox = new TextBox
            {
                Text = value ?? "",
                Height = 44,
                Padding = new Thickness(12, 10, 12, 10),
                FontSize = 13,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 23, 42)), // #0F172A
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)), // White
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(226, 232, 240)), // #E2E8F0
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add rounded corners to TextBox using CornerRadius
            var border = new Border
            {
                CornerRadius = new CornerRadius(8),
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(226, 232, 240)), // #E2E8F0
                BorderThickness = new Thickness(1),
                Child = textBox,
                Padding = new Thickness(0)
            };

            // Add focus styling
            textBox.GotFocus += (s, e) =>
            {
                border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(59, 130, 246)); // #3B82F6
                border.BorderThickness = new Thickness(2);
            };

            textBox.LostFocus += (s, e) =>
            {
                border.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(226, 232, 240));
                border.BorderThickness = new Thickness(1);
            };

            container.Children.Add(labelBlock);
            container.Children.Add(border);

            return (container, textBox);
        }

        private Button CreateButton(string text, string backgroundColor, string hoverColor, string foreground = "#FFFFFF")
        {
            var button = new Button
            {
                Content = text,
                Width = 120,
                Height = 44,
                Margin = new Thickness(8, 0, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush(ConvertHexToColor(foreground)),
                Background = new System.Windows.Media.SolidColorBrush(ConvertHexToColor(backgroundColor)),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Template = GetButtonTemplate()
            };

            button.MouseEnter += (s, e) =>
            {
                var btn = s as Button;
                if (btn != null)
                {
                    btn.Background = new System.Windows.Media.SolidColorBrush(ConvertHexToColor(hoverColor));
                    // Add scaling effect for better feedback
                    btn.RenderTransform = new System.Windows.Media.ScaleTransform(1.02, 1.02);
                    btn.RenderTransformOrigin = new Point(0.5, 0.5);
                }
            };

            button.MouseLeave += (s, e) =>
            {
                var btn = s as Button;
                if (btn != null)
                {
                    btn.Background = new System.Windows.Media.SolidColorBrush(ConvertHexToColor(backgroundColor));
                    // Reset scale
                    btn.RenderTransform = new System.Windows.Media.ScaleTransform(1, 1);
                }
            };

            return button;
        }

        private ControlTemplate GetButtonTemplate()
        {
            // Create a custom button template with rounded corners
            var template = new ControlTemplate(typeof(Button));
            
        var border = new FrameworkElementFactory(typeof(Border));
         border.SetValue(Border.BackgroundProperty, new System.Windows.TemplateBindingExtension(Button.BackgroundProperty));
        border.SetValue(Border.BorderBrushProperty, new System.Windows.TemplateBindingExtension(Button.BorderBrushProperty));
     border.SetValue(Border.BorderThicknessProperty, new System.Windows.TemplateBindingExtension(Button.BorderThicknessProperty));
       border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8)); // Rounded corners

         var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
 contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
    contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

    border.AppendChild(contentPresenter);
   template.VisualTree = border;

        return template;
        }

        private System.Windows.Media.Color ConvertHexToColor(string hexColor)
        {
            hexColor = hexColor.Replace("#", "");
            return System.Windows.Media.Color.FromRgb(
                byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)
            );
        }

        private async Task HandleSaveAsync(string name, string regNumber, string gear, string ownerName, Window editWindow)
        {
            if (_state.SelectedVessel == null) return;

            // Validation
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(regNumber))
            {
                MessageBox.Show("Name and Registration Number are required!", "Validation Error",
      MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Update the vessel object
                _state.SelectedVessel.Name = name;
                _state.SelectedVessel.RegNumber = regNumber;
                _state.SelectedVessel.Gear = gear;
                _state.SelectedVessel.OwnerName = ownerName;

                // Save to database using polymorphism
                await _state.VesselRepo.UpdateAsync(_state.SelectedVessel);

                MessageBox.Show("Vessel updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                editWindow.Close();
                await LoadAsync(); // Refresh the list
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating vessel: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!_state.IsAdmin || _state.SelectedVessel == null) return;

            var result = MessageBox.Show(
   $"Are you sure you want to delete vessel '{_state.SelectedVessel.Name}'?",
   "Confirm Delete",
                MessageBoxButton.YesNo,
     MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Menggunakan polymorphism - DeleteAsync dari IRepository
                    await _state.VesselRepo.DeleteAsync(_state.SelectedVessel.VesselId);
                    MessageBox.Show("Vessel deleted successfully!", "Success",
      MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting vessel: {ex.Message}", "Error",
    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}