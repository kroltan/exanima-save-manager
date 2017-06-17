using System;
using System.IO;

namespace ExanimaSaveManager {
    public class SaveWatcher : IDisposable {
        private readonly FileSystemWatcher _watcher;

        public event Action<string> GameWrittenSave;

        public SaveWatcher(string profile = null) {
            var path = SaveLoader.BaseDataPath;
            if (profile != null) {
                path = SaveLoader.ProfilePath(profile);
            }

            _watcher = new FileSystemWatcher {
                IncludeSubdirectories = false,
                Path = path,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += OnChange;
            _watcher.Created += OnChange;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnChange(object sender, FileSystemEventArgs e) {
            if (SaveLoader.FilePathFormat.IsMatch(e.FullPath)) {
                GameWrittenSave?.Invoke(e.FullPath);
            }
        }

        public void Dispose() {
            _watcher?.Dispose();
        }
    }
}
