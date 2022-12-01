using System;
using Godot;
using SafeStrings;

// Example of how the InputAction class can be used
public partial class InputActions : Node
{
    public override void _Process(double delta)
    {
        var horizontalInput = Input.GetAxis(InputAction.MoveLeft, InputAction.MoveDown);

        // Move(horizontalInput, delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsEcho())
            return;

        if (@event.IsActionPressed(InputAction.Jump))
        {
            // Jump();
            return;
        }
        if (@event.IsAction(InputAction.Crouch))
        {
            if (@event.IsPressed())
            {
                // StartCrouch();
                return;
            }
            // StopCrouch():
            return;
        }
    }
}
