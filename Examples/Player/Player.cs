using System;
using Godot;
using SafeStrings;
using Rel = SafeStrings.Res.Examples.Player;

public partial class Player : Node
{
    // Prefer second way of getting coin scene because it caches and only loads once per game
    private static PackedScene coinScene1 = GD.Load<PackedScene>(Res.Examples.Items.Coin.coin_tscn);
    private static PackedScene coinScene2 = Res.Examples.Items.Coin.coin_tscn /*.Value*/;

    public static Player Instantiate()
    {
        // Relative to folder containing script.
        return Rel.player_tscn.Value.Instantiate<Player>();
        // The same
        return ((PackedScene)Rel.player_tscn).Instantiate<Player>();
    }

    public override void _Ready()
    {
        // Preloads everything in the folder or subfolder
        Res.Preload();
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
