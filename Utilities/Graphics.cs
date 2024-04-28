using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ThePlayground.Utilities;

public static class Graphics
{
    /// <summary>
    ///     Sets the current render target to the provided one.
    /// </summary>
    /// <param name="target">The render target to swap to</param>
    /// <param name="flushColor">The color to clear the screen with. Transparent by default</param>
    /// <returns>The old render target bindings.</returns>
    public static RenderTargetBinding[] SwapTo(this RenderTarget2D target, Color? flushColor = null)
    {
        if (Main.gameMenu || Main.dedServ || target is null || Main.instance.GraphicsDevice is null || Main.spriteBatch is null)
            return null;

        RenderTargetBinding[] oldTargets = Main.graphics.GraphicsDevice.GetRenderTargets();

        Main.graphics.GraphicsDevice.SetRenderTarget(target);
        Main.graphics.GraphicsDevice.Clear(flushColor ?? Color.Transparent);

        return oldTargets;
    }

    /// <summary>
    ///     Restarts the spriteBatch with the given parameters. Use sparingly.
    /// </summary>
    /// <param name="end">Whether to end the SpriteBatch first or not.</param>
    public static void Restart(this SpriteBatch spriteBatch, SpriteSortMode spriteSortMode = SpriteSortMode.Deferred, BlendState blendState = null,
        SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null,
        Matrix? transformMatrix = null, bool end = true)
    {
        if (end)
            spriteBatch.End();

        spriteBatch.Begin(spriteSortMode, blendState ?? BlendState.AlphaBlend, samplerState ?? Main.DefaultSamplerState, depthStencilState ?? DepthStencilState.None,
            rasterizerState ?? Main.Rasterizer, effect, transformMatrix ?? Main.Transform);
    }

    /// <summary>
    ///     Returns an <see cref="Effect" /> from the provided path.
    /// </summary>
    /// <param name="name">Name of the effect file</param>
    /// <param name="path">The path to the effects folder</param>
    /// <returns></returns>
    public static Effect GetEffect(string name, string path = $"{nameof(ThePlayground)}/Assets/Effects")
    {
        return ModContent.Request<Effect>($"{path}/{name}").Value;
    }
}