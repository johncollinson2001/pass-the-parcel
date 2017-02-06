# Introduction
Pass the parcel is a 2D "platform like" game, which requires you to operate two workers, who must pass parcels along conveyor belts and load them onto a truck. Parcels are spawned from the factory in the top right, and loaded onto the truck in the bottom left. 

You get 3 lives, and a dropped parcel results in a lost life. Each level requires a certain number of parcels to be loaded, and as the levels increase so does the difficulty (e.g. rate of parcels spawned, speed of the conveyors, randomness between spawns, etc etc) and the number of parcels required to be loaded. 

At the end of each level you get a 10 second break to regain your composure and let the workers have a slurp of their coffee! After so many levels, you move into a new worlds such as underwater, desert, mountains and the moon.

The game starts slowly, so you may be thinking after the first couple of levels that it's a bit easy - however it gets mighty tough further down the line.

The game can be played via the web GL build here:

http://johncollinson.info/pass-the-parcel

Please note the build is for a fixed size of 960x600 - so the text is mis-aligned if you make it full screen. Will fix this at some point :)

The controls are:

Left Worker: up = A, down = Z

Right Worker: up = K, down = M

# Features

The game is really still in demo territory, however is fully playable. Features include the main game scene, game menu, high score, changing backgrounds (as you level up) and a fully automated AI bot that will play the game when you're on the menu screen (check out how far the AI can get!).

# Technical Stuff
The game was created in Unity 3D, using C# scripting. In order to run the code you will need to download Unity 3D.

All game objects have their own controller. Collections of objects are managed by "manager" classes. There are also some special classes worth paying attention to such as the parcel spawner, level generator and game ai.

# Screenshots

##### Level 1
![level-one](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic4.png)

##### Level up
![level-up](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic5.png)

##### Life lost
![life-lost](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic6.png)

##### Game over
![game-over](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic7.png)

##### Mountain land (AI controlled)
![mountain-land](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic1.png)

##### Desert land (AI controlled)
![desert-land](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic2.png)

##### Underwater land (AI controlled)
![desert-land](https://raw.githubusercontent.com/johncollinson2001/pass-the-parcel/master/design/pic3.png)