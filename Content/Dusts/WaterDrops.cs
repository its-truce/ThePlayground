using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ThePlayground.Content.Particles;
using ThePlayground.Core.Graphics;

namespace ThePlayground.Content.Dusts;

public class WaterDrops : ModDust
{
    private static int _timer;

    public override void OnSpawn(Dust dust)
    {
        _timer = 0;
    }

    public override bool Update(Dust dust)
    {
        _timer++;
        WaterMetaball metaball = new(outlineColor: Color.Lerp(Color.White, Color.SkyBlue, _timer / 125), scale: dust.scale, timeLeft: 10,
            scaleDecreaseOverTime: 0.15f);
        ParticleSystem.AddParticle(metaball, dust.position, Vector2.Zero, dust.color);

        return base.Update(dust);
    }

    public override bool PreDraw(Dust dust)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Main.EntitySpriteDraw(texture, dust.position - Main.screenPosition, null, dust.color with { A = 0 }, 0,
            texture.Size() / 2, dust.scale * 0.75f, SpriteEffects.None);

        return false;
    }
}