using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using ThePlayground.Core.Graphics;
using ThePlayground.Utilities;

namespace ThePlayground.Content.Particles;

/// <summary>
///     <see cref="Metaball" /> particle that gets lighter in shade the further away it is from the parent.
/// </summary>
public class FireMetaball(
    Vector2 spawnPos,
    bool affectedByLight = false,
    float deceleration = 1f,
    float gravity = 0,
    bool outlineAffectedByLight = false,
    Color? outlineColor = null,
    float scale = 2,
    float scaleDecreaseOverTime = 0f,
    bool tileCollide = false,
    float timeLeft = 600,
    Action<SpriteBatch, Particle> preDrawOutline = null,
    Action<SpriteBatch, Particle> postDrawOutline = null,
    Action<SpriteBatch, Particle> preDraw = null,
    Action<SpriteBatch, Particle> postDraw = null,
    Entity parent = null)
    : Metaball(affectedByLight, deceleration, gravity, outlineAffectedByLight, outlineColor, scale, scaleDecreaseOverTime, tileCollide, timeLeft,
        preDrawOutline, postDrawOutline, preDraw, postDraw, parent)
{
    public override bool DrawsRenderTarget => true;
    private readonly Color[] _colors = [new Color(238, 152, 33), new Color(250, 253, 146), Color.White];

    public override void DrawAfter(SpriteBatch spriteBatch)
    {
        if (Scale > 1 && Active)
        {
            Vector2 drawPos = Position - Main.screenPosition;
            Vector2 drawOrigin = new(Texture.Width() / 2);
            Rectangle sourceRectangle = new(0, Texture.Width() * (int)(Scale - 1), Texture.Width(), Texture.Width());
            int parentDist = Parent is not null ? (int)(spawnPos.Y - Position.Y) : 0;

            Color currentColor;
            if (parentDist > Main.rand.Next(120, 140))
                currentColor = _colors[2];
            else if (parentDist > Main.rand.Next(80, 110))
                currentColor = _colors[1];
            else if (parentDist > Main.rand.Next(30, 70))
                currentColor = _colors[0];
            else
                currentColor = Color;

            Color drawColor = currentColor;

            if (AffectedByLight)
                drawColor = Lighting.GetColor((Position / 16).ToPoint(), Color);

            PreDraw(spriteBatch, this);
            spriteBatch.Draw(Texture.Value, drawPos, sourceRectangle, drawColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
            PostDraw(spriteBatch, this);
        }
    }
}

public class FireRenderTarget : ParticleRenderTarget
{
    public override void DrawToRenderTarget(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (Main.gameMenu)
            return;
        
        graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        RenderTargetBinding[] oldTargets = RenderTarget.SwapTo();

        spriteBatch.Restart(end: false, transformMatrix: Main.GameViewMatrix.EffectMatrix);
        
        foreach (Particle particle in ParticleSystem.Particles)
            if (particle is FireMetaball)
                particle.DrawAfter(spriteBatch);

        spriteBatch.End();
        graphicsDevice.SetRenderTargets(oldTargets);
    }

    public override void DrawRenderTarget(SpriteBatch spriteBatch)
    {
        spriteBatch.Restart(SpriteSortMode.Immediate);
        
        Effect effect = Graphics.GetEffect("Fire");
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects / 30);
        effect.Parameters["uLerpCoff"].SetValue(0.4f);
        effect.CurrentTechnique.Passes[0].Apply();
        
        spriteBatch.Draw(RenderTarget, new Vector2(0, 0), null, Color.White);
        
        spriteBatch.Restart();
    }
}