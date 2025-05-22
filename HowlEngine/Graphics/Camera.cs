using System;
using Microsoft.Xna.Framework;

namespace HowlEngine.Graphics;

public class Camera{

    //==============================================================================================
    // A Translation Matrix shifts (moves) objects in 3D space along the X, Y, and Z axes.
    // It's used to change an objects posiiton without altering its orientation within world space.
    //==============================================================================================

    /// <summary>
    /// Gets the translation matrix.
    /// </summary>
    public Matrix TranslationMatrix {get;private set;} = Matrix.Identity;

    //===========================================================================
    // A View Matrix transforms world coordinates into camera (or view) space.
    // This effectively positions and orients the camera within the scene/level.
    //===========================================================================

    public Matrix ViewMatrix {get;private set;} = Matrix.Identity;

    // EXTRA:

    //==============================================================================================
    // A Projection matrix converts 3D coordinates into 2D screen space,
    // Applying perspective or orthographics projection to simulate depth and camera lens behaviour.
    //==============================================================================================

    /// <summary>
    /// Gets and Sets the position of the camera.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Gets and Sets the offset of the camera.
    /// </summary>
    public Vector2 Offset;

    /// <summary>
    /// Gets the resolution, in pixels, that camera can see in world-space.
    /// </summary>
    public Vector2 Resolution {get; private set;}

    /// <summary>
    /// Gets and Set the target position that the camera will move to.
    /// </summary>
    public Vector2? TargetPosition;

    /// <summary>
    /// How far from the screen the camera is currently positioned.
    /// </summary>
    public float Zoom = 1f; // 1 = normal, 2 = zoomed in, 0.5 = zoomed out

    /// <summary>
    /// Where the camera must be positioned to see the exact number of pixels as the current application windows resolution.
    /// </summary>
    private float baseZoom;

    public Camera(Vector2 position, Vector2 offset, Vector2 resolution){
        Position = position;
        Offset = offset;
        Resolution = resolution;

        int windowWidth = HowlApp.Graphics.PreferredBackBufferWidth; 
        int windowHeight = HowlApp.Graphics.PreferredBackBufferHeight;

        baseZoom = Math.Min(
            (float)windowWidth / Resolution.X,
            (float)windowHeight / Resolution.Y
        );
        Zoom = baseZoom;
    }
    

    public void Update(GameTime gameTime){
        int windowWidth = HowlApp.Graphics.PreferredBackBufferWidth; 
        int windowHeight = HowlApp.Graphics.PreferredBackBufferHeight;
        
        Vector2 target = TargetPosition ?? Vector2.Zero;
        Vector2 centerScreen = new Vector2(windowWidth * 0.5f, windowHeight * 0.5f);

        Vector2 cameraCenter = Position + Offset + target;

        // Move world opposite to camera.
        TranslationMatrix = Matrix.CreateTranslation(new Vector3(-cameraCenter, 0f));

        // Build the ViewMatrix
        ViewMatrix =
            TranslationMatrix *
            Matrix.CreateScale(Zoom, Zoom, 1f) *                        // Apply zoom
            Matrix.CreateTranslation(new Vector3(centerScreen, 0f));    // Move to center of screen
    }

    /// <summary>
    /// Multiplies a value to the current zoom of the camera.
    /// </summary>
    /// <param name="amount">The specified factor to multiply by.</param>
    public void MultiplyZoom(float factor){
        Zoom *= factor;
        Zoom = MathHelper.Clamp(Zoom, 0, float.MaxValue);
    }
}