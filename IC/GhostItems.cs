using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace GhostPlando {
    public class ShadelessItem: VoidItem {
        public ShadelessItem() {
            name = "Ghost_Shadeless";
            InteropTag tag = RandoInterop.AddTag(this);
            tag.Properties["PinSprite"] = new EmbeddedGhostSprite("Shadeless");
            UIDef = new MsgUIDef {
                name = new BoxedString("Shadeless"),
                shopDesc = new BoxedString("You weren't supposed to find this."),
                sprite = new EmbeddedGhostSprite("Shadeless")
            };
        }
    }

    public class IncorporealItem: VoidItem {
        public IncorporealItem() {
            name = "Ghost_Incorporeal";
            InteropTag tag = RandoInterop.AddTag(this);
            tag.Properties["PinSprite"] = new EmbeddedGhostSprite("Incorporeal");
            UIDef = new MsgUIDef {
                name = new BoxedString("Incorporeal"),
                shopDesc = new BoxedString("This text should never appear."),
                sprite = new EmbeddedGhostSprite("Incorporeal")
            };
        }
    }

    public class ImpossibleItem: VoidItem {
        public ImpossibleItem() {
            name = "Ghost_Impossible";
            InteropTag tag = RandoInterop.AddTag(this);
            UIDef = new MsgUIDef {
                name = new BoxedString("Impossible"),
                shopDesc = new BoxedString("This item only exists to allow rando to generate with unreachable locations."),
                sprite = new ItemChangerSprite("ShopIcons.")
            };
        }
    }
}
