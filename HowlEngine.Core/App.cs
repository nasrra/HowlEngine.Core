using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HowlEngine.Core.Input;
using HowlEngine.Audio;
using HowlEngine.Graphics;
using HowlEngine.Physics;
using HowlEngine.SceneManagement;
using HowlEngine.Collections;
using HowlEngine.AssetManagement;

namespace HowlEngine.Core;

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
    /// Gets the input manager used to get keyboard, mouse, and gamepad input.
    /// </summary>
    public static InputManager Input {get; private set;}

    /// <summary>
    /// The time taken between the previous and current frame.
    /// </summary>
    public static float DeltaTime{get; private set;}

    /// <summary>
    /// Gets the audio manager used to play and emit sounds.
    /// </summary>
    public static AudioManager AudioManager {get; protected set;}

    /// <summary>
    /// Gets the entity manager used to allocate and free any game objects.
    /// </summary>
    public static EntityManager EntityManager { get; protected set; }

    /// <summary>
    /// Gets the sprite renderer used to draw all images to the screen.
    /// </summary>
    public static SpriteRenderer SpriteRenderer { get; protected set; }

    /// <summary>
    /// Gets the physics system used to handle all physics bodies within the game.
    /// </summary>
    public static AABBPhysicSystem PhysicsSystem { get; protected set; }

    /// <summary>
    /// Gets the scene manager to load, unload, serialise, and deserialise scenes.
    /// </summary>
    public static SceneManager SceneManager { get ; protected set; }

    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    public static AssetManager AssetManager { get; protected set; }

    private static float _fixedUpdateCounter = 0.0f;
    private static float _fixedDeltaTime = 0.0166667f; // update the physics loop at 60hz;

    // debug purposes.
    public Texture2D DebugTexture{get; private set;}

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

        IsMouseVisible = mouseVisible;
    }

    protected override void Initialize(){
        base.Initialize();

        // set the graphics device to a reference to the base Game's
        GraphicsDevice = base.GraphicsDevice;

        // create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Create a new input manager.
        Input = new InputManager();

        DebugTexture = new Texture2D(GraphicsDevice, 1, 1);
        DebugTexture.SetData([Color.White]);

        #if DEV
        Console.WriteLine("[HOWLAPP] DEV CONFIGURATION.");
        #endif
        #if DEBUG
        Console.WriteLine("[HOWLAPP] DEBUG CONFIGURATION.");
        #endif
	}

    protected override void Update(GameTime gameTime){
        Input.Update(gameTime);
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        

        _fixedUpdateCounter += DeltaTime;
        while(_fixedUpdateCounter >= _fixedDeltaTime){
            FixedUpdate(gameTime);
            _fixedUpdateCounter -= _fixedDeltaTime;
        }

        base.Update(gameTime);
    }


    protected virtual void FixedUpdate(GameTime gameTime){
        
    }

    protected void SetFrameRate(float frameRate){
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0/frameRate);
    }

    protected void UncapFrameRate(){
        IsFixedTimeStep = false;
    }

    protected void EnableVSync(){
        Graphics.SynchronizeWithVerticalRetrace = true; // Disable V-Sync
        Graphics.ApplyChanges();
    }

    protected void DisableVSync(){
        Graphics.SynchronizeWithVerticalRetrace = false; // Disable V-Sync
        Graphics.ApplyChanges();        
    }
}



// ✅ Where to save data on consoles (and other platforms):
// Platform Type	Typical Writable Folder / API	Notes
// Windows/PC	Anywhere you have permissions (e.g., AppData, Documents)	Use %AppData% or user directories
// Xbox (UWP)	Windows.Storage.ApplicationData.Current.LocalFolder	Use UWP API to get the folder
// PlayStation	SaveData folder provided by SDK	Platform-specific API
// Nintendo Switch	Application Save Data folder via SDK	Platform-specific API
// Linux/macOS	User home directories (~/.config, ~/Documents, etc.)