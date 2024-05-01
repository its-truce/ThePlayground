using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ThePlayground.Core;

public class ShaderLoader
{
    /// <summary>
    ///     Loads all the compiled shaders present in the mod (extension xnb or fxc) using <see cref="LoadShader" />. You don't
    ///     need to check for <see cref="Main.dedServ" />.
    /// </summary>
    public static void Load()
    {
        if (Main.dedServ)
            return;

        MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance)?.GetGetMethod(true);

        TmodFile file = (TmodFile)info?.Invoke(ThePlayground.Instance, null);
        IEnumerable<TmodFile.FileEntry> shaders = file?.Where(n => n.Name.Contains("Effects/") && (n.Name.EndsWith(".xnb") || n.Name.EndsWith(".fxc")));

        if (shaders is not null)
            foreach (TmodFile.FileEntry entry in shaders)
            {
                string name = entry.Name.Replace(".xnb", "").Replace(".fxc", "").Replace("Assets/Effects/", "");
                string path = entry.Name.Replace(".xnb", "").Replace(".fxc", "");
                LoadShader(name, path);
            }
    }

    /// <summary>
    ///     Loads a shader using the given name and path.
    /// </summary>
    /// <param name="name">The name of the shader.</param>
    /// <param name="path">Path to the shader. Should not include extensions.</param>
    private static void LoadShader(string name, string path)
    {
        Asset<Effect> shader = ThePlayground.Instance.Assets.Request<Effect>(path);
        Filters.Scene[name] = new Filter(new ScreenShaderData(shader, name + "Pass"), EffectPriority.High);
        Filters.Scene[name].Load();
    }
}