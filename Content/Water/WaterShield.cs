using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThePlayground.Content.Projectiles;
using ThePlayground.Utilities;

namespace ThePlayground.Content.Water;

public class WaterShield : ModItem
{
    public override string Texture => Graphics.EmptyTexture;

    public override void SetDefaults()
    {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<RevolvingSpouts>(), 40, 0, true);
        Item.damage = 20;
        Item.knockBack = 5;
    }

    public override bool CanUseItem(Player player)
    {
        return base.CanUseItem(player) && player.ownedProjectileCounts[Item.shoot] < 1;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = -1; i < 2; i += 2)
        {
            Vector2 spawnPos = player.Center + new Vector2(0, 100) * i;
            Projectile.NewProjectile(source, spawnPos, Vector2.Zero, type, damage, knockback, player.whoAmI, ai1: player.DirectionTo(spawnPos).ToRotation());
        }
        
        return false;
    }
}