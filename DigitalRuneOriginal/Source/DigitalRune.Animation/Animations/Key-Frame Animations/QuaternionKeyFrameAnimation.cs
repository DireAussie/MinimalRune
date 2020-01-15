﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.


using MinimalRune.Animation.Traits;
using Microsoft.Xna.Framework;


namespace MinimalRune.Animation
{
  /// <summary>
  /// Animates a <see cref="Quaternion"/> value using key frames.
  /// (Only available in the XNA-compatible build.)
  /// </summary>
  /// <inheritdoc/>
  public class QuaternionKeyFrameAnimation : KeyFrameAnimation<Quaternion>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Quaternion> Traits
    {
      get { return QuaternionTraits.Instance; }
    }
  }
}

