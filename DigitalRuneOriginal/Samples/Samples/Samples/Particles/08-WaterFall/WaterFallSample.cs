using MinimalRune.Diagnostics;
using MinimalRune.Geometry;
using MinimalRune.Graphics.SceneGraph;
using MinimalRune.Mathematics;
using MinimalRune.Mathematics.Algebra;
using MinimalRune.Particles;
using Microsoft.Xna.Framework;


namespace Samples.Particles
{
  [Sample(SampleCategory.Particles,
    "A waterfall effect.",
    @"This effect uses preloading. The water particles use a special billboard type to follow 
the direction of the water flow.",
    8)]
  public class WaterFallSample : ParticleSample
  {
    private readonly ParticleSystemNode _particleSystemNode;


    public WaterFallSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      ParticleSystem particleSystem = WaterFall.CreateWaterFall(ContentManager);
      particleSystem.Pose = new Pose(new Vector3(0, 2, 0), Matrix.CreateRotationY(ConstantsF.Pi));
      ParticleSystemService.ParticleSystems.Add(particleSystem);

      _particleSystemNode = new ParticleSystemNode(particleSystem);
      GraphicsScreen.Scene.Children.Add(_particleSystemNode);
    }


    public override void Update(GameTime gameTime)
    {
      // Synchronize particles <-> graphics.
      _particleSystemNode.Synchronize(GraphicsService);

      Profiler.AddValue("ParticleCount", ParticleHelper.CountNumberOfParticles(ParticleSystemService.ParticleSystems));
    }
  }
}
