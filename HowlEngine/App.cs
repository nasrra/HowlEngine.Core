using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HowlEngine;

public class HowlApp : Game {
    /// <summary>
    /// Gets a reference to the app instance.
    /// </summary>
    public static HowlApp Instance {get;private set;}

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

    public HowlApp(string title, int width, int height, bool fullScreen, bool mouseVisible) {

        // Ensure that only one app can be running.
        if (Instance != null) {
            throw new InvalidOperationException($"Only a single HowlApp instance can be created");
        }

        Instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // Apply changes.
        Graphics.ApplyChanges();

        // Set the window title
        Window.Title = title;

        // Set the content manager to reference the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        IsMouseVisible = mouseVisible;
    }

    protected override void Initialize(){
        base.Initialize();

        // set the graphics device to a reference to the base Game's
        GraphicsDevice = base.GraphicsDevice;

        // create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
	}
}