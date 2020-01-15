using MinimalRune.Animation.Traits;
using NUnit.Framework;


namespace MinimalRune.Animation.Tests
{
  [TestFixture]
  public class QuaternionKeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new QuaternionKeyFrameAnimation();
      Assert.AreEqual(QuaternionTraits.Instance, animationEx.Traits);
    }
  }
}
