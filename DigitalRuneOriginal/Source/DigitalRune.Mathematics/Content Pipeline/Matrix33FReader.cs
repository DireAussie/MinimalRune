// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Mathematics.Algebra;

using Microsoft.Xna.Framework.Content;



namespace MinimalRune.Mathematics.Content
{
  /// <summary>
  /// Reads a <see cref="Matrix"/> from binary format. (Only available in the XNA-compatible
  /// build.)
  /// </summary>
  /// <remarks>
  /// This type is available only in the XNA-compatible build of the DigitalRune.Mathematics.dll.
  /// </remarks>
  public class MatrixReader : ContentTypeReader<Matrix>
  {
    /// <summary>
    /// Reads a strongly typed object from the current stream.
    /// </summary>
    /// <param name="input">The <see cref="ContentReader"/> used to read the object.</param>
    /// <param name="existingInstance">An existing object to read into.</param>
    /// <returns>The type of object to read.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    protected override Matrix Read(ContentReader input, Matrix existingInstance)
    {
      float m00 = input.ReadSingle();
      float m01 = input.ReadSingle();
      float m02 = input.ReadSingle();
      float m10 = input.ReadSingle();
      float m11 = input.ReadSingle();
      float m12 = input.ReadSingle();
      float m20 = input.ReadSingle();
      float m21 = input.ReadSingle();
      float m22 = input.ReadSingle();

      return new Matrix(m00, m01, m02, 
                           m10, m11, m12, 
                           m20, m21, m22);
    }
  }
}
