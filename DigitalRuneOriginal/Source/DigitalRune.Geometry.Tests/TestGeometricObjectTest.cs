using System;
using MinimalRune.Geometry.Shapes;
using MinimalRune.Mathematics.Algebra;
using NUnit.Framework;


namespace MinimalRune.Geometry.Tests
{

  [TestFixture]
  public class TestGeometricObjectTest
  {
    [Test]
    public void Events()
    {
      // The events are "empty". This is only for full code coverage:
      var g = TestGeometricObject.Create();
      g.PoseChanged += OnChanged;
      g.ShapeChanged += OnChanged;

      g.Pose = new Pose(new Vector3(1, 2, 3));
      g.Shape = new BoxShape(1, 2, 3);

      g.PoseChanged -= OnChanged;
      g.ShapeChanged -= OnChanged;
    }


    private void OnChanged(object sender, EventArgs e)
    {
      Assert.Fail("The TestGeometricObject must not raise the Changed-events.");
    }
  }
}
