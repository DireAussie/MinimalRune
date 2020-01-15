// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Animation.Traits;


namespace MinimalRune.Animation
{
  /// <summary>
  /// Animates a <see langword="Single"/> value from/to/by a certain value.
  /// </summary>
  /// <inheritdoc/>
  public class SingleFromToByAnimation : FromToByAnimation<float>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<float> Traits
    {
      get { return SingleTraits.Instance; }
    }
  }
}
