using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

/// <summary>
/// Represents a rectangular region within a texture.
/// </summary>
public class TextureRegion{
    /// <summary>
    /// Gets or Sets the source texture this region is part of.
    /// </summary>
    public Texture2D Texture {get; set;}

    /// <summary>
    /// Gets or Sets the source rectangle boundary of this texture region within the source texture.
    /// </summary>
    public Rectangle SourceRect {get; set;}

    /// <summary>
    /// Gets the width, in pixels, of this texture region.
    /// </summary>
    public int Width => SourceRect.Width;

    /// <summary>
    /// Gets the height, in pixels, of this texture region.
    /// </summary>
    public int Height => SourceRect.Height;

    /// <summary>
    /// Creates a new texture region using the specified source texture.
    /// </summary>
    public TextureRegion(){}

    /// <summary>
    /// Creates a new texture region using the specified source texture.
    /// </summary>
    /// <param name="texture">The texture to use as the source texture for this texture region</param>
    /// <param name="x">The x-coordintate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
    /// <param name="y">The y-coordintate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
    /// <param name="width">The width, in pixels, of this texture region.</param>
    /// <param name="height">The height, in pixels, of this texture region.</param>
    public TextureRegion(Texture2D texture, int x, int y, int width, int height){
        Texture = texture;
        SourceRect = new Rectangle(x, y, width, height);
    }

    /// <summary>
    /// Submit this texture region for drawing in the current batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite_batch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate location to draw this texture region.</param>
    /// <param name="color">The colour tint to apply when drawing this texture region.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color){
        Draw(
            spriteBatch, 
            position, 
            color, 
            0.0f, 
            Vector2.Zero, 
            Vector2.One, 
            SpriteEffects.None, 
            0.0f
        );
    }

    /// <summary>
    /// Submit this texture region for drawing in the current batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite_batch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate location to draw this texture region.</param>
    /// <param name="color">The colour tint to apply when drawing this texture region.</param>
    /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region.</param>
    /// <param name="origin">The center of rotation, scaling, and position when drawing this texture region.</param>
    /// <param name="scale">The scale factor to apply when drawing this texture.</param>
    /// <param name="spriteEffects">Specifies whether the texture is flipped horinzontally, vertically, or both.</param>
    /// <param name="layer">The sorting layer to use when drawing this texture region.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float _scale, SpriteEffects spriteEffects, float layer){
        Draw(
            spriteBatch,
            position,
            color,
            rotation,
            origin,
            new Vector2(_scale, _scale),
            spriteEffects,
            layer
        );
    }


    /// <summary>
    /// Submit this texture region for drawing in the current batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite_batch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate location to draw this texture region.</param>
    /// <param name="color">The colour tint to apply when drawing this texture region.</param>
    /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region.</param>
    /// <param name="origin">The center of rotation, scaling, and position when drawing this texture region.</param>
    /// <param name="scale">The scale factor to apply when drawing this texture.</param>
    /// <param name="spriteEffects">Specifies whether the texture is flipped horinzontally, vertically, or both.</param>
    /// <param name="layer">The sorting layer to use when drawing this texture region.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layer){
        spriteBatch.Draw(
            Texture,
            position,
            SourceRect,
            color,
            rotation,
            origin,
            scale,
            spriteEffects,
            layer
        );
    }
}