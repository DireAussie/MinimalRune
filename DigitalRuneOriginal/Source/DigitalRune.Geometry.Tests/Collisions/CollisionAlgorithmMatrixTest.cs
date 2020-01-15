using System;
using MinimalRune.Geometry.Shapes;
using NUnit.Framework;


namespace MinimalRune.Geometry.Collisions.Tests
{
  [TestFixture]
  public class CollisionAlgorithmMatrixTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestException()
    {
      new CollisionAlgorithmMatrix(new CollisionDetection())[typeof(BoxShape), typeof(CapsuleShape)] = null;
    }
  }
}
