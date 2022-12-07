namespace SafeStrings;

public static class Res
{
    public static readonly string README_md = "res://README.md";
    public static class Examples
    {
        public static class Items
        {
            public static class Coin
            {
                public static readonly ResourcePath<Godot.PackedScene> coin_tscn = "res://Examples/Items/Coin/coin.tscn";
            }
        }
        public static class Player
        {
            public static readonly ResourcePath<Godot.CSharpScript> Player_cs = "res://Examples/Player/Player.cs";
            public static readonly ResourcePath<Godot.PackedScene> player_tscn = "res://Examples/Player/player.tscn";
        }
    }
}