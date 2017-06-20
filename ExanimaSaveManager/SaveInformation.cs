using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    public sealed class SaveInformation : INotifyPropertyChanged {
        private string _characterName;
        private string _currentLevel;
        private string _gameMode;
        private string _identifier;
        private DateTime _modificationTime;

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
                OnPropertyChanged(nameof(FileName));
            }
        }

        public string Identifier {
            get => _identifier;
            set {
                _identifier = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FileName));
            }
        }

        public DateTime ModificationTime {
            get => _modificationTime;
            set {
                _modificationTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FileName));
            }
        }

        public string FileName => $"{GameMode}{Identifier}.rsg";

        public bool IsSameFile(SaveInformation other) {
            return GameMode == other.GameMode
                   && Identifier == other.Identifier;
        }

        public void UseInformationFrom(SaveInformation other) {
            CharacterName = other.CharacterName;
            GameMode = other.GameMode;
            Identifier = other.Identifier;
            ModificationTime = other.ModificationTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}