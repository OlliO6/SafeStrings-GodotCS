namespace Game.Player;
partial class Player
{
    public static class Scene
    {
        public static partial class Node2D
        {
            public static SafeStrings.SceneNodePath<Godot.Node2D> Path = "./Node2D";
            public static Godot.Node2D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Node2D GetCached(Godot.Node root) => Path.GetCached(root);
        }
        partial class Node2D { public static partial class Node
        {
            public static SafeStrings.SceneNodePath<Godot.Node2D> Path = "./Node2D/Node";
            public static Godot.Node2D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Node2D GetCached(Godot.Node root) => Path.GetCached(root);
        }}
        partial class Node2D { partial class Node { public static partial class Sprite2D
        {
            public static SafeStrings.SceneNodePath<Godot.Sprite2D> Path = "./Node2D/Node/Sprite2D";
            public static Godot.Sprite2D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Sprite2D GetCached(Godot.Node root) => Path.GetCached(root);
        }}}
        public static partial class Node3D
        {
            public static SafeStrings.SceneNodePath<Godot.Node3D> Path = "./Node3D";
            public static Godot.Node3D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Node3D GetCached(Godot.Node root) => Path.GetCached(root);
        }
        public static partial class Node3D2
        {
            public static SafeStrings.SceneNodePath<Godot.Node3D> Path = "./Node3D2";
            public static Godot.Node3D Get(Godot.Node root) => Path.Get(root);
            public static Godot.Node3D GetCached(Godot.Node root) => Path.GetCached(root);
        }
        
        public static class Unique
        {
            public static class Node
            {
                public static SafeStrings.SceneNodePath<Godot.Node2D> Path = Node2D.Node.Path;
                public static Godot.Node2D Get(Godot.Node root) => Path.Get(root);
                public static Godot.Node2D GetCached(Godot.Node root) => Path.GetCached(root);
            }
            
        }
    }
}
