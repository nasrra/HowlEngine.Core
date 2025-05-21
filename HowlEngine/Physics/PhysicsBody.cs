using HowlEngine.ECS;
using Microsoft.Xna.Framework;

namespace HowlEngine.Physics;

public struct PhysicsBodyAABB{
    public RectangleColliderStruct Collider;

    // may need to add previous velocity for previoud frame check in case that velocities decay over time.
    public Vector2 Velocity {get; set;}

    /// <summary>
    /// Gets and Sets the width the of the PhysicsBody.
    /// </summary>
    public int Width {
        get => Collider.Width;
        set => Collider.Width = value;
    }

    /// <summary>
    /// Gets and Sets the height of the PhysicsBody.
    /// </summary>
    public int Height {
        get => Collider.Height;
        set => Collider.Height = value;
    }

    /// <summary>
    /// Gets and Sets the x-position of the PhysicsBody.
    /// </summary>
    public int X {
        get => Collider.X;
        set => Collider.X = value;
    }

    /// <summary>
    /// Gets and Sets the y_posiiton of the PhysicsBody.
    /// </summary>
    public int Y {
        get => Collider.Y;
        set => Collider.Y = value;
    }

    /// <summary>
    /// Gets and Sets the xy-position of the PhysicsBody.
    /// </summary>
    public Vector2 Position{
        get => new Vector2(Collider.X, Collider.Y);
        set {
            Collider.X = (int)value.X;
            Collider.Y = (int)value.Y;
        }
    }

    /// <summary>
    /// Gets the x-coordinate of the left edge of this PhysicsBody.
    /// </summary>
    public int Left => Collider.Left;

    /// <summary>
    /// Gets the x-coordinate of the right edge of this PhysicsBody.
    /// </summary>
    public int Right => Collider.Right;

    /// <summary>
    /// Gets the y-coordinate of the top edge of this PhysicsBody.
    /// </summary>
    public int Top => Collider.Top;

    /// <summary>
    /// Gets the y-coordinate of the bottom edge of this PhysicsBody.
    /// </summary>
    public int Bottom => Collider.Bottom;

    public PhysicsBodyAABB(RectangleColliderStruct collider){
        Collider = collider;
        Velocity = Vector2.Zero;
    }

    public PhysicsBodyAABB(RectangleColliderStruct collider, Vector2 velocity){
        Collider = collider;
        Velocity = velocity;
    }
}