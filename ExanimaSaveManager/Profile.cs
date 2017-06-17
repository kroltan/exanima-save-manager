using System.IO;

namespace ExanimaSaveManager {
    public class Profile {
        private SaveRepository _repository;

        public Profile(SaveInformation master) {
            var profile = $"{master.CharacterName}_{master.GameMode}";
            var path = SaveLoader.ProfilePath(profile);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
                _repository = SaveRepository.Initialize(profile);
            }
        }
    }
}
