using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged {
        private static readonly string BaseDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Exanima"
        );

        private static readonly string ProfilesPath = Path.Combine(BaseDataPath, "ExSaM_Profiles");

        private Visibility _profileBoxVisibility;
        public Visibility ProfileBoxVisibility {
            get { return _profileBoxVisibility; }
            set {
                _profileBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _profiles;
        public ObservableCollection<string> Profiles {
            get { return _profiles; }
            set {
                _profiles = value;
                OnPropertyChanged();
            }
        }

        public bool HasProfileSelection => profileListBox.SelectedItem != null;

        public MainWindow() {
            _profileBoxVisibility = Visibility.Collapsed;
            _profiles = new ObservableCollection<string>(new DirectoryInfo(ProfilesPath)
                .EnumerateDirectories()
                .Select(d => d.Name));
            _profiles.CollectionChanged += ProfilesChanged;

            InitializeComponent();
        }
        
        private void ProfilesChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    var newProfile = e.NewItems.Cast<string>().First();
                    Directory.CreateDirectory(Path.Combine(ProfilesPath, newProfile));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var deletedProfile = e.OldItems.Cast<string>().First();
                    Directory.Delete(Path.Combine(ProfilesPath, deletedProfile), true);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void newButton_Click(object sender, RoutedEventArgs e) {
            newProfileName.Text = "";
            ProfileBoxVisibility = Visibility.Visible;
            newProfileName.Focus();
        }

        private void newProfileName_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Escape:
                    ProfileBoxVisibility = Visibility.Collapsed;
                    break;
                case Key.Enter:
                    ProfileBoxVisibility = Visibility.Collapsed;
                    if (string.IsNullOrEmpty(newProfileName.Text)
                        || _profiles.Contains(newProfileName.Text)) {
                        return;
                    }
                    _profiles.Add(newProfileName.Text);
                    break;
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e) {
            if (!HasProfileSelection) return;
            var result = MessageBox.Show(
                $"Do you really want to delete \"{profileListBox.SelectedItem}\"?",
                "Profile deletion",
                MessageBoxButton.YesNo
            );
            if (result == MessageBoxResult.Yes) {
                _profiles.Remove((string)profileListBox.SelectedItem);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            if (!HasProfileSelection) return;
            var profile = (string) profileListBox.SelectedItem;
            foreach (var file in new DirectoryInfo(BaseDataPath).EnumerateFiles()) {
                file.CopyTo(Path.Combine(ProfilesPath, profile, file.Name), true);
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e) {
            if (!HasProfileSelection) return;
            var profile = (string)profileListBox.SelectedItem;
            foreach (var file in new DirectoryInfo(Path.Combine(ProfilesPath, profile)).EnumerateFiles()) {
                file.CopyTo(Path.Combine(BaseDataPath, file.Name), true);
            }
        }

        private void profileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            OnPropertyChanged(nameof(HasProfileSelection));
        }
    }
}
