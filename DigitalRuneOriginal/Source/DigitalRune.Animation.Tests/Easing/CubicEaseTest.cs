﻿using MinimalRune.Mathematics;
using NUnit.Framework;


namespace MinimalRune.Animation.Easing.Tests
{
  [TestFixture]
  public class CubicEaseTest : BaseEasingFunctionTest<CubicEase>
  {
    [SetUp]
    public void Setup()
    {
      EasingFunction = new CubicEase();
    }


    [Test]
    public void EaseInTest()
    {
      EasingFunction.Mode = EasingMode.EaseIn;
      TestEase();
    }


    [Test]
    public void EaseOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseOut;
      TestEase();
    }


    [Test]
    public void EaseInOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseInOut;
      TestEase();

      // Check center.
      Assert.IsTrue(Numeric.AreEqual(0.5f, EasingFunction.Ease(0.5f)), "Easing function using EaseInOut failed for t = 0.5.");
    }
  }
}

