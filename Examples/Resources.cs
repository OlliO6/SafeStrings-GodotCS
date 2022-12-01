using System;
using Godot;
using SafeStrings;

// Example of how to 
public partial class Resources : Node
{
    public PackedScene coinScene = GD.Load<PackedScene>(Res.Examples.Items.Coin.coin_tscn);
}
