
using System;
using System.ComponentModel.Design;
using HowlEngine.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public interface ISprite{
    /// <summary>
    /// Gets and Sets the reference to the texture atlas used by this sprite.
    /// </summary>
    // public WeakReference<Texture2D> Texture {get; set;}

    /// <summary>
    /// Gets or Sets the source texture region represented by this sprite.
    /// </summary>
    public TextureRegion TextureRegion{get; protected set;}

    /// <summary>
    /// Gets or Sets the color mask to apply when rendering.
    /// </summary>
    public Color Color {get;set;} // = Color.White;


    /// <summary>
    /// Gets or Sets the scale factor to apply to the xy-axes when rendering.
    /// </summary>
    public Vector2 Scale {get;set;} // = Vector2.One;

    /// <summary>
    /// Gets or set the xy-coordinate origin point, relative to the top left corner, of the sprite texture. 
    /// </summary>
    public Vector2 Origin {get;set;} // = Vector2.Zero;

    /// <summary>
    /// Gets or Sets the xy-position where the sprite is drawn.
    /// </summary>
    public Vector2 Position {get;set;} // = Vector2.Zero;

    /// <summary>
    /// Gets or Sets the sprite effect to apply when rendering.
    /// </summary>
    public SpriteEffects Effects {get;set;} // = SpriteEffects.None;

    /// <summary>
    /// Gets or Sets the amount of rotation, in radians, to apply when rendering.
    /// </summary>
    public float Rotation {get;set;} // = 0.0f;

    /// <summary>
    /// Gets or Sets the depth layer when rendering.
    /// </summary>
    public float Layer {get;set;} // = 0.0f;

    /// <summary>
    /// Gets the width, in pixels, of the sprite texture.
    /// </summary>
    public float Width => TextureRegion.Width * Scale.X;

    /// <summary>
    /// Gets the height, in pixels, of the sprite texture.
    /// </summary>
    public float Height => TextureRegion.Height * Scale.Y;

    // /// <summary>
    // /// Creates a new sprite.
    // /// </summary>
    // public Sprite(){}

    /// <summary>
    /// Creates a new sprite  using the specified source texture region.
    /// </summary>
    /// <param name="textureRegion">The texture region to use as the source texture when rendering.</param>
    // public Sprite(TextureRegion textureRegion, WeakReference<Texture2D> texture){
    //     TextureRegion = textureRegion;
    //     Texture = texture;
    // }

    // public Sprite(TextureRegion textureRegion, WeakReference<Texture2D> texture, Vector2 position){
    //     TextureRegion = textureRegion;
    //     Texture = texture;
    //     Position = position;
    // }
}