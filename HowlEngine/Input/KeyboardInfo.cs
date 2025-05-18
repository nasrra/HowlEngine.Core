using Microsoft.Xna.Framework.Input;

namespace HowlEngine.Input;

public class KeyboardInfo{
    /// <summary>
    /// Gets the state of the keyboard input during the previous update cycle.
    /// </summary>
    public KeyboardState PreviousState {get;private set;}

    /// <summary>
    /// Gets the state of the keyboard input during the current update cycle.
    /// </summary>
    public KeyboardState CurrentState {get;private set;}

    /// <summary>
    /// Creates a new KeybaordInfo.
    /// </summary>
    public KeyboardInfo(){
        CurrentState = Keyboard.GetState();
        // create an empty state since there is no previous input on the first frame.
        PreviousState = new KeyboardState();
    }

    /// <summary>
    /// Updates the state information about keyboard input.
    /// </summary>
    public void Update(){
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Returns a value indicating if the specified key is down.
    /// </summary>
    /// <param name="key">The specified key to check.</param>
    /// <returns>true if the key is currently down; otherwise false</returns>
    public bool IsKeyDown(Keys key){
        return CurrentState.IsKeyDown(key);
    }

    /// <summary>
    /// Returns a value indicating if the specified key is up.
    /// </summary>
    /// <param name="key">The specified key to check.</param>
    /// <returns>true if the key is currently up; otherwise false.</returns>
    public bool IsKeyUp(Keys key){
        return CurrentState.IsKeyUp(key);
    }

    /// <summary>
    /// Returns a value indicating if the specified key has just been pressed.
    /// </summary>
    /// <param name="key">The specified key to check.</param>
    /// <returns>true if the key has just been pressed; otherwise false.</returns>
    public bool IsKeyJustPressed(Keys key){
        return CurrentState.IsKeyDown(key) == true && PreviousState.IsKeyUp(key) == true;
    }

    /// <summary>
    /// Returns a value indicating if the specified key has just been pressed.
    /// </summary>
    /// <param name="key">The specified key to check.</param>
    /// <returns>true if the key has just been released; otherwise false.</returns>
    public bool IsKeyJustReleased(Keys key){
        return CurrentState.IsKeyUp(key) == true && PreviousState.IsKeyDown(key) == true;
    }
}