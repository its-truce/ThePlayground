using Terraria.ModLoader;
using ThePlayground.Core;

namespace ThePlayground;

public class ThePlayground : Mod
{
    public static ThePlayground Instance => ModContent.GetInstance<ThePlayground>();

    public override void Load()
    {
        ShaderLoader.Load();
    }
}