// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Animation.Traits;
using MinimalRune.Mathematics.Algebra;


namespace MinimalRune.Animation
{
  /// <summary>
  /// Animates a <see cref="Vector3"/> value using key frames.
  /// </summary>
  /// <inheritdoc/>
  public class Vector3KeyFrameAnimation : KeyFrameAnimation<Vector3>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector3> Traits
    {
      get { return Vector3Traits.Instance; }
    }
  }
}
