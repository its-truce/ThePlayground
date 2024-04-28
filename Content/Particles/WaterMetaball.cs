using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ThePlayground.Core.Graphics;
using ThePlayground.Utilities;

namespace ThePlayground.Content.Particles;

/// <summary>
///     <see cref="Metaball" /> particle that gets more blue the further away it is from the parent.
/// </summary>
public class WaterMetaball(
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

    public override void DrawAfter(SpriteBatch spriteBatch)
    {
        if (Scale > 1 && Active)
        {
            Vector2 drawPos = Position - Main.screenPosition;
            Vector2 drawOrigin = new(Texture.Width() / 2);
            Rectangle sourceRectangle = new(0, Texture.Width() * (int)(Scale - 1), Texture.Width(), Texture.Width());
            Color drawColor = new(Color.R, Color.G, Color.B)
            {
                G = Parent is not null ? (byte)(Color.G - (int)(Parent.Distance(Position) * 1.4f)) : (byte)Main.rand.Next(100, 191)
            };

            if (AffectedByLight)
                drawColor = Lighting.GetColor((Position / 16).ToPoint(), Color);

            PreDraw(spriteBatch, this);
            spriteBatch.Draw(Texture.Value, drawPos, sourceRectangle, drawColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
            PostDraw(spriteBatch, this);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture ??= ModContent.Request<Texture2D>($"{nameof(ThePlayground)}/Content/Particles/Metaball");

        Vector2 drawPos = Position - Main.screenPosition - (Parent?.Distance(Position) < 20 && Parent is not null ? Parent.velocity : Vector2.Zero);
        Vector2 drawOrigin = new(Texture.Width() / 2);
        Rectangle sourceRectangle = new(0, Texture.Width() * (int)Scale, Texture.Width(), Texture.Width());
        Color drawColor = OutlineColor;
        if (OutlineAffectedByLight)
            drawColor = Lighting.GetColor((Position / 16).ToPoint(), OutlineColor);

        PreDrawOutline(spriteBatch, this);
        spriteBatch.Draw(Texture.Value, drawPos, sourceRectangle, drawColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
        PostDrawOutline(spriteBatch, this);
    }
}

public class WaterRenderTarget : ParticleRenderTarget
{
    public override void DrawToRenderTarget(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (Main.gameMenu)
            return;

        bool needToDrawRenderTarget = false;

        foreach (Particle particle in ParticleSystem.Particles)
            if (particle is WaterMetaball)
                needToDrawRenderTarget = true;

        if (needToDrawRenderTarget)
        {
            graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            RenderTargetBinding[] oldTargets = RenderTarget.SwapTo();

            spriteBatch.Restart(end: false, transformMatrix: Main.GameViewMatrix.EffectMatrix);

            foreach (Particle particle in ParticleSystem.Particles)
                if (particle is WaterMetaball)
                    particle.DrawAfter(spriteBatch);

            spriteBatch.End();
            graphicsDevice.SetRenderTargets(oldTargets);
        }
    }

    public override void DrawRenderTarget(SpriteBatch spriteBatch)
    {
        spriteBatch.Restart(SpriteSortMode.Immediate);

        Effect effect = Graphics.GetEffect("Foam");
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects / 5);
        effect.Parameters["uLerpCoff"].SetValue(0.4f);
        effect.CurrentTechnique.Passes[0].Apply();
        spriteBatch.Draw(RenderTarget, new Vector2(0, 0), null, Color.White);
        
        spriteBatch.Restart();
    }
}