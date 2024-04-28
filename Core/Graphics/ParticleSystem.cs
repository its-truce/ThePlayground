using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ThePlayground.Utilities;

// ReSharper disable UnusedMember.Local

namespace ThePlayground.Core.Graphics;

public enum ParticleLayer
{
    Normal,
    UI,
    BehindTiles
}

// TODO: documentation
public class ParticleSystem : ModSystem
{
    private const int MaxParticles = 3000;
    public static List<Particle> Particles;
    private static List<Particle> _tooltipParticles;
    private static List<Particle> _preTileParticles;

    public override void Load()
    {
        Particles = new List<Particle>(MaxParticles);
        _tooltipParticles = new List<Particle>(MaxParticles);
        _preTileParticles = new List<Particle>(MaxParticles);

        On_Main.DrawDust += (orig, self) =>
        {
            orig.Invoke(self);
            DrawParticles();
        };

        On_Main.DrawBackGore += (orig, self) =>
        {
            orig.Invoke(self);
            DrawParticlesBehindTiles();
        };
    }

    private static void DrawUIParticle(Vector2 linePosition)
    {
        Main.spriteBatch.Restart(samplerState: Main.SamplerStateForCursor, transformMatrix: Main.UIScaleMatrix);

        foreach (Particle particle in _tooltipParticles)
            if (particle.Active)
                particle.DrawInUI(Main.spriteBatch, linePosition);

        Main.spriteBatch.Restart(samplerState: Main.SamplerStateForCursor, rasterizerState: RasterizerState.CullCounterClockwise, transformMatrix: Main.UIScaleMatrix);
    }

    private static void DrawParticlesBehindTiles()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        bool postDraw = false;

        foreach (Particle particle in _preTileParticles)
            if (particle.Active)
            {
                if (particle.HasPartsDrawnOverParticles)
                    postDraw = true;

                particle.Draw(spriteBatch);
            }

        if (postDraw)
            foreach (Particle particle in _preTileParticles)
                if (particle.HasPartsDrawnOverParticles)
                    particle.DrawAfter(spriteBatch);
    }

    private static void DrawParticles()
    {
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null,
            Main.Transform);

        SpriteBatch spriteBatch = Main.spriteBatch;
        bool postDraw = false;

        foreach (Particle particle in Particles)
            if (particle.Active)
            {
                if (particle.HasPartsDrawnOverParticles)
                    postDraw = true;

                particle.Draw(spriteBatch);
            }

        if (postDraw) // draw this after all particles have finished drawing
            foreach (Particle particle in Particles)
                if (particle.HasPartsDrawnOverParticles && !particle.DrawsRenderTarget)
                    particle.DrawAfter(spriteBatch);

        Main.spriteBatch.End();
    }

    public override void PostUpdateDusts()
    {
        CheckActiveParticles(Particles);
        CheckActiveParticles(_tooltipParticles);
        CheckActiveParticles(_preTileParticles);

        void CheckActiveParticles(List<Particle> particleList)
        {
            foreach (Particle particle in particleList)
                if (particle.Active)
                {
                    particle.TimeInWorld++;
                    particle.Update();
                }
                else
                {
                    _tooltipParticles.Remove(particle);
                }
        }
    }

    public static void AddParticle(Particle particle, Vector2 position, Vector2? velocity = null, Color? color = null, ParticleLayer layer = ParticleLayer.Normal)
    {
        particle.Position = position;
        particle.Velocity = velocity ?? Vector2.Zero;
        particle.Color = color ?? Color.White;

        switch (layer)
        {
            case ParticleLayer.Normal:
            {
                AddParticleToList(Particles);
                break;
            }

            case ParticleLayer.UI:
            {
                AddParticleToList(_tooltipParticles);
                break;
            }

            case ParticleLayer.BehindTiles:
            {
                AddParticleToList(_preTileParticles);
                break;
            }
        }

        particle.OnSpawn();

        void AddParticleToList(List<Particle> particleList)
        {
            if (particleList.Count == MaxParticles)
                particleList.Remove(particleList[0]);

            particleList.Add(particle);
        }
    }
}

public abstract class Particle : ModType
{
    public bool Active = true;
    public Color Color;
    public Vector2 Position;
    public int TimeInWorld;
    public Vector2 Velocity;
    public virtual bool HasPartsDrawnOverParticles => false;
    public virtual bool DrawsRenderTarget => false;

    protected override void Register()
    {
        ModTypeLookup<Particle>.Register(this);
    }

    public virtual void Update()
    {
    }

    public virtual void OnSpawn()
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }

    public virtual void DrawAfter(SpriteBatch spriteBatch)
    {
    }

    public virtual void DrawInUI(SpriteBatch spriteBatch, Vector2 linePos)
    {
    }
}