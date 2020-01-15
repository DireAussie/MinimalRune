﻿using MinimalRune.Geometry;
using MinimalRune.Graphics.SceneGraph;


namespace Samples.Graphics
{
  // A GeometricObject class which stores a reference to a FigureNode.
  internal class FigureGeometricObject : GeometricObject
  {
    public FigureNode FigureNode { get; set; }
  }
}
