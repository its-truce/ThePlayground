using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThePlayground.Content.Dusts;
using ThePlayground.Content.Particles;
using ThePlayground.Core.Graphics;
using ThePlayground.Utilities;

namespace ThePlayground.Content.Projectiles;

public class WaterTest : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 900;
        Projectile.damage = 1;
    }

    public override void AI()
    {
        if (Projectile.Distance(Main.MouseWorld) > 6)
        {
            const int timeLeft = 300;
            float lerpCoff = (float)Easings.EaseInOutSine((timeLeft - Projectile.timeLeft) / (float)timeLeft);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * 14, lerpCoff);
        }
        else
        {
            Projectile.velocity = Vector2.Zero;
        }

        if (Main.rand.NextBool(10))
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<WaterDrops>(), Projectile.velocity.X / 5 * -1,
                Projectile.velocity.Y / 5 * -1, default, new Color(76, Main.rand.Next(100, 200), 255), Main.rand.NextFloat(2f, 4f));
            dust.noGravity = true;
        }


        Lighting.AddLight(Projectile.Center, new Color(76, 120, 255, 0).ToVector3());

        Owner.ChangeDir(Projectile.Center.X > Owner.MountedCenter.X ? 1 : -1);
        float handRot = Owner.DirectionTo(Projectile.Center).ToRotation() - MathF.PI / 2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, handRot);
        Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, handRot);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color color = new(76, 150, 255);

        // scaleDecrease controls length of the trail
        WaterMetaball metaball = new(outlineColor: Color.Lerp(Color.White with { A = 0 }, color, Main.masterColor / 4), scale: 7, timeLeft: 10,
            parent: Projectile, scaleDecreaseOverTime: 0.5f);
        ParticleSystem.AddParticle(metaball, Projectile.Center + Projectile.velocity, Vector2.Zero, color);

        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.CornflowerBlue with { A = 0 }, 0,
            texture.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None);

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Splash(target);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Splash(Projectile);
        return base.OnTileCollide(oldVelocity);
    }

    private void Splash(Entity entity)
    {
        int randDust = Main.rand.Next(8, 15);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, entity.width, entity.height, ModContent.DustType<WaterDrops>(), 0f, 0f, default,
                new Color(76, Main.rand.Next(100, 200), 255), Main.rand.NextFloat(2f, 4f));

            dust.velocity *= 6.4f;
            dust.velocity.Y -= 1f;
            dust.velocity += Projectile.velocity;
            dust.noGravity = true;
        }

        SoundID.Splash.Play(entity.Center, pitchVariance: 0.4f, maxInstances: -1);
    }
}