// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Collections;
using MinimalRune.Mathematics.Algebra;


namespace MinimalRune.Graphics
{
  /// <summary>
  /// Represents a figure that is composed of several figures.
  /// </summary>
  public class CompositeFigure : Figure
  {
    

    



    

    

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>The children.</value>
    public FigureCollection Children { get; private set; }


    /// <inheritdoc/>
    internal override bool HasFill
    {
      get
      {
        foreach (var child in Children)
          if (child.HasFill)
            return true;

        return false;
      }
    }



    

    

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeFigure"/> class.
    /// </summary>
    public CompositeFigure()
    {
      Children = new FigureCollection(this);
    }



    

    

    /// <inheritdoc/>
    internal override void Flatten(ArrayList<Vector3> vertices, ArrayList<int> strokeIndices, ArrayList<int> fillIndices)
    {
      foreach (var child in Children)
        child.Flatten(vertices, strokeIndices, fillIndices);
    }

  }
}
