using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using ThePlayground.Core.Graphics;

namespace ThePlayground.Content.Particles;

/// <summary>
///     A circular particle with "connected" outlines.
/// </summary>
[method: SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
public class Metaball(
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
    : Particle
{
    public override bool HasPartsDrawnOverParticles => true;

    public override void Update()
    {
        Position += Velocity;
        Velocity *= Deceleration;
        Velocity.Y += Gravity;
        Scale -= ScaleDecreaseOverTime;

        if (Scale < 0)
            Active = false;

        if (TileCollide)
            if (Collision.SolidCollision(Position - new Vector2(1) + Velocity, 2, 2))
            {
                Vector2 correctedVelocity = Collision.AnyCollision(Position - new Vector2(1), Velocity, 2, 2);

                if (correctedVelocity.X != Velocity.X)
                    Velocity.X *= -1;
                if (correctedVelocity.X != Velocity.Y)
                    Velocity.Y *= -1;
            }

        if (TimeInWorld >= TimeLeft)
            Active = false;

        if (Parent is not null && !Parent.active)
            Active = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture ??= ModContent.Request<Texture2D>($"{nameof(ThePlayground)}/Content/Particles/Metaball");

        Vector2 drawPos = (Position / 2).ToPoint().ToVector2() * 2 - Main.screenPosition;
        Vector2 drawOrigin = new(Texture.Width() / 2);
        Rectangle sourceRectangle = new(0, Texture.Width() * (int)Scale, Texture.Width(), Texture.Width());
        Color drawColor = OutlineColor;
        if (OutlineAffectedByLight)
            drawColor = Lighting.GetColor((Position / 16).ToPoint(), OutlineColor);

        PreDrawOutline(spriteBatch, this);
        spriteBatch.Draw(Texture.Value, drawPos, sourceRectangle, drawColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
        PostDrawOutline(spriteBatch, this);
    }

    public override void DrawAfter(SpriteBatch spriteBatch)
    {
        if (Scale > 1 && Active)
        {
            Vector2 drawPos = (Position / 2).ToPoint().ToVector2() * 2 - Main.screenPosition;
            Vector2 drawOrigin = new(Texture.Width() / 2);
            Rectangle sourceRectangle = new(0, Texture.Width() * (int)(Scale - 1), Texture.Width(), Texture.Width());
            Color drawColor = new(Color.R, Color.G, Color.B);
            if (AffectedByLight)
                drawColor = Lighting.GetColor((Position / 16).ToPoint(), Color);

            PreDraw(spriteBatch, this);
            spriteBatch.Draw(Texture.Value, drawPos, sourceRectangle, drawColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
            PostDraw(spriteBatch, this);
        }
    }

    // ReSharper disable MemberCanBePrivate.Global
    protected static Asset<Texture2D> Texture;
    protected readonly bool AffectedByLight = affectedByLight;
    public readonly float Deceleration = deceleration;
    public readonly float Gravity = gravity;
    public readonly bool OutlineAffectedByLight = outlineAffectedByLight;
    public readonly Color OutlineColor = outlineColor ?? Color.Black;
    protected float Scale = scale;

    public readonly float ScaleDecreaseOverTime = scaleDecreaseOverTime;
    public readonly bool TileCollide = tileCollide;
    public readonly float TimeLeft = timeLeft;

    public readonly Action<SpriteBatch, Particle> PreDrawOutline = preDrawOutline ?? ((_, _) => { });
    public readonly Action<SpriteBatch, Particle> PostDrawOutline = postDrawOutline ?? ((_, _) => { });
    protected readonly Action<SpriteBatch, Particle> PreDraw = preDraw ?? ((_, _) => { });
    protected readonly Action<SpriteBatch, Particle> PostDraw = postDraw ?? ((_, _) => { });

    protected readonly Entity Parent = parent;
    // ReSharper restore MemberCanBePrivate.Global
}