﻿using MinimalRune.Animation.Traits;
using NUnit.Framework;


namespace MinimalRune.Animation.Tests
{
  [TestFixture]
  public class SingleKeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new SingleKeyFrameAnimation();
      Assert.AreEqual(SingleTraits.Instance, animationEx.Traits);
    }
  }
}
