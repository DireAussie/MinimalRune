using System;
using MinimalRune.Geometry.Shapes;
using MinimalRune.Mathematics;
using MinimalRune.Mathematics.Algebra;
using MinimalRune.Mathematics.Statistics;
using NUnit.Framework;


namespace MinimalRune.Geometry.Collisions.Algorithms.Tests
{
  [TestFixture]
  public class RaySphereAlgorithmTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ComputeCollisionException()
    {
      RaySphereAlgorithm algo = new RaySphereAlgorithm(new CollisionDetection());
      algo.GetClosestPoints(new CollisionObject(), new CollisionObject(new GeometricObject { Shape = new RayShape() })); // no sphere!
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ComputeCollisionException2()
    {
      RaySphereAlgorithm algo = new RaySphereAlgorithm(new CollisionDetection());
      algo.GetClosestPoints(new CollisionObject(), new CollisionObject(new GeometricObject { Shape = new SphereShape() })); // no ray!
    }


    [Test]
    public void ComputeCollisionLineLine()
    {
      CollisionObject ray = new CollisionObject(new GeometricObject
            {
              Shape = new RayShape(new Vector3(0, 0, -1), new Vector3(1, 0, 0), 10),
              Pose = new Pose(new Vector3(0, 0, 1)),
            });

      CollisionObject sphere = new CollisionObject(new GeometricObject
            {
              Shape = new SphereShape(1),
              Pose = new Pose(RandomHelper.Random.NextQuaternion()),
            });

      RaySphereAlgorithm algo = new RaySphereAlgorithm(new CollisionDetection());

      ContactSet set;

      set = algo.GetClosestPoints(ray, sphere);      
      Assert.AreEqual(0, set[0].PenetrationDepth);
      Assert.AreEqual(Vector3.UnitY, set[0].Normal);
      Assert.AreEqual(true, algo.HaveContact(ray, sphere));

      set = set.Swapped;
      algo.UpdateContacts(set, 0);
      Assert.AreEqual(0, set[0].PenetrationDepth);
      Assert.AreEqual(-Vector3.UnitY, set[0].Normal);

      ((GeometricObject)sphere.GeometricObject).Pose = new Pose(new Vector3(10, 0, 0), sphere.GeometricObject.Pose.Orientation);
      set = algo.GetClosestPoints(sphere, ray);
      Assert.IsTrue(Numeric.AreEqual(9, set[0].PenetrationDepth));
      Assert.IsTrue(Vector3.AreNumericallyEqual(-Vector3.UnitX, set[0].Normal));
      Assert.AreEqual(true, algo.HaveContact(ray, sphere));

      ((GeometricObject)ray.GeometricObject).Pose = new Pose(new Vector3(0, -2, 1), Quaternion.CreateRotationZ(ConstantsF.PiOver2));
      set = algo.GetClosestPoints(sphere, ray);
      Assert.IsTrue(Numeric.AreEqual(-9, set[0].PenetrationDepth));
      Assert.IsTrue(Vector3.AreNumericallyEqual(-Vector3.UnitX, set[0].Normal));
      Assert.IsTrue(Vector3.AreNumericallyEqual(new Vector3(4.5f, 0, 0), set[0].Position));
      Assert.AreEqual(false, algo.HaveContact(ray, sphere));

      algo.UpdateContacts(set, 0);
      Assert.AreEqual(0, set.Count);

      // Degenerate case: sphere radius is 0.
      ((GeometricObject)sphere.GeometricObject).Shape = new SphereShape(0);
      ((GeometricObject)ray.GeometricObject).Pose = new Pose(new Vector3(0, 0, 1));
      algo.UpdateClosestPoints(set, 0);
      Assert.AreEqual(0, set[0].PenetrationDepth);
      Assert.AreEqual(-Vector3.UnitY, set[0].Normal);
      Assert.AreEqual(true, algo.HaveContact(ray, sphere));
    }
  }
}
