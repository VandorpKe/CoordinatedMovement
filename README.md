# CoordinatedMovement

## Introduction

I am a student from Howest and am currently studying DAE with a major in Game Development and below you will find my research on AI-Coordinated Movement. 

This will be a simple implementation on how you could add coordinated movement to your game. Of course there is more than one way to implement coordinated movement, but this will just be a simple implementation as the movement will happen on a flat surface and not a complicated surface with hills, etc.

## Implementation

### Selection

To be able to move units around the field we are first gonna need the ability to select some units. This can be done by a number of ways, the mosst common ways are as followed. You will be able to select a unit with just LeftClick or by Shift-LeftClicking you can select multiple units. There's also the option to Click and Drag on the screen to select multiple units.

To not confuse the player while selecting, we have to visualize this selection rectangle. This will give a nice and clean visual feedback on which units will be selected. These will be the ones that are in this rectangle ofcourse. It's best to also visualize which units we have selected.

### Navigation

Unity has a built in navigation mesh that you can bake on you world. This navigation mesh will be used to easily guide the units around the field. This is ofcourse just a simple solution, as it will bring some problems forth.

As we are moving as a group to a certain destination, it is not possible for all units to reach this goal destination. They will try to overlap and push the units to reach their destination, but that is also the destination of the other units in that group. This is one of the problems that we will try to solve.

#### Problem

A problem that occurs is that the units will push eachother around to get to their destination. To fix this you have to bake in their position into the navigation mesh. A 'NavMeshAgent' that we are using in unity doesn't have that ability, but a 'NavMeshObstacle' does have that ability. But the 'NavMeshObstacle' doesn't move.

Now here is a big but, we can NOT use them together on a unit. Of course there is a way to get around that. When we initiate an unit we'll just disable the 'NavMeshObstacle'. Now when the unit is stationary we'll have to use our 'NavMeshObstacle' to bake our position. The order that we enable and disable our meshes is very important here, we first want to disable our 'NavMeshAgent' and only after that we enable out 'NavMeshObstacle'. In reverse we'll have to wait a frame to enable out 'NavMeshAgent' again or ther will be shifting.

![ezgif com-gif-maker](https://user-images.githubusercontent.com/113976115/213511720-6df2b7de-02d8-4226-b3ae-b33e7689c1b6.gif)

![image](https://user-images.githubusercontent.com/113976115/213511905-ae733bee-cc5f-41a6-b2d8-e92055f9a3dc.png)

### Formation

As seen already above, it is possible to move the agents as a square. This is calculated by the clicked point on the plane. When you click you will automatically calculate all the points around that point that is possible to go to for the amount of agents that are selected. So there is already a grid for you ready when you have clicked. The algorithm that I have made isn't the most optimal or efficient but it is the easiest for me to understand.

![image](https://user-images.githubusercontent.com/113976115/213533542-a2f8f12a-0ebf-4a48-82a4-819c74e44b07.png)

![image](https://user-images.githubusercontent.com/113976115/213535293-ebb73b79-caae-412e-8fde-d11ee0b43f60.png)

First we have to add our 'startPoint' to our 'listOfPoints'. Then it will make a spiral grid around the unit with the amount of spacing that you have given it with the 'directionX'.

There is and optimization in there that will thake the shortest path to the next point, but this is only relevant when you are making a new rectangle or circle. When you try to calculate the shortest distance when you want yo move your formation, it will screw up the formation. But in case you were wondering this is it:

![image](https://user-images.githubusercontent.com/113976115/213534319-322b146c-5358-42ae-839b-01a11750f90f.png)

#### Different formations

There are of course more than one possible ways than just a rectangle ex. circle, hexagon, line, curtom draw, ... . I only have two, a rectangle and a circle formation. The circle formation is good for surrounding for example enemies. 

You can select a preset formation by the following :
- [C] Will set the preset after you click to circle, so this will not change instantly.
- [R] Will set the preset to rectangle.

## State

Currently the formation is very basic. There is no efficiency and it's not perfect but it's a very basic implementation if coordinated movement.

## Sources

- https://sandruski.github.io/RTS-Group-Movement/
- https://www.gamedeveloper.com/programming/group-pathfinding-movement-in-rts-style-games
- https://www.gamedeveloper.com/blogs/rts-unit-group-movement-deterrence---video-devlog-5
