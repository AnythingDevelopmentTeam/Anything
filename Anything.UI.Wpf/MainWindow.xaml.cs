using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Anything.Core.Models;
using Anything.Core.Services;
using Anything.Platform.Windows;

namespace Anything.UI.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/icon.png"));

            var provider = new WindowsFileIndexProvider();
            var service = new AnythingSearchService(provider);
            DataContext = new MainViewModel(service);

            Loaded += async (_, _) =>
            {
                ApplyMica();
                await ((MainViewModel)DataContext).InitializeAsync();
            };
        }

        // ---------------- MICA ----------------

        private void ApplyMica()
        {
            if (Environment.OSVersion.Version.Build < 22000)
                return;

            var hwnd = new WindowInteropHelper(this).EnsureHandle();
            EnableDarkMode(hwnd);
            EnableMica(hwnd);
        }

        private void EnableDarkMode(IntPtr hwnd)
        {
            const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
            int dark = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
        }

        private void EnableMica(IntPtr hwnd)
        {
            const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
            const int DWMSBT_MAINWINDOW = 2;
            int mica = DWMSBT_MAINWINDOW;
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref mica, sizeof(int));
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // ---------------- ЗАГОЛОВОК ----------------

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
                return;
            }

            DragMove();
        }

        private void Minimize_Hover(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = (Brush)FindResource("TitleButtonHover");
        }

        private void Maximize_Hover(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = (Brush)FindResource("TitleButtonHover");
        }

        private void Close_Hover(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = (Brush)FindResource("TitleButtonCloseHover");
        }

        private void TitleButton_Leave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = Brushes.Transparent;
        }

        private void Minimize_Click(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // ---------------- ЛОГИКА ----------------

        private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView lv &&
                lv.SelectedItem is FileEntry entry)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(entry.Path) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cannot open file");
                }
            }
        }
    }
}
