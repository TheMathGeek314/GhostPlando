using System;
using UnityEngine;
using ItemChanger;
using ItemChanger.Internal;

namespace GhostPlando {
    [Serializable]
    public class EmbeddedDoorSprite: ISprite {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(EmbeddedDoorSprite).Assembly, "GhostPlando.Resources.Images.");
        
        public string key;
        public EmbeddedDoorSprite(string key) {
            this.key = key;
        }

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
