using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine;

public class HowlApp : Game {

    internal static HowlApp s_instance;

    /// <summary>
    /// Gets a reference to the app instance.
    /// </summary>
    public static HowlApp Instance => s_instance;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics {get; private set;}

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    public HowlApp(string _title, int _width, int _height, bool _full_screen, bool _mouse_visible) {

        // Ensure that only one app can be running.
        if (s_instance != null) {
            throw new InvalidOperationException($"[ERROR] Only a single HowlApp instance can be created");
        }

        s_instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = _width;
        Graphics.PreferredBackBufferHeight = _height;
        Graphics.IsFullScreen = _full_screen;

        // Apply changes.
        Graphics.ApplyChanges();

        // Set the window title
        Window.Title = _title;

        // Set the content manager to reference the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        IsMouseVisible = _mouse_visible;
    }

    protected override void Initialize(){
        base.Initialize();

        // set the graphics device to a reference to the base Game's
        GraphicsDevice = base.GraphicsDevice;

        // create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
	}
}