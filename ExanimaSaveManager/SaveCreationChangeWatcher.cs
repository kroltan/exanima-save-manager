using System;
using System.IO;
using System.Timers;

namespace ExanimaSaveManager {
    public class SaveCreationChangeWatcher : IDisposable {
        private readonly FileSystemWatcher _watcher;
        private const int RetryMilliseconds = 10;
        private const int RetryMaxCount = 5;

        public event Action<string> GameWrittenSave;

        public SaveCreationChangeWatcher(string path) {
            _watcher = new FileSystemWatcher {
                IncludeSubdirectories = false,
                Path = path,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += OnChange;
            _watcher.Renamed += OnChange;
            _watcher.Created += OnChange;
            _watcher.EnableRaisingEvents = true;
        }

        private bool IsAvailiable(string filePath) {
            try {
                using (File.OpenRead(filePath)) {
                    return true;
                }
            } catch (IOException) {
                return false;
            }
        }

        private void OnChange(object sender, FileSystemEventArgs e) {
            if (!SaveLoader.FilePathFormat.IsMatch(e.FullPath)) {
                return;
            }
            var timer = new Timer {
                Interval = RetryMilliseconds,
                AutoReset = true
            };
            var tries = 0;
            timer.Elapsed += (o, args) => {
                if (tries >= RetryMaxCount) {
                    throw new IOException($"File '{e.FullPath}' took to long to release!");
                }
                tries++;
                if (!IsAvailiable(e.FullPath)) return;
                GameWrittenSave?.Invoke(e.FullPath);
                timer.Stop();
            };
            timer.Start();
        }

        public void Dispose() {
            _watcher?.Dispose();
        }
    }
}
