using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Metro_WPF_Template_App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isAppSettingsFlyoutOpen;
        private bool _isTipsFlyoutOpen;

        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> OptionsMenuItems { get; set; }

        public MainWindowViewModel()
        {
            // Top Menu Items
            MenuItems =
            [
                new MenuItem
                {
                    Text = "App Settings",
                    Icon = "Information",
                    Command = new RelayCommand((sender, e) => SidebarAppSettings_Click(sender, e))
                }
            ];

            // Bottom Menu Items
            OptionsMenuItems =
            [
                new MenuItem
                {
                    Text = "App Settings",
                    Icon = "Cog",
                    Command = new RelayCommand((sender, e) => SidebarAppSettings_Click(sender, e))
                },
                new MenuItem
                {
                    Text = "Tips",
                    Icon = "HandHeart",
                    Command = new RelayCommand((sender, e) => SidebarTips_Click(sender, e))
                }
            ];
        }

        // ============ App Settings Flyout ============

        // App Settings Button Click
        private void SidebarAppSettings_Click(object? sender, RoutedEventArgs e)
        {
            OpenAppSettingsFlyout();
        }

        // App Settings Flyout
        public bool IsAppSettingsFlyoutOpen
        {
            get => _isAppSettingsFlyoutOpen;
            set
            {
                if (_isAppSettingsFlyoutOpen != value)
                {
                    _isAppSettingsFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        // Toggle App Settings Flyout to Open (true)
        public void OpenAppSettingsFlyout()
        {
            if (IsAppSettingsFlyoutOpen)
            {
                IsAppSettingsFlyoutOpen = false;
            }
            IsAppSettingsFlyoutOpen = true;
        }

        // ============ Tips Flyout ============

        // Tips Button Click
        private void SidebarTips_Click(object? sender, RoutedEventArgs e)
        {
            OpenTipsFlyout();
        }

        // Tips Flyout
        public bool IsTipsFlyoutOpen
        {
            get => _isTipsFlyoutOpen;
            set
            {
                if (_isTipsFlyoutOpen != value)
                {
                    _isTipsFlyoutOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        // Toggle Tips Flyout to Open (true)
        public void OpenTipsFlyout()
        {
            if (IsTipsFlyoutOpen)
            {
                IsTipsFlyoutOpen = false;
            }
            IsTipsFlyoutOpen = true;
        }

        // ============ Property Changed Helper ============
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChangingUsingCustomArgs(propertyName);
        }

        private void PropertyChangingUsingCustomArgs(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Sidebar Helper Classes
    public class RelayCommand(Action<object?, RoutedEventArgs> execute, Func<object?, bool>? canExecute = null) : ICommand
    {
        private readonly Action<object?, RoutedEventArgs> _execute = execute;
        private readonly Func<object?, bool> _canExecute = canExecute ?? (param => true);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute(parameter);

        public void Execute(object? parameter)
        {
            _execute(parameter, new RoutedEventArgs());
        }
    }

    // MenuItems Model
    public class MenuItem
    {
        public string Text { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public ICommand Command { get; set; } = new RelayCommand((s, e) => { });
    }
}