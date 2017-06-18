using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    public class SaveRepository : ReadOnlyObservableCollection<SaveInformation>, IDisposable {
        private static SaveRepository _master;
        public static SaveRepository Master => Initialize(null);

        private readonly ObservableCollection<SaveInformation> _list;
        private readonly SaveCreationChangeWatcher _watcher;

        private SaveRepository([NotNull] ObservableCollection<SaveInformation> list, string path = null) : base(list) {
            _list = list;
            _watcher = new SaveCreationChangeWatcher(path);
            _watcher.GameWrittenSave += file => {
                var save = SaveLoader.Load(file);
                AddPotentiallyExistingSave(save);
            };
        }

        private void AddPotentiallyExistingSave(SaveInformation save) {
            for (var index = 0; index < _list.Count; index++) {
                if (!save.IsSameFile(_list[index])) {
                    continue;
                }

                _list[index] = save;
                return;
            }
            _list.Add(save);
        }

        public void Dispose() {
            if (this == _master) {
                return;
            }
            _watcher.Dispose();
        }

        public static SaveRepository Initialize(string profile) {
            var path = SaveLoader.BaseDataPath;
            if (profile == null) {
                if (_master != null) {
                    return _master;
                }
            } else {
                path = SaveLoader.ProfilePath(profile);
            }
            var repo = new SaveRepository(new ObservableCollection<SaveInformation>(), path);

            var existing = Directory.GetFiles(path)
                .Where(f => SaveLoader.FilePathFormat.IsMatch(f))
                .Select(SaveLoader.Load);

            foreach (var info in existing) {
                repo._list.Add(info);
            }

            if (profile == null) {
                _master = repo;
            }
            return repo;
        }
    }
}
