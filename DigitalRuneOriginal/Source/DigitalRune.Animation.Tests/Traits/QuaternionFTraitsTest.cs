using System;
using MinimalRune.Mathematics.Algebra;
using MinimalRune.Mathematics.Interpolation;
using MinimalRune.Mathematics.Statistics;
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
    public void IdentityTest()
    {
      var traits = QuaternionTraits.Instance;
      var value = _random.NextQuaternion();
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }


    [Test]
    public void MultiplyTest()
    {
      var traits = QuaternionTraits.Instance;
      var value = _random.NextQuaternion();
      Assert.IsTrue(Quaternion.AreNumericallyEqual(Quaternion.Identity, traits.Multiply(value, 0)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value, traits.Multiply(value, 1)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value * value, traits.Multiply(value, 2)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value * value * value, traits.Multiply(value, 3)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value.Inverse, traits.Multiply(value, -1)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value.Inverse * value.Inverse, traits.Multiply(value, -2)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value.Inverse * value.Inverse * value.Inverse, traits.Multiply(value, -3)));
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = QuaternionTraits.Instance;
      var from = _random.NextQuaternion();
      var by = _random.NextQuaternion();

      var to = traits.Add(from, by);
      Assert.IsTrue(Quaternion.AreNumericallyEqual(by * from, to));
      
      Assert.IsTrue(Quaternion.AreNumericallyEqual(from, traits.Add(to, traits.Inverse(by))));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(by, traits.Add(traits.Inverse(from), to)));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = QuaternionTraits.Instance;
      var first = _random.NextQuaternion();    // Animation value of first key frame.
      var last = _random.NextQuaternion();     // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);

      // Cycle offset should be the difference between last and first key frame.
      Assert.IsTrue(Quaternion.AreNumericallyEqual(last, traits.Add(first, cycleOffset)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(last, cycleOffset * first));

      // Check multiple cycles (post-loop).
      Assert.IsTrue(Quaternion.AreNumericallyEqual(last, traits.Add(first, traits.Multiply(cycleOffset, 1))));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(cycleOffset * cycleOffset * last, traits.Add(first, traits.Multiply(cycleOffset, 3))));

      // Check multiple cycles (pre-loop).
      Assert.IsTrue(Quaternion.AreNumericallyEqual(first, traits.Add(last, traits.Multiply(cycleOffset, -1))));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(cycleOffset.Inverse * cycleOffset.Inverse * first, traits.Add(last, traits.Multiply(cycleOffset, -3))));
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = QuaternionTraits.Instance;
      var value0 = _random.NextQuaternion();
      var value1 = _random.NextQuaternion();
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value0, traits.Interpolate(value0, value1, 0.0f)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(value1, traits.Interpolate(value0, value1, 1.0f)));
      Assert.IsTrue(Quaternion.AreNumericallyEqual(InterpolationHelper.Lerp(value0, value1, 0.75f), traits.Interpolate(value0, value1, 0.75f)));
    }
  }
}
