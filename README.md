<!DOCTYPE md>

COMP 476 Assignment 1
------------------------------

Build Instructions
------------------------------

On a computer having Unity installed, open up the project by adding it to Unity Hub and running it. This will open the project in Unity. Clicking File -&gt; Build and Run will build the project. The executable can then be found in the Build folder of the project folder.

File structure
-------------------------------

The classes necessary for the program are found in four files:

1. PlaneWall.cs

Contains the functions required to detect what happens when a player goes through a wall and to set the boundaries of the map.

2. Car.cs

Contains the logic and parameters for car collisions as well as an update function to move the characters every frame based on the type of movement they are currently using. It also contains all the relevant information for cars such as if the car is a target, a seeker, its position, speed, max speed, etc.

3. GameManager.cs

Contains the logic for the game. Has an initializer function that sets the initial seeker and target as well as logic for when the target is caught. It also handles when to reinitialize the game, i.e. when all the characters have been caught.

4. Movement.cs

This file contains an inheritance heirarchy that contains the logic for all the movement types in the game. This is explained more in the class structure section of this document.

Class Structure
-------------------------------

The most important thing to note here is the inheritance heirarchy for the Movement types. This is broken down as follows.

At the top of the heirarchy is a Movement class that contains all the fields that all movements have in common, as well as some abstract functions that all the child classes will implement.

Next down is AlignedMovement. This class ensures that the character aligns as it moves (or before it moves). This is decided by which function is called based on distance to target, etc. 

From here we have three classes (ReachMovement (abstract) and EvadeMovement (abstract) and Wander). Below ReachMovement are KinematicSeek and KinematicArrive. Below Kinematic Arrive is the Pursue movement class.

From Evade Movement we have Kinematic Flee and below Kinematic Flee we have Evade.

The Car class has an AlignedMovement field that can be changed at runtime. In its update method it calls Move from its Movement class which will execute move based on the type of movement that the car is currently using.

Colors
-------------------------------

Red: Seeker.<br />
Blue: Regular AI character or Frozen Character.<br />
Green: AI Character being chased.

AI Behaviours
-------------------------------

When an AI character is the seeker, the behaviour used is Pursue. This makes the seeker track another AI character until it has been caught. When this happens the chased character changes.

When an AI character is the target of the seeker, it will use Evade. When it is caught, the character does not move until it has been freed (explained below). Once it is freed, it will use the Wander behaviour.

When an AI character is not being chased and there is a frozen character, this Ai character will use KinematicArrive to try and reach the frozen character in order to free him. Otherwise, these characters use the wander behaviour.

If while being chased, the chased character goes through too many walls, inducing deadlock in the chase since the characters will just keep turning around, the target being chased will change to break the deadlock.