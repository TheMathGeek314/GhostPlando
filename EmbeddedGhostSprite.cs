using System;
using UnityEngine;
using ItemChanger;
using ItemChanger.Internal;

namespace GhostPlando {
    [Serializable]
    public class EmbeddedGhostSprite: ISprite {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(EmbeddedGhostSprite).Assembly, "GhostPlando.Resources.Images.");
        
        public string key;
        public EmbeddedGhostSprite(string key) {
            this.key = key;
        }

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
