Safe Strings For Godot 4 Mono
=============================

This plugin tries to kill the need of most magic strings and adds some extra functionalities.

💭 Setup:
-----
- Copy the addons folder of this repository and paste it in your Godot 4 mono project.
- Enable the `SafeStrings` plugin.
- Add `using SafeStrings;` to all your scipts or just add a global using for it.

🕹️ Input Actions:
-----------------
The first improvement is about Godot's input actions.
A static class called `InputAction` will get generated automatically, it will contain a static readonly `StringName` for each input action added in the project settings input map.
```c#
Input.IsActionPressed("action_name");
// Becomes
Input.IsActionPressed(InputAction.ActionName);
```

📂 Resource Paths:
------------------
A static class called `Res` will get generated it will contain other static classes for each folder to recreate the resource file structure.
These classes will also contain fields of the `ResourcePath<T>` class for every resource.

> The `ResourcePath<T>` class can be implicity converted to a string, just like e.g. `StringName`.
> The generic type `T` is the type of the resource where it points to.
> It has a property of the `T` type called `Value`, getting it will return the resource where it's pointing to, it also caches automatically.
> Use `Preload()` to load the cache (you do not need to do that).

Example:
```c#
Res.Enemies.zombie_tscn.Value.Instantiate();
```

#### Relative Paths:
If you have a file structure where for example the player script is in the same folder as the player scene, this feature will be useful.

Select your script and click on `Project/Tools/SafeStrings/Generate Rel Using` or press `ctrl + alt + r` and this line of code will get generated:
```c#
using Rel = SafeStrings.Res.Examples.Player;
```
So now you can do something like:
```c#
public static Player NewPlayer()
{
    return Rel.player_tscn.Value.Instantiate<Player>();
}
```

🪵 Node Paths:
----------

You can associate a scene to a script using `Project/Tools/SafeStrings/Associate Scene to script`
Or press `ctrl + alt + s`.

Than a window will popup where you associate a scene to a script.
By default the scene field will be set to the currently open scene and the script to the one attached to the root node.

When a scene is associated the plugin will add a static class called `Scene` to the given script class.
This class will have static subclasses that are recreating the scene structure (one subclass for every node).

Every "Node" class will have a static property called `Path` wich is of the `SceneNodePath<T>` type.

The `SceneNodePath<T>` class is a wrapper around the `NodePath` class.
- `T` is the type of the node where the path. `GetType()` returns the type of `T`.
- Implicity convertable to `NodePath`.
- `Get(Node root)` method is similar to `Node.GetNode<T>(NodePath path)`.
- `GetCached(Node root)` is like `Get` but will cache the result in a [ConditionalWeakTable](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.conditionalweaktable-2?view=net-7.0).

Every 'Node' class will also have methods, `Get(Node root)` and `GetCached(Node root)`, these are just shortcuts for methods on `Path`.

### Examples:

```c#
Scene.NodeName.ChildOfNodeName.Get(this).SomeMethod();
// Same as 
this.GetNode<NodeType>("NodeName/ChildOfNodeName").SomeMethod();
// Or
this.GetNode<NodeType>(Scene.NodeName.ChildOfNodeName.Path).SomeMethod();
```

When you wanna use a Node more often (e.g. in the `_Process` function) it's a good practise to cache it.
I always liked to use [lazy getters with the null-coalescing-operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator) instead of setting a node reference in the `_Ready()` function.

It looked something like this:
```c#
private NodeType _myNode;

public MyNode => _myNode ??= GetNode<NodeType>("Node/MyNode"); // Scene.Node.MyNode.Get(this);
```
But with this plugin I can just do this:
```c#
public MyNode => Scene.Node.MyNode.GetCached(this);
```

It's kind of like `@onready ... get_node` in gdscript but instead of assigning the field on ready it assigns it at the first use.

Contributing:
------------
I'd love to see a PR.
