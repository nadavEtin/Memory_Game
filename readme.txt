features and design choices:
Dynamic board size - amount of cards is determined by Line & Column amount in LevelManager.
Sadly I didn't have time to scale the cards dynamically to fit into the playing area, so beyond a certain amount they'll be positioned outside the screen.
Save\Load - method used to save or load is easily configured through the save \ load managers.
Added binary serialization as an example for adding a new method of saving to the project with minimal changes to the code base.
Save is available only while the game is running. Load can be done at any time. 