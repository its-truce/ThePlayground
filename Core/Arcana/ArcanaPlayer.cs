using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace ThePlayground.Core.Arcana;

public class ArcanaPlayer : ModPlayer
{
    private readonly ModKeybind[] _keybinds = [KeybindSystem.Slot1, KeybindSystem.Slot2, KeybindSystem.Slot3, KeybindSystem.Slot4];
    public ModArcana[] Arcana = new ModArcana[4];

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        for (int i = 0; i < _keybinds.Length; i++)
        {
            ModKeybind keybind = _keybinds[i];
            
            if (keybind.JustPressed)
            {
                bool success = Arcana[i].Activate();
                
                if (!success)
                    Main.NewText("cooldown not over!");
            }
        }
    }

    public override void PostUpdateMiscEffects()
    {
        foreach (ModArcana arcana in Arcana.Where(t => t.Cooldown > 0))
        {
            arcana.Cooldown--;
        }
    }
}