using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ThePlayground.Content.Particles;
using ThePlayground.Core.Graphics;

namespace ThePlayground.Content.Projectiles;

public class FireTest : ModProjectile
{
    public override string Texture => $"{nameof(ThePlayground)}/Assets/Textures/EmptyTexture";
    private ref float MetaballTimer => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 72;
        Projectile.height = 36;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 900;
        Projectile.damage = 1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        MetaballTimer++;

        if (MetaballTimer % 15 == 0)
            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPos = Projectile.Center + new Vector2(Main.rand.Next(-60, 60), Main.rand.NextFloat(0, 40));
                FireMetaball metaball = new(spawnPos, outlineColor: Color.White with { A = 0 }, scale: 10, timeLeft: 30, parent: Projectile, scaleDecreaseOverTime: 0.25f);
                ParticleSystem.AddParticle(metaball, spawnPos, new Vector2(0, Main.rand.Next(-7, -3)), new Color(236, 97, 32));
            }

        Vector2 spawnPosRest = Projectile.Center + new Vector2(Main.rand.Next(-60, 60), 0);
        FireMetaball metaballRest = new(spawnPosRest, outlineColor: Color.White with { A = 0 }, scale: 10, timeLeft: 30, parent: Projectile,
            scaleDecreaseOverTime: 0.01f);
        ParticleSystem.AddParticle(metaballRest, spawnPosRest, Vector2.Zero, new Color(236, 97, 32));

        return false;
    }
}