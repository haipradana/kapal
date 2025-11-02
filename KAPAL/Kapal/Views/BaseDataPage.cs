using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    /// <summary>
    /// Base class untuk semua data management pages
    /// Menerapkan konsep inheritance untuk code reuse
    /// </summary>
    /// <typeparam name="T">Type dari model data</typeparam>
    public abstract class BaseDataPage<T> : Page where T : class
    {
        protected readonly AppState _state;
        protected List<T> _all = new();

        public BaseDataPage(AppState state)
        {
            _state = state;
        }

        /// <summary>
        /// Abstract method untuk load data - harus diimplementasikan oleh derived class
        /// </summary>
        protected abstract Task<List<T>> LoadDataFromRepositoryAsync();

        /// <summary>
        /// Abstract method untuk delete data - harus diimplementasikan oleh derived class
        /// </summary>
        protected abstract Task DeleteDataAsync(T item);

        /// <summary>
        /// Abstract method untuk mendapatkan DataGrid control
        /// </summary>
        protected abstract DataGrid GetDataGrid();

        /// <summary>
        /// Abstract method untuk mendapatkan nama item (untuk display)
        /// </summary>
        protected abstract string GetItemName(T item);

        /// <summary>
        /// Abstract method untuk mendapatkan ID item
        /// </summary>
        protected abstract int GetItemId(T item);

        /// <summary>
        /// Template method untuk load data - menggunakan polymorphism
        /// </summary>
        protected virtual async Task LoadAsync()
        {
            try
            {
                _all = await LoadDataFromRepositoryAsync() ?? new List<T>();
                var dataGrid = GetDataGrid();
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = _all;
                }
                OnDataLoaded();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Virtual method yang bisa di-override untuk custom behavior setelah data loaded
        /// </summary>
        protected virtual void OnDataLoaded()
        {
            // Default implementation - bisa di-override
        }

        /// <summary>
        /// Template method untuk delete dengan konfirmasi
        /// </summary>
        protected virtual async Task<bool> DeleteWithConfirmationAsync(T item)
        {
            if (item == null) return false;

            var itemName = GetItemName(item);
            var result = MessageBox.Show(
                $"Are you sure you want to delete '{itemName}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await DeleteDataAsync(item);
                    MessageBox.Show($"{typeof(T).Name} deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting {typeof(T).Name}: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return false;
        }
    }
}
