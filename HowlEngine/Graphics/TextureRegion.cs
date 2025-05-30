using HowlEngine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Graphics;

/// <summary>
/// Represents a rectangular region within a texture.
/// </summary>
public struct TextureRegion{
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
    /// <param name="x">The x-coordintate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
    /// <param name="y">The y-coordintate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
    /// <param name="width">The width, in pixels, of this texture region.</param>
    /// <param name="height">The height, in pixels, of this texture region.</param>
    public TextureRegion(int x, int y, int width, int height){
        SourceRect = new Rectangle(x, y, width, height);
    }

}