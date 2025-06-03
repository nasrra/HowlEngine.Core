using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HowlEngine.Core.Input;

public class GamePadInfo{
    private TimeSpan _vibrationCounter = TimeSpan.Zero;

    /// <summary>
    /// Gets the index of the player this gamepad is for.
    /// </summary>
    public PlayerIndex PlayerIndex {get;}

    /// <summary>
    /// Gets the state of input for this gamepad during the previous update cycle.
    /// </summary>
    public GamePadState PreviousState {get; private set;}

    /// <summary>
    /// Gets the state of input for this gamepad during the currnet update cycle.
    /// </summary>
    public GamePadState CurrentState {get; private set;}

    /// <summary>
    /// Gets a value that indicates if this gamepad is currently conncted.
    /// </summary>
    public bool IsConnected => CurrentState.IsConnected;

    /// <summary>
    /// Gets the value of the left thumbstick for this gamepad.
    /// </summary>
    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

    /// <summary>
    /// Gets the value of the right thumbstick for this gamepad.
    /// </summary>
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

    /// <summary>
    /// Gets the value of the left trigger for this gamepad.
    /// </summary>
    public float LeftTrigger => CurrentState.Triggers.Left;

    /// <summary>
    /// gets the value of the right trigger for this gamepad.
    /// </summary>
    public float RightTrigger => CurrentState.Triggers.Right;


    /// <summary>
    /// Creates a new GamePadInfo for the gamepad connected at the specified player index.
    /// </summary>
    /// <param name="playerIndex">The index of the player for this gamepad.</param>
    public GamePadInfo(PlayerIndex playerIndex){
        PlayerIndex = playerIndex;
        CurrentState = GamePad.GetState(playerIndex);
        // create an empty gamepad state for PreviousState since there is no previous state on the first frame.
        PreviousState = new GamePadState();
    }

    /// <summary>
    /// Updates the state information for this gamepad input.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime){
        if(IsConnected == false){
            return;
        }
        
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if(_vibrationCounter > TimeSpan.Zero){
            _vibrationCounter -= gameTime.ElapsedGameTime;
            if(_vibrationCounter < TimeSpan.Zero){
                StopVibration();
            }
        }
    }

    /// <summary>
    /// Returns a value indicating whether the specified button is down.
    /// </summary>
    /// <param name="button">The specified button.</param>
    /// <returns>true, if the specified button is down; otherwise false.</returns>
    public bool IsButtonDown(Buttons button){
        return CurrentState.IsButtonDown(button);
    }

    /// <summary>
    /// Returns a value indicating whether the specified button is up.
    /// </summary>
    /// <param name="button">The specified button.</param>
    /// <returns>true, if the specified button is up; otherwise false.</returns>
    public bool IsButtonUp(Buttons button){
        return CurrentState.IsButtonUp(button);
    }

    /// <summary>
    /// Returns a value indicating whether the specified button has just been pressed.
    /// </summary>
    /// <param name="button">The specified button.</param>
    /// <returns>true, if the specified button has just been pressed; otherwise false.</returns>
    public bool IsButtonJustPressed(Buttons button){
        return CurrentState.IsButtonDown(button) == true && PreviousState.IsButtonUp(button) == true;
    }

    /// <summary>
    /// Returns a value indicating whether the specified button has just been released.
    /// </summary>
    /// <param name="button">The specified button.</param>
    /// <returns>true, if the specified button has just been released; otherwise false.</returns>
    public bool IsButtonJustReleased(Buttons button){
        return CurrentState.IsButtonUp(button) == true && PreviousState.IsButtonDown(button) == true;
    }

    /// <summary>
    /// Sets the vibration for all motors of this gamepad.
    /// </summary>
    /// <param name="strength">The strength of the vibration from 0.0f (none) to 1.0f (full).</param>
    /// <param name="time">The amount of time the vibration should occur.</param>
    public void SetVibration(float strength, TimeSpan time){
        _vibrationCounter = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    /// <summary>
    /// Stop the vibration of all motors for this gamepad.
    /// </summary>
    public void StopVibration(){
        GamePad.SetVibration(PlayerIndex, 0, 0);
    }
}