﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Geometry.Shapes;
using MinimalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Content;


namespace MinimalRune.Geometry.Content
{
  /// <summary>
  /// Reads a <see cref="RayShape"/> from binary format.
  /// </summary>
  public class RayShapeReader : ContentTypeReader<RayShape>
  {

    /// <summary>
    /// Determines if deserialization into an existing object is possible.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the type can be deserialized into an existing instance; 
    /// <see langword="false"/> otherwise.
    /// </value>
    public override bool CanDeserializeIntoExistingObject
    {
      get { return true; }
    }


    
    /// <summary>
    /// Reads a strongly typed object from the current stream.
    /// </summary>
    /// <param name="input">The <see cref="ContentReader"/> used to read the object.</param>
    /// <param name="existingInstance">An existing object to read into.</param>
    /// <returns>The type of object to read.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    protected override RayShape Read(ContentReader input, RayShape existingInstance)
    {
      if (existingInstance == null)
        existingInstance = new RayShape();

      existingInstance.Origin = input.ReadRawObject<Vector3>();
      existingInstance.Direction = input.ReadRawObject<Vector3>();
      existingInstance.Length = input.ReadSingle();
      existingInstance.StopsAtFirstHit = input.ReadBoolean();

      return existingInstance;
    }
  }
}
