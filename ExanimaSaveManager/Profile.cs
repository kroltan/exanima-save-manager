﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    public sealed class Profile : INotifyPropertyChanged, IDisposable {
        private readonly SaveInformation _master;
        private readonly string _profileName;
        public SaveRepository Repository { get; }

        public IEnumerable<SaveInformation> ByModification => Repository.OrderBy(i => i.ModificationTime);

        private string MasterFilePath => Path.Combine(SaveLoader.BaseDataPath, _master.FileName);

        public Profile(SaveInformation master) {
            _master = master;
            _profileName = master.FileName;
            var path = SaveLoader.ProfilePath(_profileName);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            Repository = SaveRepository.Initialize(_profileName);
            if (Repository.Count == 0) {
                CreateBackup();
            }
            OnPropertyChanged(nameof(Repository));
            OnPropertyChanged(nameof(MasterFilePath));
        }

        public void CreateBackup() {
            var timestamp = DateTime.UtcNow.ToFileTime().ToString();
            var dummyInfo = new SaveInformation {
                GameMode = _master.GameMode,
                Identifier = timestamp
            };
            var backupPath = Path.Combine(SaveLoader.ProfilePath(_profileName), dummyInfo.FileName);
            var temp = $"{backupPath}.tmp";
            File.Copy(MasterFilePath, temp);
            File.Move(temp, backupPath);

            var file = new FileInfo(backupPath) {
                LastWriteTime = DateTime.Now
            };
            file.Refresh();
        }

        public void Restore(SaveInformation backupInfo) {
            var backupPath = Path.Combine(SaveLoader.ProfilePath(_profileName), backupInfo.FileName);
            var oldTemp = $"{MasterFilePath}_a.tmp";
            var newTemp = $"{MasterFilePath}_b.tmp";
            File.Move(MasterFilePath, oldTemp);
            File.Copy(backupPath, newTemp);
            File.Move(newTemp, MasterFilePath);
            File.Delete(oldTemp);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose() {
            Repository.Dispose();
        }
    }
}
