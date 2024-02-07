Thank you for considering me!
I'm sorry that I can't provide this game as its Unity project but the best I can do is the game itself and my scripts decompiled out of the build using dnSpy (https://github.com/dnSpy/dnSpy)

This game was originally made as an assessed project for university in 2022.
The game is very short and I encourage you to try it to better understand what the scripts do
- PlayerBehaviour: Attached your player character (Neon) and handled inputs
- GrappleHeadBehaviour: PlayerBehaviour can instantiate an object with this script. That object acts like a grapple hook and this script encourages that.
- AreaBehaviour: As you move through the game you may notice the camera snaps to different views. Each view has a boundary which via this script moves the camera and player to the next area which takes place in the same scene.

By the way, I'm aware that in each of these scripts I use GetComponent without caching way too much. It's horrible and I won't do it again (promise).