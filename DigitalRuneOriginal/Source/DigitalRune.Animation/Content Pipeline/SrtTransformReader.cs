// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Animation.Character;
using MinimalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Content;


namespace MinimalRune.Animation.Content
{
  /// <summary>
  /// Reads a <see cref="SrtTransform"/> from binary format. 
  /// (Only available in the XNA-compatible build.)
  /// </summary>
  /// <remarks>
  /// This type is available only in the XNA-compatible build of the DigitalRune.Animation.dll.
  /// </remarks>
  public class SrtTransformReader : ContentTypeReader<SrtTransform>
  {
    /// <summary>
    /// Reads a strongly typed object from the current stream.
    /// </summary>
    /// <param name="input">The <see cref="ContentReader"/> used to read the object.</param>
    /// <param name="existingInstance">An existing object to read into.</param>
    /// <returns>The type of object to read.</returns>
    protected override SrtTransform Read(ContentReader input, SrtTransform existingInstance)
    {
      Vector3 scale = input.ReadRawObject<Vector3>();
      Quaternion rotation = input.ReadRawObject<Quaternion>();
      Vector3 translation = input.ReadRawObject<Vector3>();

      return new SrtTransform(scale, rotation, translation);
    }
  }
}
