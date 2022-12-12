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

Select your script and click on `Project/Tools/SafeStrings/Generate Rel Using` or press `ctrl + r` and this line of code will get generated:
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
By default the scene field will be set to the currently open scene and the script to the script attached to the root node.

When a scene is associated the plugin will add a static class called `Scene`.

```c#
public override void _Process(double delta)
{

}
```



