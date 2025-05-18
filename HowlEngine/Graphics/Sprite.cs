
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public class Sprite{
    /// <summary>
    /// Gets or Sets the source texture region represented by this sprite.
    /// </summary>
    public TextureRegion TextureRegion {get;set;}

    /// <summary>
    /// Gets or Sets the color mask to apply when rendering.
    /// </summary>
    public Color Color {get;set;} = Color.White;

    /// <summary>
    /// Gets or Sets the amount of rotation, in radians, to apply when rendering.
    /// </summary>
    public float Rotation {get;set;} = 0.0f;

    /// <summary>
    /// Gets or Sets the scale factor to apply to the xy-axes when rendering.
    /// </summary>
    public Vector2 Scale {get;set;} = Vector2.One;

    /// <summary>
    /// Gets or set the xy-coordinate origin point, relative to the top left corner, of the sprite texture. 
    /// </summary>
    public Vector2 Origin {get;set;} = Vector2.Zero;

    /// <summary>
    /// Gets or Sets the sprite effect to apply when rendering.
    /// </summary>
    public SpriteEffects Effects {get;set;} = SpriteEffects.None;

    /// <summary>
    /// Gets or Sets the depth layer when rendering.
    /// </summary>
    public float Layer {get;set;} = 0.0f;

    /// <summary>
    /// Gets the width, in pixels, of the sprite texture.
    /// </summary>
    public float Width => TextureRegion.Width * Scale.X;

    /// <summary>
    /// Gets the height, in pixels, of the sprite texture.
    /// </summary>
    public float Height => TextureRegion.Height * Scale.Y;

    /// <summary>
    /// Creates a new sprite.
    /// </summary>
    public Sprite(){}

    /// <summary>
    /// Creates a new sprite  using the specified source texture region.
    /// </summary>
    /// <param name="textureRegion">The texture region to use as the source texture when rendering.</param>
    public Sprite(TextureRegion textureRegion){
        TextureRegion = textureRegion;
    }

    /// <summary>
    /// Sets the origin of the sprite to the center of the texture.
    /// </summary>
    public void CenterOrigin(){
        Origin = new Vector2(TextureRegion.Width, TextureRegion.Height) * 0.5f;
    }

    /// <summary>
    /// Sets the origin of the sprite to the top left of the texture.
    /// </summary>
    public void ZeroOrigin(){
        Origin = Vector2.Zero;
    }

    /// <summary>
    /// Submit the sprite for drawing to the current batch.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate position to render at.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position){
        TextureRegion.Draw(
            spriteBatch,
            position,
            Color,
            Rotation,
            Origin,
            Scale,
            Effects,
            Layer
        );
    }
}