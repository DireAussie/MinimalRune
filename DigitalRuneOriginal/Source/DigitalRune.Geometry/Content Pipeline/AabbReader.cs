﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Geometry.Shapes;
using MinimalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Content;


namespace MinimalRune.Geometry.Content
{
  /// <summary>
  /// Reads an <see cref="Aabb"/> from binary format.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
  public class AabbReader : ContentTypeReader<Aabb>
  {
    /// <summary>
    /// Reads a strongly typed object from the current stream.
    /// </summary>
    /// <param name="input">The <see cref="ContentReader"/> used to read the object.</param>
    /// <param name="existingInstance">An existing object to read into.</param>
    /// <returns>The type of object to read.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    protected override Aabb Read(ContentReader input, Aabb existingInstance)
    {
      Vector3 minimum = input.ReadRawObject<Vector3>();
      Vector3 maximum = input.ReadRawObject<Vector3>();
      return new Aabb(minimum, maximum);
    }
  }
}
