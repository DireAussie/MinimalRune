﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

namespace MinimalRune.Graphics
{
  /// <summary>
  /// Represents a 2D bitmap rendered in screen space.
  /// </summary>
  /// <inheritdoc/>
  public class ImageSprite : Sprite
  {
    

    



    

    

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>The texture. (Can be <see langword="null"/>.)</value>
    /// <remarks>
    /// A packed texture can define a single image or a tile set. Tile sets can be used for 2D 
    /// animations.
    /// </remarks>
    public PackedTexture Texture { get; set; }



    

    

    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSprite"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSprite"/> class.
    /// </summary>
    public ImageSprite()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSprite"/> class with the specified 
    /// texture.
    /// </summary>
    /// <param name="texture">The texture. (Can be <see langword="null"/>.)</param>
    public ImageSprite(PackedTexture texture)
    {
      Texture = texture;
    }



    

    
    


    /// <inheritdoc cref="Sprite.Clone"/>
    public new ImageSprite Clone()
    {
      return (ImageSprite)base.Clone();
    }


    /// <inheritdoc/>
    protected override Sprite CreateInstanceCore()
    {
      return new ImageSprite();
    }


    /// <inheritdoc/>
    protected override void CloneCore(Sprite source)
    {
      // Clone Sprite properties.
      base.CloneCore(source);

      // Clone ImageSprite properties.
      var sourceTyped = (ImageSprite)source;
      Texture = sourceTyped.Texture;
    }



  }
}
