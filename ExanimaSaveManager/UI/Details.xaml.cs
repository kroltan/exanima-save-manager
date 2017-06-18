﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    /// <summary>
    /// Lógica interna para Details.xaml
    /// </summary>
    public partial class Details : INotifyPropertyChanged {
        private SaveInformation _master;
        private Profile _profile;
        private SaveInformation _selected;
        private readonly object _lock;

        public Details(SaveInformation master) {
            Master = master;
            _profile = new Profile(master);
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(_profile.Repository, _lock);
            InitializeComponent();
        }

        public SaveInformation Master {
            get => _master;
            private set {
                _master = value;
                OnPropertyChanged();
            }
        }

        public Profile Profile {
            get => _profile;
            set {
                _profile = value;
                OnPropertyChanged();
            }
        }

        public SaveInformation Selected {
            get => _selected;
            set {
                _selected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
            }
        }

        public bool HasSelection => _selected != null;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplySelected_Click(object sender, RoutedEventArgs e) {
            Profile.Restore(Selected);
        }

        private void BackupNow_Click(object sender, RoutedEventArgs e) {
            Profile.CreateBackup();
        }
    }
}