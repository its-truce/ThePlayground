using System;
using Terraria;
using Terraria.ModLoader;

namespace ThePlayground.Core.Arcana;

public abstract class ModArcana : ModType
{
    public virtual string Texture => GetType().FullName?.Replace(".", "/");
    protected virtual int MaxCooldown => 60;
    public int Cooldown;
    
    protected override void Register()
    {
        ModTypeLookup<ModArcana>.Register(this);
    }

    public bool Activate()
    {
        if (Cooldown == 0)
        {
            Shoot();
            Cooldown = MaxCooldown;
            return true;
        }

        return false;
    }

    protected virtual void Shoot()
    {
    }

    private void Equip(int slot, Player player)
    {
        ArcanaPlayer arcanaPlayer = player.GetModPlayer<ArcanaPlayer>();

        slot = Math.Clamp(slot, 0, 4);
        arcanaPlayer.Arcana[slot + 1] = this;
    }
}