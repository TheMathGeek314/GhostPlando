using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GhostPlando {
#pragma warning disable CS0649
    internal class GhostDoorData {
        public string bossName;
        public string languageKey;
        public string locationName;
        public JsonDoorTransform transform;
    }

    internal class JsonDoorTransform {
        public float x;
        public float y;
        public bool faceLeft;
    }
#pragma warning restore CS0649

    internal class ParseJson {
        private readonly string _jsonFilePath;
        private readonly Stream _jsonStream;
        private bool isPath;

        public ParseJson(string jsonFilePath) {
            _jsonFilePath = jsonFilePath;
            isPath = true;
        }

        public ParseJson(Stream jsonStream) {
            _jsonStream = jsonStream;
            isPath = false;
        }

        public List<T> parseFile<T>() {
            if(isPath) {
                using StreamReader reader = new(_jsonFilePath);
                var json = reader.ReadToEnd();
                List<T> values = JsonConvert.DeserializeObject<List<T>>(json);
                return values;
            }
            else {
                using StreamReader reader = new(_jsonStream);
                var json = reader.ReadToEnd();
                List<T> values = JsonConvert.DeserializeObject<List<T>>(json);
                return values;
            }
        }
    }
}
