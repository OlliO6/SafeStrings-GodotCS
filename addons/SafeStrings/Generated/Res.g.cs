namespace SafeStrings;

public static class Res
{
    public static class Examples
    {
        public static class Items
        {
            public static class Coin
            {
                public static readonly ResourcePath<Godot.PackedScene> coin_tscn = "res://Examples/Items/Coin/coin.tscn";
                public static void Preload()
                {
                    coin_tscn.Preload();
                }
                public static System.Threading.Tasks.Task PreloadAsync(bool parallel, int updateDelayMSec = 50, bool useSubThreads = false)
                {
                    if (parallel)
                        return System.Threading.Tasks.Task.Run(() => System.Threading.Tasks.Task.WaitAll(
                            coin_tscn.PreloadAsync(updateDelayMSec, useSubThreads)));
                    return System.Threading.Tasks.Task.Run(async () =>
                    {
                        await coin_tscn.PreloadAsync(updateDelayMSec, useSubThreads);
                    });
                }
            }
            public static void Preload()
            {
                Coin.Preload();
            }
            public static System.Threading.Tasks.Task PreloadAsync(bool parallel, int updateDelayMSec = 50, bool useSubThreads = false)
            {
                if (parallel)
                    return System.Threading.Tasks.Task.Run(() => System.Threading.Tasks.Task.WaitAll(
                        Coin.PreloadAsync(parallel, updateDelayMSec, useSubThreads)));
                return System.Threading.Tasks.Task.Run(async () =>
                {
                    await Coin.PreloadAsync(parallel, updateDelayMSec, useSubThreads);
                });
            }
        }
        public static class Player
        {
            public static readonly ResourcePath<Godot.CSharpScript> Player_cs = "res://Examples/Player/Player.cs";
            public static readonly ResourcePath<Godot.PackedScene> player_tscn = "res://Examples/Player/player.tscn";
            public static void Preload()
            {
                Player_cs.Preload();
                player_tscn.Preload();
            }
            public static System.Threading.Tasks.Task PreloadAsync(bool parallel, int updateDelayMSec = 50, bool useSubThreads = false)
            {
                if (parallel)
                    return System.Threading.Tasks.Task.Run(() => System.Threading.Tasks.Task.WaitAll(
                        Player_cs.PreloadAsync(updateDelayMSec, useSubThreads), 
                        player_tscn.PreloadAsync(updateDelayMSec, useSubThreads)));
                return System.Threading.Tasks.Task.Run(async () =>
                {
                    await Player_cs.PreloadAsync(updateDelayMSec, useSubThreads);
                    await player_tscn.PreloadAsync(updateDelayMSec, useSubThreads);
                });
            }
        }
        public static void Preload()
        {
            Items.Preload();
            Player.Preload();
        }
        public static System.Threading.Tasks.Task PreloadAsync(bool parallel, int updateDelayMSec = 50, bool useSubThreads = false)
        {
            if (parallel)
                return System.Threading.Tasks.Task.Run(() => System.Threading.Tasks.Task.WaitAll(
                    Items.PreloadAsync(parallel, updateDelayMSec, useSubThreads), 
                    Player.PreloadAsync(parallel, updateDelayMSec, useSubThreads)));
            return System.Threading.Tasks.Task.Run(async () =>
            {
                await Items.PreloadAsync(parallel, updateDelayMSec, useSubThreads);
                await Player.PreloadAsync(parallel, updateDelayMSec, useSubThreads);
            });
        }
    }
    public static void Preload()
    {
        Examples.Preload();
    }
    public static System.Threading.Tasks.Task PreloadAsync(bool parallel, int updateDelayMSec = 50, bool useSubThreads = false)
    {
        if (parallel)
            return System.Threading.Tasks.Task.Run(() => System.Threading.Tasks.Task.WaitAll(
                Examples.PreloadAsync(parallel, updateDelayMSec, useSubThreads)));
        return System.Threading.Tasks.Task.Run(async () =>
        {
            await Examples.PreloadAsync(parallel, updateDelayMSec, useSubThreads);
        });
    }
}