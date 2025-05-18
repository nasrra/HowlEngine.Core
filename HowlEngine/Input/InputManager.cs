using Microsoft.Xna.Framework;

namespace HowlEngine.Input;

public class InputManager{
    /// <summary>
    /// Gets the state information of keyboard input.
    /// </summary>
    public KeyboardInfo Keyboard {get; private set;}

    /// <summary>
    /// Gets the state information of mouse input.
    /// </summary>
    public MouseInfo Mouse {get; private set;}

    /// <summary>
    /// Gets the state information of a gamepad.
    /// </summary>
    public GamePadInfo[] GamePads {get; private set;} // Monogame supports up to four gamepads simultaneously (PlayerIndex 0-3).

    public InputManager(){
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();
        GamePads = new GamePadInfo[4];
        for(int i = 0; i < 4; i++){
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }
    }

    public void Update(GameTime gameTime){
        Keyboard.Update();
        Mouse.Update();
        for(int i = 0; i < 4; i++){
            GamePads[i].Update(gameTime);
        }
    }
}