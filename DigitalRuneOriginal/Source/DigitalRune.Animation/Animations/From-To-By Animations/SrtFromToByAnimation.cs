// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Animation.Character;
using MinimalRune.Animation.Traits;


namespace MinimalRune.Animation
{
  /// <summary>
  /// Animates an <see langword="SrtTransform"/> from/to/by a certain value.
  /// </summary>
  /// <inheritdoc/>
  public class SrtFromToByAnimation : FromToByAnimation<SrtTransform>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<SrtTransform> Traits
    {
      get { return SrtTransformTraits.Instance; }
    }
  }
}
