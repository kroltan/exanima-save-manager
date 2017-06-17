using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    /// <summary>
    /// Lógica interna para VersionHistory.xaml
    /// </summary>
    public partial class VersionHistory : INotifyPropertyChanged {
        private SaveInformation _master;
        private Profile _profile;

        public VersionHistory(SaveInformation master) {
            InitializeComponent();
            Master = master;
            _profile = new Profile(master);
        }

        public SaveInformation Master {
            get => _master;
            private set {
                _master = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplySelected_Click(object sender, System.Windows.RoutedEventArgs e) { }
    }
}