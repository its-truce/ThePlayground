using Terraria.ModLoader;

namespace ThePlayground.Core.Arcana;

public class KeybindSystem : ModSystem
{
    public static ModKeybind Slot1 { get; private set; }
    public static ModKeybind Slot2 { get; private set; }
    public static ModKeybind Slot3 { get; private set; }
    public static ModKeybind Slot4 { get; private set; }


    public override void Load()
    {
        Slot1 = KeybindLoader.RegisterKeybind(Mod, "Arcana (Slot 1)", "U");
        Slot2 = KeybindLoader.RegisterKeybind(Mod, "Arcana (Slot 2)", "K");
        Slot3 = KeybindLoader.RegisterKeybind(Mod, "Arcana (Slot 3)", "L");
        Slot4 = KeybindLoader.RegisterKeybind(Mod, "Arcana (Slot 4)", "I");
    }

    public override void Unload()
    {
        Slot1 = null;
        Slot2 = null;
        Slot3 = null;
        Slot4 = null;
    }
}