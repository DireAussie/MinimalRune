﻿using MinimalRune.Geometry;
using MinimalRune.Geometry.Shapes;
using MinimalRune.Mathematics;
using MinimalRune.Mathematics.Algebra;
using MinimalRune.Physics.Constraints;
using NUnit.Framework;


namespace MinimalRune.Physics.Constraints.Tests
{
  [TestFixture]
  public class ConstraintHelperTest
  {
    [Test]
    public void SpringDampingTest()
    {
      float erp = 0.3f;
      float cfm = 0.001f;
      
      float spring = ConstraintHelper.ComputeSpringConstant(1 / 60f, erp, cfm);
      float damping = ConstraintHelper.ComputeDampingConstant(1 / 60f, erp, cfm);
      
      Assert.IsTrue(Numeric.AreEqual(erp, ConstraintHelper.ComputeErrorReduction(1 / 60f, spring, damping)));
      Assert.IsTrue(Numeric.AreEqual(cfm, ConstraintHelper.ComputeSoftness(1 / 60f, spring, damping)));
    }

    [Test]
    public void ComputeKMatrix()
    {
      var b = new RigidBody(new EmptyShape());

      b.MassFrame = new MassFrame()
      {
        Mass = 3,
        Inertia = new Vector3(0.4f, 0.5f, 0.6f),
      };

      var r = new Vector3(1, 2, 3);
      var k = ConstraintHelper.ComputeKMatrix(b, r);

      var desiredK = 1 / b.MassFrame.Mass * Matrix.Identity - r.ToCrossProductMatrix() * Matrix.CreateScale(b.MassFrame.InertiaInverse) * r.ToCrossProductMatrix();

      Assert.AreEqual(desiredK, k);
    }


    [Test]
    public void SetVelocityTest()
    {
      var body = new RigidBody(new BoxShape(1, 2, 3));

      body.Pose = new Pose(new Vector3(10, 20, 30), Quaternion.CreateRotationY(1.1f));
      body.LinearVelocity = new Vector3(1, 2, 3);
      body.AngularVelocity = new Vector3(4, 5, 6);

      Vector3 pointLocal = new Vector3(0.5f, 0.9f, 1.3f);
      Vector3 point = body.Pose.ToWorldPosition(pointLocal);
      Vector3 targetVelocity = new Vector3(7, -8, 9);
      Assert.AreNotEqual(targetVelocity, body.GetVelocityOfLocalPoint(pointLocal));
      
      ConstraintHelper.SetVelocityOfWorldPoint(body, point, targetVelocity);
      Assert.IsTrue(Vector3.AreNumericallyEqual(targetVelocity, body.GetVelocityOfLocalPoint(pointLocal)));
    }
  }
}
