using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ThePlayground.Content.Particles;

namespace ThePlayground.Core.Graphics;

/// <summary>
///     A class with a <see cref="RenderTarget2D" /> field that is initiated on <see cref="Load" /> and recreated when the screen is
///     resized. It is disposed during <see cref="Unload" />.
///     Used for drawing <see cref="Particle" />s with render targets.
/// </summary>
public abstract class ParticleRenderTarget
{
    /// <summary>
    ///     The <see cref="RenderTarget2D" /> associated with the <see cref="ParticleRenderTarget" />.
    /// </summary>
    public RenderTarget2D RenderTarget;

    /// <summary>
    ///     Override this to specify what and how particles should be drawn. Called in a <see cref="On_Main.CheckMonoliths" />
    ///     detour.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch" /> to use for drawing.</param>
    /// <param name="graphicsDevice">The <see cref="GraphicsDevice" /> to set the target to.</param>
    public virtual void DrawToRenderTarget(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
    }

    /// <summary>
    ///     Override this to specify how the associated <see cref="RenderTarget2D" /> should be drawn. Called in a
    ///     <see cref="On_Main.DrawInfernoRings" /> detour.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void DrawRenderTarget(SpriteBatch spriteBatch)
    {
    }
}

public class ParitcleRenderTargetLoader : ILoadable
{
    private readonly List<ParticleRenderTarget> _renderTargets = [new FireRenderTarget(), new WaterRenderTarget()];
    
    public void Load(Mod mod)
    {
        Main.QueueMainThreadAction(() =>
        {
            foreach (ParticleRenderTarget particleRenderTarget in _renderTargets)
                particleRenderTarget.RenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        });

        On_Main.CheckMonoliths += orig =>
        {
            orig.Invoke();

            foreach (ParticleRenderTarget particleRenderTarget in _renderTargets)
                particleRenderTarget.DrawToRenderTarget(Main.spriteBatch, Main.graphics.GraphicsDevice);
        };
        
        On_Main.DrawInfernoRings += (orig, self) =>
        {
            orig.Invoke(self);
            
            foreach (ParticleRenderTarget particleRenderTarget in _renderTargets)
                particleRenderTarget.DrawRenderTarget(Main.spriteBatch);
        };

        Main.OnResolutionChanged += UpdateTargetSize;
    }

    private void UpdateTargetSize(Vector2 screenSize)
    {
        foreach (ParticleRenderTarget particleRenderTarget in _renderTargets)
        {
            particleRenderTarget.RenderTarget?.Dispose();
            particleRenderTarget.RenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)screenSize.X, (int)screenSize.Y);
        }
    }

    public void Unload()
    {
        Main.QueueMainThreadAction(() =>
        {
            foreach (ParticleRenderTarget particleRenderTarget in _renderTargets)
                particleRenderTarget.RenderTarget?.Dispose();
        });
        
        Main.OnResolutionChanged -= UpdateTargetSize;
    }
}