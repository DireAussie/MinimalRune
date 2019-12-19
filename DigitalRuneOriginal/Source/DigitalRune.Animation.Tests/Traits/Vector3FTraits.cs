using DigitalRune.Mathematics.Algebra;
using NUnit.Framework;


namespace DigitalRune.Animation.Traits.Tests
{
  [TestFixture]
  public class Vector3TraitsTest
  {
    [Test]
    public void IdentityTest()
    {
      var traits = Vector3Traits.Instance;
      var value = new Vector3(-1, -2, 3);
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }

    
    [Test]
    public void MultiplyTest()
    {
      var traits = Vector3Traits.Instance;
      var value = new Vector3(-1, -2, 3);
      Assert.IsTrue(Vector3.AreNumericallyEqual(Vector3.Zero, traits.Multiply(value, 0)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(value, traits.Multiply(value, 1)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(value + value, traits.Multiply(value, 2)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(value + value + value, traits.Multiply(value, 3)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(-value, traits.Multiply(value, -1)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(-value - value, traits.Multiply(value, -2)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(-value - value - value, traits.Multiply(value, -3)));
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = Vector3Traits.Instance;
      var from = new Vector3(-1, -2, 3);
      var by = new Vector3(4, -5, 6);

      var to = traits.Add(from, by);
      Assert.IsTrue(Vector3.AreNumericallyEqual(by + from, to));

      Assert.IsTrue(Vector3.AreNumericallyEqual(from, traits.Add(to, traits.Inverse(by))));
      Assert.IsTrue(Vector3.AreNumericallyEqual(by, traits.Add(traits.Inverse(from), to)));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = Vector3Traits.Instance;
      var first = new Vector3(1, 2, 3);    // Animation value of first key frame.
      var last = new Vector3(-4, 5, -6);   // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);

      // Cycle offset should be the difference between last and first key frame.
      Assert.IsTrue(Vector3.AreNumericallyEqual(last, traits.Add(first, cycleOffset)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(last, cycleOffset + first));

      // Check multiple cycles (post-loop).
      Assert.IsTrue(Vector3.AreNumericallyEqual(last, traits.Add(first, traits.Multiply(cycleOffset, 1))));
      Assert.IsTrue(Vector3.AreNumericallyEqual(cycleOffset + cycleOffset + last, traits.Add(first, traits.Multiply(cycleOffset, 3))));

      // Check multiple cycles (pre-loop).
      Assert.IsTrue(Vector3.AreNumericallyEqual(first, traits.Add(last, traits.Multiply(cycleOffset, -1))));
      Assert.IsTrue(Vector3.AreNumericallyEqual(first - cycleOffset - cycleOffset, traits.Add(last, traits.Multiply(cycleOffset, -3))));
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = Vector3Traits.Instance;
      var value0 = new Vector3(1, 2, 3);
      var value1 = new Vector3(-4, 5, -6);
      Assert.IsTrue(Vector3.AreNumericallyEqual(value0, traits.Interpolate(value0, value1, 0.0f)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(value1, traits.Interpolate(value0, value1, 1.0f)));
      Assert.IsTrue(Vector3.AreNumericallyEqual(0.25f * value0 + 0.75f * value1, traits.Interpolate(value0, value1, 0.75f)));
    }
  }
}
