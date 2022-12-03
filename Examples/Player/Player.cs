using System;
using Godot;
using SafeStrings;
using Rel = SafeStrings.Res.Examples.Player;

public partial class Player : Node
{
    private static PackedScene playerScene = GD.Load<PackedScene>(Rel.player_tscn);
    private static PackedScene coinScene = GD.Load<PackedScene>(Res.Examples.Items.Coin.coin_tscn);

    public static Player NewPlayer()
    {
        return playerScene.Instantiate<Player>();
    }

    public override void _Process(double delta)
    {
        var horizontalInput = Input.GetAxis(InputAction.MoveLeft, InputAction.MoveDown);

        // Move(horizontalInput, delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(InputAction.Jump))
        {
            // Jump();
            return;
        }
    }
}
