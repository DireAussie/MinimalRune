﻿using System;
using MinimalRune.Mathematics.Algebra;
using MinimalRune.Mathematics.Interpolation;
using MinimalRune.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace MinimalRune.Animation.Traits.Tests
{
  [TestFixture]
  public class QuaternionTraitsTest
  {
    private Random _random;


    [SetUp]
    public void Setup()
    {
      _random = new Random(123456);
    }

    [Test]
    public void XnaQuaternionMultiplication()
    {
      Quaternion q1 = _random.NextQuaternion();
      Quaternion q2 = _random.NextQuaternion();
      var q1Xna = (Quaternion)q1;
      var q2Xna = (Quaternion)q2;
      
      Assert.IsTrue(Quaternion.AreNumericallyEqual(q1 * q2, (Quaternion)(q1Xna * q2Xna)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(q2 * q1, (Quaternion)(q2Xna * q1Xna)));
    }


    [Test]
    public void IdentityTest()
    {
      var traits = QuaternionTraits.Instance;
      var value = (Quaternion)_random.NextQuaternion();
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }


    [Test]
    public void MultiplyTest()
    {
      var traits = QuaternionTraits.Instance;
      var value = (Quaternion)_random.NextQuaternion();
      Quaternion valueInverse = value;
      valueInverse.Conjugate();
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)Quaternion.Identity, (Quaternion)traits.Multiply(value, 0)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)value, (Quaternion)traits.Multiply(value, 1)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)(value * value), (Quaternion)traits.Multiply(value, 2)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)(value * value * value), (Quaternion)traits.Multiply(value, 3)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)valueInverse, (Quaternion)traits.Multiply(value, -1)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)valueInverse * (Quaternion)valueInverse, (Quaternion)traits.Multiply(value, -2)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)valueInverse * (Quaternion)valueInverse * (Quaternion)valueInverse, (Quaternion)traits.Multiply(value, -3)));
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = QuaternionTraits.Instance;
      var from = (Quaternion)_random.NextQuaternion();
      var by = (Quaternion)_random.NextQuaternion();

      var to = traits.Add(from, by);
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)by * (Quaternion)from, (Quaternion)to));

      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)from, (Quaternion)traits.Add(to, traits.Inverse(by))));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)by, (Quaternion)traits.Add(traits.Inverse(from), to)));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = QuaternionTraits.Instance;
      var first = (Quaternion)_random.NextQuaternion();    // Animation value of first key frame.
      var last = (Quaternion)_random.NextQuaternion();     // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);
      var cycleOffsetInverse = cycleOffset;
      cycleOffsetInverse.Conjugate();

      // Cycle offset should be the difference between last and first key frame.
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)last, (Quaternion)traits.Add(first, cycleOffset)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)last, (Quaternion)(cycleOffset * first)));

      // Check multiple cycles (post-loop).
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)last, (Quaternion)traits.Add(first, traits.Multiply(cycleOffset, 1))));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)cycleOffset * (Quaternion)cycleOffset * (Quaternion)last, (Quaternion)traits.Add(first, traits.Multiply(cycleOffset, 3))));

      // Check multiple cycles (pre-loop).
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)first, (Quaternion)traits.Add(last, traits.Multiply(cycleOffset, -1))));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)cycleOffsetInverse * (Quaternion)cycleOffsetInverse * (Quaternion)first, (Quaternion)traits.Add(last, traits.Multiply(cycleOffset, -3))));
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = QuaternionTraits.Instance;
      var value0 = (Quaternion)_random.NextQuaternion();
      var value1 = (Quaternion)_random.NextQuaternion();
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)value0, (Quaternion)traits.Interpolate(value0, value1, 0.0f)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual((Quaternion)value1, (Quaternion)traits.Interpolate(value0, value1, 1.0f)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(InterpolationHelper.Lerp((Quaternion)value0, (Quaternion)value1, 0.75f), (Quaternion)traits.Interpolate(value0, value1, 0.75f)));
    }
  }
}
