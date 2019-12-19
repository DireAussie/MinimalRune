using DigitalRune.Geometry;
using DigitalRune.Graphics.Scene3D;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using NUnit.Framework;


namespace DigitalRune.Graphics.Tests
{
  [TestFixture]
  public class CameraInstanceTest
  {
    [Test]
    public void PoseTest()
    {
      CameraInstance cameraInstance = new CameraInstance(new Camera(new PerspectiveProjection()));
      Assert.IsNotNull(cameraInstance.PoseWorld);
      Assert.AreEqual(Vector3.Zero, cameraInstance.PoseWorld.Position);
      Assert.AreEqual(Matrix.Identity, cameraInstance.PoseWorld.Orientation);

      // Set new Pose
      Vector3 position = new Vector3(1, 2, 3);
      Quaternion orientation = Quaternion.CreateRotation(new Vector3(3, 4, 5), 0.123f);
      cameraInstance.PoseWorld = new Pose(position, orientation);
      Assert.AreEqual(position, cameraInstance.PoseWorld.Position);
      Assert.AreEqual(orientation.ToRotationMatrix33(), cameraInstance.PoseWorld.Orientation);
      Assert.IsTrue(Matrix.AreNumericallyEqual(cameraInstance.PoseWorld.ToMatrix(), cameraInstance.ViewInverse));
      Assert.IsTrue(Matrix.AreNumericallyEqual(cameraInstance.PoseWorld.Inverse.ToMatrix(), cameraInstance.View));

      // Set Position and Orientation
      position = new Vector3(5, 6, 7);
      orientation = Quaternion.CreateRotation(new Vector3(1, -1, 6), -0.123f);
      cameraInstance.PoseWorld = new Pose(position, orientation);
      Assert.AreEqual(position, cameraInstance.PoseWorld.Position);
      Assert.AreEqual(orientation.ToRotationMatrix33(), cameraInstance.PoseWorld.Orientation);
      Assert.IsTrue(Matrix.AreNumericallyEqual(cameraInstance.PoseWorld.Inverse.ToMatrix(), cameraInstance.View));
      Assert.IsTrue(Matrix.AreNumericallyEqual(cameraInstance.PoseWorld.ToMatrix(), cameraInstance.ViewInverse));
    }


    [Test]
    public void ViewTest()
    {
      CameraInstance cameraInstance = new CameraInstance(new Camera(new PerspectiveProjection()));
      Assert.AreEqual(Matrix.Identity, cameraInstance.View);
      Assert.AreEqual(Matrix.Identity, cameraInstance.ViewInverse);

      Vector3 position = new Vector3(1, 2, 3);
      Vector3 target = new Vector3(2, 5, 4);
      Vector3 upVector = new Vector3(1, 1, 1);
      Matrix view = Matrix.CreateLookAt(position, target, upVector);

      cameraInstance.View = view;

      Assert.AreEqual(view, cameraInstance.View);
      Assert.AreEqual(view.Inverse, cameraInstance.ViewInverse);

      Vector3 originOfCamera = cameraInstance.PoseWorld.Position;
      originOfCamera = cameraInstance.View.TransformPosition(originOfCamera);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.Zero, originOfCamera));

      Vector4F positionView = new Vector4F(0, 0, -1, 1);
      Vector4F positionView2;

      // Transform a point from view space to world space.
      Vector4F positionWorld = cameraInstance.PoseWorld * positionView;
      Vector4F positionWorld2 = cameraInstance.ViewInverse * positionView;
      Assert.IsTrue(Vector4F.AreNumericallyEqual(positionWorld, positionWorld2));

      // Transform a point from world space to view space.
      positionView = cameraInstance.PoseWorld.Inverse * positionWorld;
      positionView2 = cameraInstance.View * positionWorld;
      Assert.IsTrue(Vector4F.AreNumericallyEqual(positionView, positionView2));

      cameraInstance.View = Matrix.Identity;
      Assert.AreEqual(Vector3.Zero, cameraInstance.PoseWorld.Position);
      Assert.AreEqual(Matrix.Identity, cameraInstance.PoseWorld.Orientation);
    }


    [Test]
    public void InverseViewTest()
    {
      CameraInstance cameraInstance = new CameraInstance(new Camera(new PerspectiveProjection()));
      Assert.AreEqual(Matrix.Identity, cameraInstance.View);
      Assert.AreEqual(Matrix.Identity, cameraInstance.ViewInverse);

      Vector3 position = new Vector3(1, 2, 3);
      Vector3 target = new Vector3(2, 5, 4);
      Vector3 upVector = new Vector3(1, 1, 1);
      Matrix view = Matrix.CreateLookAt(position, target, upVector);

      cameraInstance.ViewInverse = view.Inverse;
      Assert.IsTrue(Matrix.AreNumericallyEqual(view, cameraInstance.View));
      Assert.IsTrue(Matrix.AreNumericallyEqual(view.Inverse, cameraInstance.ViewInverse));
      Assert.IsTrue(Matrix.AreNumericallyEqual(view.Inverse, cameraInstance.PoseWorld.ToMatrix()));
    }


    [Test]
    public void LookAtTest()
    {
      CameraInstance cameraInstance = new CameraInstance(new Camera(new PerspectiveProjection()));

      Vector3 position = new Vector3(1, 2, 3);
      Vector3 target = new Vector3(2, 5, 4);
      Vector3 upVector = new Vector3(1, 1, 1);

      cameraInstance.PoseWorld = new Pose(new Vector3(1, 2, 3));
      Matrix expected = Matrix.CreateLookAt(position, target, upVector);
      cameraInstance.LookAt(target, upVector);
      Assert.That(Matrix.AreNumericallyEqual(expected, cameraInstance.View));

      position = new Vector3(-2, 3, -7.5f);
      expected = Matrix.CreateLookAt(position, target, upVector);
      cameraInstance.LookAt(position, target, upVector);
      Assert.That(Vector3.AreNumericallyEqual(position, cameraInstance.PoseWorld.Position));
      Assert.That(Matrix.AreNumericallyEqual(expected, cameraInstance.View));
    }


    [Test]
    public void PoseChangedTest()
    {
      bool poseChanged = false;
      CameraInstance cameraInstance = new CameraInstance(new Camera(new PerspectiveProjection()));
      cameraInstance.PoseChanged += (sender, eventArgs) => poseChanged = true;

      cameraInstance.PoseWorld = new Pose(new Vector3(1, 2, 3));
      Assert.IsTrue(poseChanged);
    }


    [Test]
    public void ShapeChangedTest()
    {
      bool shapeChanged = false;
      CameraInstance cameraInstance = new CameraInstance(new Camera(new PerspectiveProjection()));
      cameraInstance.BoundingShapeChanged += (sender, eventArgs) => shapeChanged = true;

      cameraInstance.Camera.Projection.Far = 9;
      Assert.IsTrue(shapeChanged);
    }


    [Test]
    public void SetProjectionTest()
    {
      Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(1, 4, 2, 5, 6, 11);
      OrthographicProjection orthographicProjection = new OrthographicProjection();
      orthographicProjection.Set(projectionMatrix);
      CameraInstance cameraInstance = new CameraInstance(new Camera(orthographicProjection));

      Assert.AreEqual(Vector3.Zero, cameraInstance.PoseWorld.Position);
      Assert.AreEqual(Matrix.Identity, cameraInstance.PoseWorld.Orientation);
      Assert.That(Numeric.AreEqual(3, cameraInstance.Camera.Projection.Width));
      Assert.That(Numeric.AreEqual(3, cameraInstance.Camera.Projection.Height));
      Assert.That(Numeric.AreEqual(1f, cameraInstance.Camera.Projection.AspectRatio));
      Assert.That(Numeric.AreEqual(6, cameraInstance.Camera.Projection.Near));
      Assert.That(Numeric.AreEqual(11, cameraInstance.Camera.Projection.Far));
      Assert.That(Numeric.AreEqual(1, cameraInstance.Camera.Projection.Left));
      Assert.That(Numeric.AreEqual(4, cameraInstance.Camera.Projection.Right));
      Assert.That(Numeric.AreEqual(2, cameraInstance.Camera.Projection.Bottom));
      Assert.That(Numeric.AreEqual(5, cameraInstance.Camera.Projection.Top));
      Assert.That(Numeric.AreEqual(5, cameraInstance.Camera.Projection.Depth));
      Assert.That(Matrix.AreNumericallyEqual(orthographicProjection, cameraInstance.Camera.Projection));
      Assert.That(Matrix.AreNumericallyEqual(orthographicProjection.Inverse, cameraInstance.Camera.Projection.Inverse));
      Assert.IsNotNull(cameraInstance.BoundingShape);

      PerspectiveProjection perspectiveProjection = new PerspectiveProjection();
      perspectiveProjection.Inverse = Matrix.CreatePerspectiveOffCenter(1, 5, 2, 5, 1, 10).Inverse;
      cameraInstance = new CameraInstance(new Camera(perspectiveProjection));

      Assert.AreEqual(Vector3.Zero, cameraInstance.PoseWorld.Position);
      Assert.AreEqual(Matrix.Identity, cameraInstance.PoseWorld.Orientation);
      Assert.That(Numeric.AreEqual(MathHelper.ToRadians(33.690067f), cameraInstance.Camera.Projection.FieldOfViewX));
      Assert.That(Numeric.AreEqual(MathHelper.ToRadians(15.255119f), cameraInstance.Camera.Projection.FieldOfViewY));
      Assert.That(Numeric.AreEqual(4, cameraInstance.Camera.Projection.Width));
      Assert.That(Numeric.AreEqual(3, cameraInstance.Camera.Projection.Height));
      Assert.That(Numeric.AreEqual(4.0f / 3.0f, cameraInstance.Camera.Projection.AspectRatio));
      Assert.That(Numeric.AreEqual(1, cameraInstance.Camera.Projection.Left));
      Assert.That(Numeric.AreEqual(5, cameraInstance.Camera.Projection.Right));
      Assert.That(Numeric.AreEqual(2, cameraInstance.Camera.Projection.Bottom));
      Assert.That(Numeric.AreEqual(5, cameraInstance.Camera.Projection.Top));
      Assert.That(Numeric.AreEqual(1, cameraInstance.Camera.Projection.Near));
      Assert.That(Numeric.AreEqual(10, cameraInstance.Camera.Projection.Far));
      Assert.That(Numeric.AreEqual(9, cameraInstance.Camera.Projection.Depth));
      Assert.IsNotNull(cameraInstance.BoundingShape);
    }
  }
}
