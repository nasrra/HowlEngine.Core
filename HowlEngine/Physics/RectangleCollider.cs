using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine.Physics;

public struct RectangleCollider{
    private Rectangle _rectangle;
    
    /// <summary>
    /// Gets and Sets the xy-position of the collider.
    /// </summary>
    public Vector2 Position{
        get => new Vector2(_rectangle.X, _rectangle.Y);
        set {
            _rectangle.X = (int)value.X;
            _rectangle.Y = (int)value.Y;
        }
    }

    /// <summary>
    /// Gets and Sets the width the of the collider.
    /// </summary>
    public int Width {
        get => _rectangle.Width;
        set => _rectangle.Width = value;
    }

    /// <summary>
    /// Gets and Sets the height of the collider.
    /// </summary>
    public int Height {
        get => _rectangle.Height;
        set => _rectangle.Height = value;
    }

    /// <summary>
    /// Gets and Sets the x-position of the collider.
    /// </summary>
    public int X {
        get => _rectangle.X;
        set => _rectangle.X = value;
    }

    /// <summary>
    /// Gets and Sets the y_posiiton of the collider.
    /// </summary>
    public int Y {
        get => _rectangle.Y;
        set => _rectangle.Y = value;
    }

    /// <summary>
    /// Gets the x-coordinate of the left edge of this Collider.
    /// </summary>
    public int Left => _rectangle.Left;

    /// <summary>
    /// Gets the x-coordinate of the right edge of this Collider.
    /// </summary>
    public int Right => _rectangle.Right;

    /// <summary>
    /// Gets the y-coordinate of the top edge of this Collider.
    /// </summary>
    public int Top => _rectangle.Top;

    /// <summary>
    /// Gets the y-coordinate of the bottom edge of this Collider.
    /// </summary>
    public int Bottom => _rectangle.Bottom;


    /// <summary>
    /// Creates a new instance of RectangleCollider, with the specified position, width, and height.
    /// </summary>
    /// <param name="x">The x-coordinate of the top-left corner of the collider.</param>
    /// <param name="y">The y-coordinate of the top-left corner of the collider.</param>
    /// <param name="width">The width of the collider.</param>
    /// <param name="height">The height of the collider.</param>
    public RectangleCollider(int x, int y, int width, int height){
        _rectangle = new Rectangle(x,y,width,height);
    }
}