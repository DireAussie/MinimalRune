using MinimalRune.Animation.Traits;
using NUnit.Framework;


namespace MinimalRune.Animation.Tests
{
  [TestFixture]
  public class Vector3KeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new Vector3KeyFrameAnimation();
      Assert.AreEqual(Vector3Traits.Instance, animationEx.Traits);
    }
  }
}
