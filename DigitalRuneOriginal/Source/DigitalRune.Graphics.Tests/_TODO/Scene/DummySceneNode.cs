using MinimalRune.Geometry.Shapes;
using MinimalRune.Graphics.Scene3D;


namespace MinimalRune.Graphics.Tests
{
  internal class DummySceneNode : SceneNode
  {
    public DummySceneNode()
    {
      BoundingShape = new SphereShape();
    }
  }
}
