// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using MinimalRune.Animation.Traits;
using MinimalRune.Mathematics.Algebra;

using Microsoft.Xna.Framework.Content;




namespace MinimalRune.Animation
{
  /// <summary>
  /// Animates a <see cref="Vector4"/> value by applying an animation to each component of the
  /// vector.
  /// </summary>
  public class Vector4Animation : Animation<Vector4>
  {
    

    



    

    

    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector4> Traits
    {
      get { return Vector4Traits.Instance; }
    }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Vector4.X"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Vector4.X"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> X { get; set; }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Vector4.Y"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Vector4.Y"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> Y { get; set; }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Vector4.Z"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Vector4.Z"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> Z { get; set; }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Vector4.W"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Vector4.W"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> W { get; set; }



    

    

    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="Vector4Animation"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="Vector4Animation"/> class.
    /// </summary>
    public Vector4Animation()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Vector4Animation"/> class with the specified
    /// animations.
    /// </summary>
    /// <param name="x">The animation of the <see cref="Vector4.X"/> component.</param>
    /// <param name="y">The animation of the <see cref="Vector4.Y"/> component.</param>
    /// <param name="z">The animation of the <see cref="Vector4.Z"/> component.</param>
    /// <param name="w">The animation of the <see cref="Vector4.W"/> component.</param>
    public Vector4Animation(IAnimation<float> x, IAnimation<float> y, IAnimation<float> z, IAnimation<float> w)
    {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }



    

    

    /// <inheritdoc/>
    public override TimeSpan GetTotalDuration()
    {
      TimeSpan duration = TimeSpan.Zero;

      if (X != null)
        duration = AnimationHelper.Max(duration, X.GetTotalDuration());

      if (Y != null)
        duration = AnimationHelper.Max(duration, Y.GetTotalDuration());

      if (Z != null)
        duration = AnimationHelper.Max(duration, Z.GetTotalDuration());

      if (W != null)
        duration = AnimationHelper.Max(duration, W.GetTotalDuration());

      return duration;
    }


    /// <inheritdoc/>
    protected override void GetValueCore(TimeSpan time, ref Vector4 defaultSource, ref Vector4 defaultTarget, ref Vector4 result)
    {
      if (X != null)
        X.GetValue(time, ref defaultSource.X, ref defaultTarget.X, ref result.X);
      else
        result.X = defaultSource.X;

      if (Y != null)
        Y.GetValue(time, ref defaultSource.Y, ref defaultTarget.Y, ref result.Y);
      else
        result.Y = defaultSource.Y;

      if (Z != null)
        Z.GetValue(time, ref defaultSource.Z, ref defaultTarget.Z, ref result.Z);
      else
        result.Y = defaultSource.Z;

      if (W != null)
        W.GetValue(time, ref defaultSource.W, ref defaultTarget.W, ref result.W);
      else
        result.W = defaultSource.W;
    }

  }
}
