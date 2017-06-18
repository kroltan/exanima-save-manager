using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace ExanimaSaveManager {
    public class PeriodicBackup : IDisposable {
        private readonly SaveRepository _repository;
        private readonly Timer _timer;
        private readonly List<SaveInformation> _pendingBackups;

        private PeriodicBackup(SaveRepository repository, TimeSpan aggregationTime) {
            _repository = repository;
            _pendingBackups = new List<SaveInformation>();
            _timer = new Timer(aggregationTime.TotalMilliseconds);
            _timer.Elapsed += DoPendingBackups;
            ((INotifyCollectionChanged) _repository).CollectionChanged += OnCollectionChanged;
        }

        private void DoPendingBackups(object sender, ElapsedEventArgs e) {
            var uniqueInfo = _pendingBackups
                .GroupBy(i => i.FileName)
                .Select(g => g.Last());

            foreach (var info in uniqueInfo) {
                using (var profile = new Profile(info)) {
                    profile.CreateBackup();
                }
            }

            _pendingBackups.Clear();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            if (
                args.Action != NotifyCollectionChangedAction.Add 
                && args.Action != NotifyCollectionChangedAction.Move
                && args.Action != NotifyCollectionChangedAction.Replace
            ) {
                return;
            }
            _timer.Stop();
            _pendingBackups.AddRange(args.NewItems.Cast<SaveInformation>());
            _timer.Start();
        }

        public void Dispose() {
            ((INotifyCollectionChanged) _repository).CollectionChanged -= OnCollectionChanged;
        }

        public static PeriodicBackup Start(SaveRepository repository, TimeSpan aggregationTime) {
            return new PeriodicBackup(repository, aggregationTime);
        }
    }
}
