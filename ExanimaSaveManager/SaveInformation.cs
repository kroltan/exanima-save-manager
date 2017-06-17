using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    public class SaveInformation : INotifyPropertyChanged {
        private string _characterName;
        private string _currentLevel;
        private string _gameMode;
        private string _identifier;

        public string CharacterName {
            get => _characterName;
            set {
                _characterName = value;
                OnPropertyChanged();
            }
        }

        public string CurrentLevel {
            get => _currentLevel;
            set {
                _currentLevel = value;
                OnPropertyChanged();
            }
        }

        public string GameMode {
            get => _gameMode;
            set {
                _gameMode = value;
                OnPropertyChanged();
            }
        }

        public string Identifier {
            get => _identifier;
            set {
                _identifier = value;
                OnPropertyChanged();
            }
        }

        public bool IsSameFile(SaveInformation other) {
            return GameMode == other.GameMode
                   && Identifier == other.Identifier;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}