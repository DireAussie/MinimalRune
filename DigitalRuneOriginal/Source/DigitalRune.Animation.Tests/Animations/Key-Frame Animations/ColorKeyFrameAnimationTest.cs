﻿using MinimalRune.Animation.Traits;
using NUnit.Framework;


namespace MinimalRune.Animation.Tests
{
  [TestFixture]
  public class ColorKeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new ColorKeyFrameAnimation();
      Assert.AreEqual(ColorTraits.Instance, animationEx.Traits);
    }
  }
}
