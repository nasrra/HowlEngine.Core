using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

public class Sprite{
    /// <summary>
    /// Gets or Sets the source texture region represented by this sprite.
    /// </summary>
    public TextureRegion textureRegion {get;set;}

    /// <summary>
    /// Gets or Sets the color mask to apply when rendering.
    /// </summary>
    public Color color {get;set;} = Color.White;

    /// <summary>
    /// Gets or Sets the amount of rotation, in radians, to apply when rendering.
    /// </summary>
    public float rotation {get;set;} = 0.0f;

    /// <summary>
    /// Gets or Sets the scale factor to apply to the xy-axes when rendering.
    /// </summary>
    public Vector2 scale {get;set;} = Vector2.One;

    /// <summary>
    /// Gets or set the xy-coordinate origin point, relative to the top left corner, of the sprite texture. 
    /// </summary>
    public Vector2 origin {get;set;} = Vector2.Zero;

    /// <summary>
    /// Gets or Sets the sprite effect to apply when rendering.
    /// </summary>
    public SpriteEffects effects {get;set;} = SpriteEffects.None;

    /// <summary>
    /// Gets or Sets the depth layer when rendering.
    /// </summary>
    public float layer {get;set;} = 0.0f;

    /// <summary>
    /// Gets the width, in pixels, of the sprite texture.
    /// </summary>
    public float width => textureRegion.width * scale.X;

    /// <summary>
    /// Gets the height, in pixels, of the sprite texture.
    /// </summary>
    public float height => textureRegion.height * scale.Y;

    /// <summary>
    /// Creates a new sprite.
    /// </summary>
    public Sprite(){}

    /// <summary>
    /// Creates a new sprite  using the specified source texture region.
    /// </summary>
    /// <param name="_textureRegion">The texture region to use as the source texture when rendering.</param>
    public Sprite(TextureRegion _textureRegion){
        textureRegion = _textureRegion;
    }

    /// <summary>
    /// Sets the origin of the sprite to the center of the texture.
    /// </summary>
    public void CenterOrigin(){
        origin = new Vector2(textureRegion.width, textureRegion.height) * 0.5f;
    }

    /// <summary>
    /// Sets the origin of the sprite to the top left of the texture.
    /// </summary>
    public void ZeroOrigin(){
        origin = Vector2.Zero;
    }

    /// <summary>
    /// Submit the sprite for drawing to the current batch.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate position to render at.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position){
        textureRegion.Draw(
            spriteBatch,
            position,
            color,
            rotation,
            origin,
            scale,
            effects,
            layer
        );
    }
}