using Rel = SafeStrings.Res.Examples.Player;
using System;
using Godot;
using SafeStrings;

// Note: Player scene has a really weird structure (for example showcase purpose)

public partial class Player : Node, IScene<Player>
{
    public const float MovementSpeed = 400;

    // Prefer second way of getting coin scene because it caches and only loads once per game
    private static PackedScene coinScene1 = GD.Load<PackedScene>(Res.Examples.Items.Coin.coin_tscn);
    private static PackedScene coinScene2 = Res.Examples.Items.Coin.coin_tscn /*.Value (the same)*/ ;

    // Reference to use often in script (a bit using like @onready in gdscript)
    public Sprite2D Sprite => Scene.Sprite2D.GetCached(this);
    public AnimationTree AnimTree1 => Scene.Sprite2D.Node.AnimationTree.GetCached(this);
    public AnimationTree AnimTree2 => this.GetSceneNodeCached(Scene.Sprite2D.Node.AnimationTree.Path);

    public static Player NewPlayer()
    {
        // Relative to folder containing script.
        return Rel.player_tscn.Value.Instantiate<Player>();
        // The same
        return ((PackedScene)Rel.player_tscn).Instantiate<Player>();
    }

    public override void _Ready()
    {
        // No need to cache since we're only getting it once
        Scene.Unique.AnimationPlayer.Get(this).AnimationFinished += animation =>
        {
            GD.Print("Animation ", animation, " finished.");
        };
    }

    public override void _Process(double delta)
    {
        var horizontalInput = Input.GetAxis(InputAction.MoveLeft, InputAction.MoveRight);

        Sprite.Position += (float)delta * MovementSpeed * horizontalInput * Vector2.Right;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(InputAction.Jump))
        {
            // Jump();
        }
    }

    public void OnInstanced()
    {
        // Called When Player.Instantiate was called
    }
}
