09/02/24
--Jiancheng
ver.0.0.0 
Added some free art resources and downloaded the necessary packages.

13/02/24
--Jiancheng
ver.0.0.1
Used free materials to build the player's state machine, completing the logic for movement and jumping.

15/02/24
--Michael
ver.0.0.2
Basic audio and music system implemented. Trigger audio effects with MusicManager.PlayAudio(). Music with ChangeTrack(), or StartPlaylist(). This is the first implementation and will be further expanded with comments, features required by the audio designer, and better overall code.

16/02/24
--Michael
ver.0.0.4
Implemented main menu functionality, with a play button, options, credits, and quitting.

17/02/24
--Michael
ver.0.0.5
Fixed bug with global manager being replaced by scene changes, also fixes issue with music not crossfading between scenes. Added menu slider images, error sound, & highlight button option.

17/02/24
--Jiancheng
ver.0.0.5
Fixed the bug, increased the boss state machine to three types, tried the third skill, and some special effects. Determined the scaling size of the assets.

18/02/24
Important modification, unifying the version to 2021.3.35f1 LTS.

18/02/24
--Jiancheng
Fixed all known bugs related to the camera, boss, and map. The number of bugs to be resolved is 0.

19/02/24
--Michael
ver.0.0.6
Added saving and loading for all menu settings & option to revert to default settings.

20/02/24
--Michael
ver.0.0.7
Added leaderboard functionality for saving player times and viewing the times in the menu & added controller support to the menu. Also added difficulty option that scales the amount of damage you recieve.

21/02/24
--Michael
ver.0.0.8
Main menu has temp art assets to replicate book theme, with page turning animations, sounds, and a new temp font.

22/02/24
--Michael
ver.0.0.9
Journal system, fixed bug with button description not updating when opening the leaderboard, added delete data option, leaderboard now sorted from shorted to longest time.

22/02/24
--Michael
ver.0.0.10
Imported many visual assets from the team. Started constructing the TestMap scene. Added multiple "sub page" functionality, for when there are more buttons than space on the main menu. Improved journal layout and fixed bugs. Added option to load either the TestPlayer or TestMap scene from the main menu for testing.

24/02/24
--Jiancheng
ver.0.1.0
Replaced the protagonist with a free resource that is closer to our actual protagonist, changed the jump to a dash, and tried adding some special effects, but it caused an unknown bug, so it was temporarily removed. This is the current effect, a bug-free dash.

24/02/24
--Jiancheng
ver.0.1.1
Added pizza, Mimic Enemy.To ensure item drops after an enemy's death, added the ItemManager script to GlobalManager.

26/02/24
--Jiancheng
ver.0.1.2
Completed the initial version of the inventory system, utilizing a database to store objects.

26/02/24
--Michael
ver.0.1.3
Implemented the first version of the level. Fixed bug with menu button sound pitch getting too high when going across subpages.

27/02/24
--Michael
ver.0.1.4
Added individual classrooms, added more map details, added door functionality.

28/02/24
--Michael
ver.0.1.5
Added journal entries for items, fixed some bugs.

29/02/24
--Michael
ver.0.1.6
Bug fixes for the game jam test session. Added end level trigger, mimics now do damage, replaced health bar with new one in testmap level (not implemented fully), changed leaderboard logic, added swing sounds.

29/02/24
--Jiancheng
ver.0.1.7
Completed the inventory system, implemented features such as item storage and retrieval, drag-and-drop, item stacking, and preventing items that do not meet the requirements from being dragged into the toolbar. The text description and item usage parts will be completed next week.

02/03/24
--Michael
ver.0.1.8
Added title screen to the main menu, and the first implementation of the dialogue system. Replaced menu book art with copyright free version until final art is implemented.

03/03/24
--Michael
ver.0.1.9
Added students that can be rescued with a ui counter, added dialogue triggers, fixed healthbar not aligning correctly, fixed title screen only displaying when the game is first loaded, fixed bug with reflections.

07/03/24
--Jiancheng
ver.0.2.0
Completed the double-click use of items in the Inventory, the binding of keys in the Action Bar, the display of item information when hovering the mouse, and eliminated some bugs.

03/03/24
--Michael
ver.0.2.1
Maths teacher & paper planes.

04/03/24
--Michael
ver.0.2.2
Added animation to maths teacher and impact physics, added temp art. fixed bugs.

04/03/24
--Steph
ver.0.2.3
Beginning to assemble assets for the Tutorial Classroom :)

04/03/24
--Steph
ver.0.2.4
Continued development to tutorial classroom.

11/03/24
--Michael
ver.0.2.5
Fixed bug with mouse locking, added boss music as temporary level music, added light effects with optional flickering, improved audio of basketball, made locker objects spawn from prefab instead of activating an object, added random physics and rotation to locker object spawns, improved math teacher ai.
NOTE: The player sprites order in layer has changed to 0 in order to get the lighting working, i've adjusted the objects in testmap to work with this.

12/03/24
--Michael
ver.0.2.6
Added depth to testmap, made lockers interactable with the key, added interact icons, added temp sounds for doors and lockers.

13/03/24
--Jiancheng
ver.0.2.7
Switched the main character's materials to our own creations. 

15/03/24
--Jiancheng
ver.0.2.8
Completed the controller adaptation for the Inventory and made adjustments to the Inventory functionality, removing the Action Bar.

15/03/24
--Michael
ver.0.2.9
Added lift system, reset enemy positions when you enter a door.

15/03/24
--Michael	
ver.0.3.0
Added some of the math teacher animations, fixed bugs.

16/03/24
--Michael	
ver.0.3.1
Added logic for math book attacking and dying, paper plane now only damages when it hits the top of your head. Added function to change music volume midgame. Fixed issues with destructible objects, added cheat code, and added temporary magic book sprite.

18/03/24
--Jiancheng
ver.0.3.2
Fixed the bug that prevented attacking multiple enemies at the same time.
Added an attack interval to prevent multiple attacks in a short period.
Imported the attack animation.

16/03/24
--Michael	
ver.0.3.3
Fixed a bunch of bugs, added loading screen to elevator transition, made a separate scene for the final level, added dust particles to lights.

17/03/24
--Michael	
ver.0.3.4
Fixed bugs, added dash effect, added loading screen tips and animation.

17/03/24
--Michael	
ver.0.3.5
Fixed bugs, added item pickup sounds.

21/03/24
--Jiancheng
ver.0.3.6
Fixed bugs,can only use health recovery items when you are injured.

21/03/24
--Michael
ver.0.3.7
Added audio effect fading support, added interact icon, fixed bugs, started new level block-out.

21/03/24
--Jiancheng
ver.0.3.8
Added dash skill cooling effect

21/03/24
--Jiancheng
ver.0.3.9
Added GetHit cooling effect

21/03/24
--Steph
ver.0.4.0
Added asset, Created rough gym layout in 'Level' Scene.

23/03/24
--Michael
ver.0.4.1
Added more behaviour to the magic book, changed loading screen transition, fixed bugs, added student sprite, added first version of level music.

26/03/24
--Jiancheng
ver.0.4.2
Fixed the bug switching Scenes losting the CD image

28/03/24
--Michael
ver.0.4.3
Optimised some of my scripts, implemented new buzzing sound for lights.

29/03/24
--Michael
ver.0.4.4
Added game over logic with name entry.

29/03/24
--Michael
ver.0.4.5
Added godmode cheat (hold keys: g, o, d), fixed bug with destruction cheat, added level new level music, added music function to manage delayed track changes, fixed other bugs.

31/03/24
--Michael
ver.0.4.6
Added code comments, small optimisations.

16/04/24
--Michael
ver.0.4.7
First implementation of multi-language support for the main menu interface and dialogue display. 

17/04/24
--Michael
ver.0.4.8
Implemented audio effects.

18/04/24
--Michael
ver.0.4.9
Implemented more audio effects, opening video, and new music. Fixed bug with video resolution option not working, added flash transition to the video>menu change.

19/04/24
--Jiancheng
ver0.5.0
Improved operating feel. Improved mimic code. Fixed some bugs

19/04/24
--Michael
ver.0.5.1
Added high quality video opening, fixed bugs, added new journal entries, added temp art for maths teacher and lift.