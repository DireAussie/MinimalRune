﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using MinimalRune.Geometry.Shapes;
using Microsoft.Xna.Framework.Content;


namespace MinimalRune.Geometry.Content
{
  /// <summary>
  /// Reads a <see cref="TransformedShape"/> from binary format.
  /// </summary>
  public class TransformedShapeReader : ContentTypeReader<TransformedShape>
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
    protected override TransformedShape Read(ContentReader input, TransformedShape existingInstance)
    {
      if (existingInstance == null)
        existingInstance = new TransformedShape();

      input.ReadSharedResource<GeometricObject>(geometry => existingInstance.Child = geometry);

      return existingInstance;
    }
  }
}
