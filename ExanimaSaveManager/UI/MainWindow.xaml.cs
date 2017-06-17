using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged {
        private SaveRepository _saves = SaveRepository.Initialize(null);
        private SaveInformation _selected;
        private readonly object _lock;

        public MainWindow() {
            InitializeComponent();
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(_saves, _lock);
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
            }
        }

        public bool HasSelection => Selected != null;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void History_Click(object sender, System.Windows.RoutedEventArgs e) {
            var history = new VersionHistory(Selected);
            history.ShowDialog();
        }

        private void Revert_Click(object sender, System.Windows.RoutedEventArgs e) {

        }
    }
}
