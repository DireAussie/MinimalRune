﻿
using System;
using MinimalRune.Geometry;
using MinimalRune.Graphics;
using MinimalRune.Graphics.Rendering;
using MinimalRune.Graphics.SceneGraph;
using MinimalRune.Mathematics;
using MinimalRune.Mathematics.Algebra;
using MinimalRune.Mathematics.Statistics;
using MinimalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using MathHelper = Microsoft.Xna.Framework.MathHelper;


namespace Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample shows how to render two different cameras for a split-screen game.",
    @"",
    105)]
  public class SplitScreenSample : Sample
  {
    private readonly SplitScreen _graphicsScreen;

    // The second camera. 
    private readonly CameraNode _cameraNodeB;


    public SplitScreenSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      SampleFramework.IsMouseVisible = false;

      _graphicsScreen = new SplitScreen(Services);
      _graphicsScreen.DrawReticle = true;
      GraphicsService.Screens.Insert(0, _graphicsScreen);

      Services.Register(typeof(DebugRenderer), null, _graphicsScreen.DebugRenderer);
      Services.Register(typeof(IScene), null, _graphicsScreen.Scene);

      // Add gravity and damping to the physics Simulation.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a custom game object which controls the camera of player A.
      var cameraGameObject = new CameraObject(Services);
      GameObjectService.Objects.Add(cameraGameObject);
      _graphicsScreen.ActiveCameraNode = cameraGameObject.CameraNode;

      var projection = (PerspectiveProjection)cameraGameObject.CameraNode.Camera.Projection;
      projection.SetFieldOfView(
        projection.FieldOfViewY,
        GraphicsService.GraphicsDevice.Viewport.AspectRatio / 2,
        projection.Near,
        projection.Far);
      cameraGameObject.CameraNode.Camera = new Camera(projection);

      // A second camera for player B.
      _cameraNodeB = new CameraNode(cameraGameObject.CameraNode.Camera);
      _graphicsScreen.ActiveCameraNodeB = _cameraNodeB;

      GameObjectService.Objects.Add(new GrabObject(Services));
      GameObjectService.Objects.Add(new GroundObject(Services));
      GameObjectService.Objects.Add(new DudeObject(Services));
      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));
      GameObjectService.Objects.Add(new LavaBallsObject(Services));
      GameObjectService.Objects.Add(new FogObject(Services));
      GameObjectService.Objects.Add(new StaticObject(Services, "Barrier/Barrier", 0.9f, new Pose(new Vector3(0, 0, -2))));
      GameObjectService.Objects.Add(new StaticObject(Services, "Barrier/Cylinder", 0.9f, new Pose(new Vector3(3, 0, 0), Quaternion.CreateRotationY(MathHelper.ToRadians(-20)))));
      GameObjectService.Objects.Add(new StaticSkyObject(Services));

      // Add a few palm trees.
      Random random = new Random(12345);
      for (int i = 0; i < 10; i++)
      {
        Vector3 position = new Vector3(random.NextFloat(-3, -8), 0, random.NextFloat(0, -5));
        Matrix orientation = Matrix.CreateRotationY(random.NextFloat(0, ConstantsF.TwoPi));
        float scale = random.NextFloat(0.5f, 1.2f);
        GameObjectService.Objects.Add(new StaticObject(Services, "PalmTree/palm_tree", scale, new Pose(position, orientation)));
      }
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        _cameraNodeB.Dispose(false);
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      // This sample clears the debug renderer each frame.
      _graphicsScreen.DebugRenderer.Clear();

      // A second camera for player B.
      var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
      var position = Matrix.CreateRotationY(totalTime * 0.1f) * new Vector3(4, 2, 4);
      _cameraNodeB.View = Matrix.CreateLookAt(position, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
    }
  }
}
