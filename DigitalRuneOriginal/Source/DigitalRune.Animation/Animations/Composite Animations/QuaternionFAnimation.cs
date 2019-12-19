// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using DigitalRune.Animation.Traits;
using DigitalRune.Mathematics.Algebra;

using Microsoft.Xna.Framework.Content;



namespace DigitalRune.Animation
{
  /// <summary>
  /// Animates a <see cref="Quaternion"/> value by applying an animation to each component of the
  /// quaternion.
  /// </summary>
  public class QuaternionAnimation : Animation<Quaternion>
  {
    //--------------------------------------------------------------

    //--------------------------------------------------------------



    //--------------------------------------------------------------

    //--------------------------------------------------------------

    /// <inheritdoc/>
    public override IAnimationValueTraits<Quaternion> Traits
    {
      get { return QuaternionTraits.Instance; }
    }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Quaternion.W"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Quaternion.W"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> W { get; set; }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Quaternion.X"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Quaternion.X"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> X { get; set; }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Quaternion.Y"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Quaternion.Y"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> Y { get; set; }


    /// <summary>
    /// Gets or sets the animation of the <see cref="Quaternion.Z"/> component.
    /// </summary>
    /// <value>The animation of the <see cref="Quaternion.Z"/> component.</value>

    [ContentSerializer(SharedResource = true)]

    public IAnimation<float> Z { get; set; }



    //--------------------------------------------------------------

    //--------------------------------------------------------------
    
    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="QuaternionAnimation"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="QuaternionAnimation"/> class.
    /// </summary>
    public QuaternionAnimation()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="QuaternionAnimation"/> class with the 
    /// specified animations.
    /// </summary>
    /// <param name="w">The animation of the <see cref="Quaternion.W"/> component.</param>
    /// <param name="x">The animation of the <see cref="Quaternion.X"/> component.</param>
    /// <param name="y">The animation of the <see cref="Quaternion.Y"/> component.</param>
    /// <param name="z">The animation of the <see cref="Quaternion.Z"/> component.</param>
    public QuaternionAnimation(IAnimation<float> w, IAnimation<float> x, IAnimation<float> y, IAnimation<float> z)
    {
      W = w;
      X = x;
      Y = y;
      Z = z;
    }



    //--------------------------------------------------------------

    //--------------------------------------------------------------

    /// <inheritdoc/>
    public override TimeSpan GetTotalDuration()
    {
      TimeSpan duration = TimeSpan.Zero;

      if (W != null)
        duration = AnimationHelper.Max(duration, W.GetTotalDuration());

      if (X != null)
        duration = AnimationHelper.Max(duration, X.GetTotalDuration());

      if (Y != null)
        duration = AnimationHelper.Max(duration, Y.GetTotalDuration());

      if (Z != null)
        duration = AnimationHelper.Max(duration, Z.GetTotalDuration());

      return duration;
    }


    /// <inheritdoc/>
    protected override void GetValueCore(TimeSpan time, ref Quaternion defaultSource, ref Quaternion defaultTarget, ref Quaternion result)
    {
      if (W != null)
        W.GetValue(time, ref defaultSource.W, ref defaultTarget.W, ref result.W);
      else
        result.W = defaultSource.W;

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
        result.Z = defaultSource.Z;
    }

  }
}
