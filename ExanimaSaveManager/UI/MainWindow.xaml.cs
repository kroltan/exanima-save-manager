using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged {
        private SaveRepository _saves = SaveRepository.Master;
        private SaveInformation _selected;
        private readonly object _lock;

        public MainWindow() {
            InitializeComponent();
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(_saves, _lock);
            PeriodicBackup.Start(_saves, TimeSpan.FromSeconds(1));
        }

        public SaveRepository Saves {
            get => _saves;
            set {
                _saves = value;
                OnPropertyChanged();
            }
        }

        public SaveInformation Selected {
            get => _selected;
            set {
                _selected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
                OnPropertyChanged(nameof(SelectionCanRevert));
            }
        }

        public bool HasSelection => Selected != null;

        public bool SelectionCanRevert {
            get {
                if (Selected == null) return false;
                var profile = new Profile(Selected);
                return profile.Repository.Count > 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void History_Click(object sender, System.Windows.RoutedEventArgs e) {
            var history = new Details(Selected);
            history.ShowDialog();
        }

        private void Revert_Click(object sender, System.Windows.RoutedEventArgs e) {
            using (var profile = new Profile(Selected)) {
                var versions = profile.ByModification.ToArray();
                if (versions.Length < 2) {
                    return;
                }
                profile.Restore(versions[versions.Length - 2]);
            }
        }
    }
}
