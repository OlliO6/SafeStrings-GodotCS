partial class Player
{
    public static class Scene
    {
        public static partial class Sprite2D
        {
            public static SafeStrings.SceneNodePath<Godot.Sprite2D> Path = "./Sprite2D";
            public static Godot.Sprite2D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Sprite2D GetCached(Godot.Node root) => Path.GetCached(root);
        }
        partial class Sprite2D { public static partial class Area2D
        {
            public static SafeStrings.SceneNodePath<Godot.Area2D> Path = "./Sprite2D/Area2D";
            public static Godot.Area2D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Area2D GetCached(Godot.Node root) => Path.GetCached(root);
        }}
        partial class Sprite2D { partial class Area2D { public static partial class CollisionShape2D
        {
            public static SafeStrings.SceneNodePath<Godot.CollisionShape2D> Path = "./Sprite2D/Area2D/CollisionShape2D";
            public static Godot.CollisionShape2D Get(Godot.Node root) => Path.Get(root);
            public static Godot.CollisionShape2D GetCached(Godot.Node root) => Path.GetCached(root);
        }}}
        partial class Sprite2D { public static partial class Node
        {
            public static SafeStrings.SceneNodePath<Godot.Node> Path = "./Sprite2D/Node";
            public static Godot.Node Get(Godot.Node root) => Path.Get(root);
            public static Godot.Node GetCached(Godot.Node root) => Path.GetCached(root);
        }}
        partial class Sprite2D { partial class Node { public static partial class AnimationPlayer
        {
            public static SafeStrings.SceneNodePath<Godot.AnimationPlayer> Path = "./Sprite2D/Node/AnimationPlayer";
            public static Godot.AnimationPlayer Get(Godot.Node root) => Path.Get(root);
            public static Godot.AnimationPlayer GetCached(Godot.Node root) => Path.GetCached(root);
        }}}
        partial class Sprite2D { partial class Node { public static partial class AnimationTree
        {
            public static SafeStrings.SceneNodePath<Godot.AnimationTree> Path = "./Sprite2D/Node/AnimationTree";
            public static Godot.AnimationTree Get(Godot.Node root) => Path.Get(root);
            public static Godot.AnimationTree GetCached(Godot.Node root) => Path.GetCached(root);
        }}}
        
        public static class Unique
        {
            public static class AnimationPlayer
            {
                public static SafeStrings.SceneNodePath<Godot.AnimationPlayer> Path = Sprite2D.Node.AnimationPlayer.Path;
                public static Godot.AnimationPlayer Get(Godot.Node root) => Path.Get(root);
                public static Godot.AnimationPlayer GetCached(Godot.Node root) => Path.GetCached(root);
            }
            
        }
    }
}
