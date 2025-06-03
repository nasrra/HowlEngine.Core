using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HowlEngine.Core.Input;

public class MouseInfo{
    /// <summary>
    /// The state of the mouse input during the previous update cycle.
    /// </summary>
    public MouseState PreviousState {get; private set;}

    /// <summary>
    /// The state of the mouse input during the current update cycle.
    /// </summary>
    public MouseState CurrentState {get; private set;}

    /// <summary>
    /// Gets or Sets the current position of the mouse cursor in screen space.
    /// </summary>
    public Point Position{
        get => CurrentState.Position;
        set => SetPosition(value.X, value.Y);
    }

    /// <summary>
    /// Gets or Sets the current x-coordinate position of the mouse cursor in screen space.
    /// </summary>
    public int X{
        get => CurrentState.X;
        set => SetPosition(value, CurrentState.Y);
    }

    /// <summary>
    /// Gets or Sets the current y-coordinate position of the mouse cursor in screen space.
    /// </summary>
    public int Y{
        get => CurrentState.Y;
        set => SetPosition(CurrentState.X, value);
    }

    /// <summary>
    /// Gets the difference in the mouse cursor position between the previous and current frame.
    /// </summary>
    public Point PositionDelta => CurrentState.Position - PreviousState.Position;

    /// <summary>
    /// Gets the difference in the mouse cursor x-posiiton between the previous and current frame.
    /// </summary>
    public int XDelta => CurrentState.X - PreviousState.X;

    /// <summary>
    /// Gets the difference in the mouse cursor y-posiiton between the previous and current frame.
    /// </summary>
    public int YDelta => CurrentState.Y - PreviousState.Y;

    /// <summary>
    /// Gets a value indicating whether the mouse has been moved between the previous and current frame.
    /// </summary>
    public bool WasMoved => PositionDelta != Point.Zero;

    /// <summary>
    /// Gets the cumulative value of the mouse scroll wheel since the start of the game.
    /// </summary>
    public int ScrollWheel => CurrentState.ScrollWheelValue;

    /// <summary>
    /// Gets the difference in the scroll wheel value between the previous and current frame.
    /// </summary>
    public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    /// <summary>
    /// Creates a new MouseInfo.
    /// </summary>
    public MouseInfo(){
        CurrentState = Mouse.GetState();
        // Create an empty state for PreviousState since there is no previous input on the first frame.
        PreviousState = new MouseState();
    }

    /// <summary>
    /// Updates the state information about mouse input.
    /// </summary>
    public void Update(){
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// Returns a value that indicates whether the specified mouse button is currently down.
    /// </summary>
    /// <param name="button">The specified mouse button to check.</param>
    /// <returns>true, if the mouse button is down; otherwise false.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool IsButtonDown(MouseButton button){
        switch(button){
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Pressed;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Pressed;
            case MouseButton.Middle: 
                return CurrentState.MiddleButton == ButtonState.Pressed;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed;
            default:
                throw new InvalidOperationException(button+" is not a valid mouse input!");
        }
    }

    /// <summary>
    /// Returns a value that indicates whether the specified mouse button is currently up.
    /// </summary>
    /// <param name="button">The sprcified mouse button.</param>
    /// <returns>true, if the mouse button is up; otherwise false.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool IsButtonUp(MouseButton button){
        switch(button){
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Released;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Released;
            case MouseButton.Middle: 
                return CurrentState.MiddleButton == ButtonState.Released;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Released;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Released;
            default:
                throw new InvalidOperationException(button+" is not a valid mouse input!");
        }
    }

    /// <summary>
    /// Returns a value that indicates whether the specified mouse button has just been released.
    /// </summary>
    /// <param name="button">The specified mouse button.</param>
    /// <returns>true, if the mouse button has just been released; otherwise false.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool IsButtonJustReleased(MouseButton button){
        switch(button){
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
            case MouseButton.Middle: 
                return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Released && PreviousState.XButton1 == ButtonState.Pressed;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Released && PreviousState.XButton2 == ButtonState.Pressed;
            default:
                throw new InvalidOperationException(button+" is not a valid mouse input!");
        }
    }

    /// <summary>
    /// Returns a value that indicates whether the specified mouse button has just been pressed.
    /// </summary>
    /// <param name="button">The specified mouse button.</param>
    /// <returns>true, if the mouse button has just been pressed; otherwise false.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool IsButtonJustPressed(MouseButton button){
        switch(button){
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
            case MouseButton.Middle: 
                return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed && PreviousState.XButton1 == ButtonState.Released;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed && PreviousState.XButton2 == ButtonState.Released;
            default:
                throw new InvalidOperationException(button+" is not a valid mouse input!");
        }
    }

    /// <summary>
    /// Sets the current position of the mouse cursor in screen space and updates the CurrentState witht the new position.
    /// </summary>
    /// <param name="x">The x-coordinate location of the mouse cursor in screen space.</param>
    /// <param name="y">The y-coordinate location of the mouse cursor in screen space.</param>
    public void SetPosition(int x, int y){
        Mouse.SetPosition(x, y);
        CurrentState = new MouseState(
            x,
            y,
            CurrentState.ScrollWheelValue,
            CurrentState.LeftButton,
            CurrentState.MiddleButton,
            CurrentState.RightButton,
            CurrentState.XButton1,
            CurrentState.XButton2
        );
    }

}