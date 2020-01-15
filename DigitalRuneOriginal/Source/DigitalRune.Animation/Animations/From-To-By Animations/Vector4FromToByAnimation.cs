// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.


using MinimalRune.Animation.Traits;
using Microsoft.Xna.Framework;


namespace MinimalRune.Animation
{
  /// <summary>
  /// Animates a <see langword="Vector4"/> value from/to/by a certain value.
  /// (Only available in the XNA-compatible build.)
  /// </summary>
  /// <inheritdoc/>
  public class Vector4romToByAnimation : FromToByAnimation<Vector4>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector4> Traits
    {
      get { return Vector4Traits.Instance; }
    }
  }
}

