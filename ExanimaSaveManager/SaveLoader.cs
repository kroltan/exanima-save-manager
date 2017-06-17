using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ExanimaSaveManager {
    public class SaveLoader {
        public static readonly string BaseDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Exanima"
        );

        public static string ProfilePath(string profileName) => Path.Combine(ProfilesPath, profileName);
        public static readonly string ProfilesPath = Path.Combine(BaseDataPath, "ESM_Profiles");
        public static readonly Regex FilePathFormat = new Regex(@"(?<GameMode>[A-Z][a-z]*)(?<Identifier>\d{3}).rsg$", RegexOptions.Compiled);

        private const int CurrentLevelOffset = 0x2000;
        private const int CharacterNameOffset = 0x2040;

        public static SaveInformation Load(string filePath) {
            var match = FilePathFormat.Match(filePath);
            var gameMode = match.Groups["GameMode"].Value;
            var id = match.Groups["Identifier"].Value;
            using (var stream = new BufferedStream(File.OpenRead(filePath))) {
                var level = ReadNullTerminatedStringAt(stream, CurrentLevelOffset);
                var character = ReadNullTerminatedStringAt(stream, CharacterNameOffset);
                return new SaveInformation {
                    GameMode = gameMode,
                    CurrentLevel = level,
                    CharacterName = character,
                    Identifier = id
                };
            }
        }

        private static string ReadNullTerminatedStringAt(Stream stream, long position) {
            stream.Position = position;
            return ReadNullTerminatedString(stream);
        }

        private static string ReadNullTerminatedString(Stream stream) {
            var bytes = new byte[32];
            byte read;
            var i = 0;
            while ((read = (byte) stream.ReadByte()) != 0) {
                bytes[i] = read;
                ++i;
            }
            return Encoding.ASCII.GetString(bytes, 0, i);
        }
    }
}
