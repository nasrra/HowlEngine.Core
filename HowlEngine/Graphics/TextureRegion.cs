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
    public Texture2D texture {get; set;}

    /// <summary>
    /// Gets or Sets the source rectangle boundary of this texture region within the source texture.
    /// </summary>
    public Rectangle source_rect {get; set;}

    /// <summary>
    /// Gets the width, in pixels, of this texture region.
    /// </summary>
    public int width => source_rect.Width;

    /// <summary>
    /// Gets the height, in pixels, of this texture region.
    /// </summary>
    public int height => source_rect.Height;

    /// <summary>
    /// Creates a new texture region using the specified source texture.
    /// </summary>
    public TextureRegion(){}

    /// <summary>
    /// Creates a new texture region using the specified source texture.
    /// </summary>
    /// <param name="_texture">The texture to use as the source texture for this texture region</param>
    /// <param name="_x">The x-coordintate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
    /// <param name="_y">The y-coordintate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
    /// <param name="_width">The width, in pixels, of this texture region.</param>
    /// <param name="_height">The height, in pixels, of this texture region.</param>
    public TextureRegion(Texture2D _texture, int _x, int _y, int _width, int _height){
        texture = _texture;
        source_rect = new Rectangle(_x, _y, _width, _height);
    }

    /// <summary>
    /// Submit this texture region for drawing in the current batch.
    /// </summary>
    /// <param name="_sprite_batch">The sprite_batch instance used for batching draw calls.</param>
    /// <param name="_position">The xy-coordinate location to draw this texture region.</param>
    /// <param name="_color">The colour tint to apply when drawing this texture region.</param>
    public void draw(SpriteBatch _sprite_batch, Vector2 _position, Color _color){
        draw(
            _sprite_batch, 
            _position, 
            _color, 
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
    /// <param name="_sprite_batch">The sprite_batch instance used for batching draw calls.</param>
    /// <param name="_position">The xy-coordinate location to draw this texture region.</param>
    /// <param name="_color">The colour tint to apply when drawing this texture region.</param>
    /// <param name="_rotation">The amount of rotation, in radians, to apply when drawing this texture region.</param>
    /// <param name="_origin">The center of rotation, scaling, and position when drawing this texture region.</param>
    /// <param name="_scale">The scale factor to apply when drawing this texture.</param>
    /// <param name="_sprite_effects">Specifies whether the texture is flipped horinzontally, vertically, or both.</param>
    /// <param name="_layer">The sorting layer to use when drawing this texture region.</param>
    public void draw(SpriteBatch _sprite_batch, Vector2 _position, Color _color, float _rotation, Vector2 _origin, float _scale, SpriteEffects _sprite_effects, float _layer){
        draw(
            _sprite_batch,
            _position,
            _color,
            _rotation,
            _origin,
            new Vector2(_scale, _scale),
            _sprite_effects,
            _layer
        );
    }


    /// <summary>
    /// Submit this texture region for drawing in the current batch.
    /// </summary>
    /// <param name="_sprite_batch">The sprite_batch instance used for batching draw calls.</param>
    /// <param name="_position">The xy-coordinate location to draw this texture region.</param>
    /// <param name="_color">The colour tint to apply when drawing this texture region.</param>
    /// <param name="_rotation">The amount of rotation, in radians, to apply when drawing this texture region.</param>
    /// <param name="_origin">The center of rotation, scaling, and position when drawing this texture region.</param>
    /// <param name="_scale">The scale factor to apply when drawing this texture.</param>
    /// <param name="_sprite_effects">Specifies whether the texture is flipped horinzontally, vertically, or both.</param>
    /// <param name="_layer">The sorting layer to use when drawing this texture region.</param>
    public void draw(SpriteBatch _sprite_batch, Vector2 _position, Color _color, float _rotation, Vector2 _origin, Vector2 _scale, SpriteEffects _sprite_effects, float _layer){
        _sprite_batch.Draw(
            texture,
            _position,
            source_rect,
            _color,
            _rotation,
            _origin,
            _scale,
            _sprite_effects,
            _layer
        );
    }
}