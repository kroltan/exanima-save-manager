using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExanimaSaveManager.Annotations;

namespace ExanimaSaveManager {
    public class SaveRepository : ReadOnlyObservableCollection<SaveInformation>, IDisposable {
        private readonly ObservableCollection<SaveInformation> _list;
        private readonly SaveWatcher _watcher;

        private SaveRepository([NotNull] ObservableCollection<SaveInformation> list, string profile = null) : base(list) {
            _list = list;
            _watcher = new SaveWatcher(profile);
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
            _watcher.Dispose();
        }

        public static SaveRepository Initialize(string profile) {
            var repo = new SaveRepository(new ObservableCollection<SaveInformation>(), profile);

            var existing = Directory.GetFiles(SaveLoader.BaseDataPath)
                .Where(f => SaveLoader.FilePathFormat.IsMatch(f))
                .Select(SaveLoader.Load);

            foreach (var info in existing) {
                repo._list.Add(info);
            }

            return repo;
        }
    }
}
