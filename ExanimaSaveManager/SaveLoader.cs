using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ExanimaSaveManager {
    public static class SaveLoader {
        public static readonly string BaseDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Exanima"
        );

        public static string ProfilePath(string profileName) => Path.Combine(ProfilesPath, profileName);
        public static readonly string ProfilesPath = Path.Combine(BaseDataPath, "ESM_Profiles");
        public static readonly Regex FilePathFormat = new Regex(@"(?<GameMode>[A-Z][a-z]*)(?<Identifier>\d+).rsg$", RegexOptions.Compiled);

        private const int CurrentLevelOffset = 0x2000;
        private const int CharacterNameOffset = 0x2040;

        public static SaveInformation Load(string filePath) {
            var match = FilePathFormat.Match(filePath);
            var file = new FileInfo(filePath);
            using (var fileStream = File.OpenRead(filePath))
            using (var stream = new BufferedStream(fileStream)) {
                stream.Position = CurrentLevelOffset;
                var level = ReadNullTerminatedString(stream);
                stream.Position = CharacterNameOffset;
                var character = ReadNullTerminatedString(stream);
                return new SaveInformation {
                    GameMode = match.Groups["GameMode"].Value,
                    CurrentLevel = level,
                    CharacterName = character,
                    Identifier = match.Groups["Identifier"].Value,
                    ModificationTime = file.LastWriteTime
                };
            }
        }

        public static void Write(SaveInformation info, string filePath) {
            using (var fileStream = File.OpenWrite(filePath))
            using (var stream = new BufferedStream(fileStream)) {
                stream.Position = CurrentLevelOffset;
                WriteNullTerminatedString(stream, info.CurrentLevel);
                stream.Position = CharacterNameOffset;
                WriteNullTerminatedString(stream, info.CharacterName);
            }
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

        private static void WriteNullTerminatedString(Stream stream, string value) {
            var bytes = Encoding.ASCII.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0);
        }
    }
}
