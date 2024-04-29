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

public class RevolvingSpouts : ModProjectile
{
    public override string Texture => $"{nameof(ThePlayground)}/Content/Projectiles/WaterTest";
    private Player Owner => Main.player[Projectile.owner];
    private ref float RotationOffset => ref Projectile.ai[0];
    private ref float RevolvingCheck => ref Projectile.ai[2];
    private bool Revolving => RevolvingCheck == 0;

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 900;
    }

    public override void AI()
    {
        if (Revolving)
        {
            RotationOffset += MathHelper.ToRadians(5);
            if (RotationOffset >= MathHelper.ToRadians(360))
                RotationOffset = 0;
            
            const int dist = 55;
            Vector2 rotPos = Owner.Center + new Vector2(dist, dist).RotatedBy(Projectile.ai[1] + RotationOffset);
            
            float lerpCoff = Math.Clamp((float)Easings.EaseInCirc(RotationOffset / MathF.Tau), 0f, 1f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(rotPos) * (Projectile.Distance(rotPos) < 50 ? 6 : 15), lerpCoff);

            if (Main.mouseLeft && Main.mouseLeftRelease && Projectile.timeLeft < 890)
            {
                RevolvingCheck++;
                RotationOffset = 0; // now used to check for tileCollide
            }
        }
        else if (Projectile.Distance(Main.MouseWorld) > 6)
        {
            RotationOffset++;
            Projectile.penetrate = 1;

            if (RotationOffset > 120)
                Projectile.tileCollide = true; //TODO: if proj is inside tile while transitioning, set tilecollide to false till it's not
            
            const int timeLeft = 240;
            float lerpCoff = Math.Clamp((float)Easings.EaseInOutSine(RotationOffset / timeLeft), 0f, 1f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * 14, lerpCoff);
            
            Owner.ChangeDir(Projectile.Center.X > Owner.MountedCenter.X ? 1 : -1);
            float handRot = Owner.DirectionTo(Projectile.Center).ToRotation() - MathF.PI / 2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, handRot);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, handRot);
        }
        else
        {
            Splash(Projectile);
            Projectile.Kill();
        }
        
        if (Main.rand.NextBool(30))
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<WaterDrops>(), Projectile.velocity.X / 5 * -1,
                Projectile.velocity.Y / 5 * -1, default, new Color(76, Main.rand.Next(100, 200), 255), Main.rand.NextFloat(2f, 4f));
            dust.noGravity = true;
        }
        
        Lighting.AddLight(Projectile.Center, new Color(76, 120, 255, 0).ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color color = new(76, 150, 255);

        // scaleDecrease controls length of the trail
        WaterMetaball metaball = new(outlineColor: Color.Lerp(Color.White with { A = 0 }, color, Main.masterColor / 4), scale: 6.5f, timeLeft: 10,
            parent: Projectile, scaleDecreaseOverTime: 0.6f);
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
        int randDust = Main.rand.Next(Revolving ? 2 : 8, Revolving ? 6 : 15);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, entity.width, entity.height, ModContent.DustType<WaterDrops>(), 0f, 0f, default,
                new Color(76, Main.rand.Next(100, 200), 255), Main.rand.NextFloat(2f, 4f));

            dust.velocity *= Revolving ? 1.6f : 6.4f;
            dust.velocity.Y -= 1f;
            dust.velocity += Projectile.velocity;
            dust.noGravity = true;
        }

        SoundID.Splash.Play(entity.Center, Revolving ? 0.4f : 1f, 0.4f, -1);
    }
}