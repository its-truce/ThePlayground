using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using ThePlayground.Utilities;

namespace ThePlayground.Content.Projectiles;

public class Circle : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 78;
        Projectile.height = 78;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Restart(SpriteSortMode.Immediate);

        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Effect effect = Graphics.GetEffect("Fire");
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects / 30f);
        effect.CurrentTechnique.Passes[0].Apply();

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0, texture.Size() / 2,
            0.25f, SpriteEffects.None);

        spriteBatch.Restart();

        return false;
    }
}