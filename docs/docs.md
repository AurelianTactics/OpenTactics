focus areas
how to do other versions (ie classic AT etc). code so it's flexible to do versions in the future
WA has to be outlined, goals, roadmap, how it can be integrated and see which code can be used
abstraction of builders to allow better usage and future customizability

/*
to do
how to do docs better in general
if keeping this format
	auto convert directory into hierarchy
	auto match the files to the labels below
handle combattext, need something instead of the stand in

misc ideas
	things you like from isignia tactics
	way to have units drop into WA mode (either controlled or loaded from a player configs)
*/


Code directory
Assets/Scripts

Assets/Scripts/AbilityBuilder/*
	AbilityBuilderObject.cs
	AbilityEditScrollList.cs
	AbilityEditScrollListButton.cs
	CommandSet.cs
Assets/Scripts/CampaignBuilder/*
	CampaignCampaign.cs
	CampaignDialogue.cs
	CampaignEditController.cs
	CampaignLauncher.cs
	CampaignLevel.cs
	CampaignSpawn.cs
Assets/Scripts/CharacterBuilder/*
	AbilityScrollList.cs
	CharacterClassPopup.cs
	CharacterUIController.cs
	ItemScrollList.cs
	ItemScrollListButton.cs
Assets/Scripts/ClassBuilder/*
	ClassEditController.cs
	ClassEditObject.cs
Assets/Scripts/Combat/*
	AbilityData.cs
	AbilityManager.cs
	AbilityObject.cs
	CalcCode.cs
	CalculationAT.cs
	CalculationEvasion.cs
	CalculationHitDamage.cs
	CalculationMod.cs
	CalculationMono.cs
	CalculationProjectile.cs
	CalculationResolveAction.cs
	CalculationZodiac.cs
	CameraClick.cs
	CameraFacingBillboard.cs
	CombatLogClass.cs
	CombatLogSaveObject.cs
	CombatMultiplayerObject.cs
	CombatStats.cs
	CombatUITarget.cs
	FocusOnMe.cs
	ItemData.cs
	ItemManager.cs
	ItemObject.cs
	MapTileManager.cs
	NameAbility.cs
	NameAll.cs
	ParticleManager.cs
	PlayerManager.cs
	PlayerUnit.cs
	PlayerUnitLevelStats.cs
	PlayerUnitObject.cs
	Singleton.cs
	SoundManager.cs
	SpellManager.cs
	SpellName.cs
	SpellNameAI.cs
	SpellNameData.cs
	SpellReaction.cs
	SpellSlow.cs
	StatusManager.cs
	StatusObject.cs
	TurnObject.cs
	TurnsManager.cs
	UIAbilityScrollList.cs
	UIActiveTurnMenu.cs
	UICombatLogButton.cs
	UICombatLogScrollList.cs
	UICombatStats.cs
	UIConfirmButton.cs
	UIGenericButton.cs
	UIMenuMenu.cs
	UISampleButton.cs
	UISpellNameDetails.cs
	UITurnsListButton.cs
	UITurnsScrollList.cs
	UITurnsTop.cs
	UIUnitListButton.cs
	UIUnitListScrollList.cs
Assets/Scripts/Common/*
	Notification Center/*
		NotificationCenter.cs
		NotificationExtensions.cs
	State Machine/*
		State.cs
		StateMachine.cs
	UI/*
		Animation/*
			EasingControl.cs
			EasingEquation.cs
			RectTransformAnchorPositionTweener.cs
			RectTransformAnimationExtensions.cs
			TransformAnimationExtensions.cs
			TransformLocalEulerTweener.cs
			TransformLocalPositionTweener.cs
			TransformPositionTweener.cs
			Tweener.cs
			Vector3Tweener.cs
		LayoutAnchor.cs
		Panel.cs
	CustomGameTeamScrollList.cs
	DialogController.cs
	ItemConstants.cs
	ScrollListSimple.cs
	UICameraMenu.cs
	UIMainMenu.cs
Assets/Scripts/Controller/*
	CombatStates/*
		ActiveTurnState.cs
		BaseCombatAbilityMenuState.cs
		CombatAbilitySelectState.cs
		CombatCommandSelectionState.cs
		CombatConfirmAbilityTargetState.cs
		CombatCutSceneState.cs
		CombatEndFacingState.cs
		CombatEndState.cs
		CombatExploreState.cs
		CombatMoveSequenceState.cs
		CombatMoveTargetState.cs
		CombatPerformAbilityState.cs
		CombatState.cs
		CombatStateInit.cs
		CombatTargetAbilityState.cs
		GameLoopState.cs
		MimeState.cs
		MultiplayerWaitState.cs
		PostActiveTurnState.cs
		ReactionState.cs
		SlowActionState.cs
		StatusCheckState.cs
	Victor Conditions/*
		CombatVictoryCondition.cs
	WalkAroundStates/*
		WalkAroundInitState.cs
		WalkAroundMainState.cs
	BattleMessageController.cs
	CombatController.cs
	ConversationController.cs
	GameObjectPoolController.cs
	InputController.cs
Assets/Scripts/Enums/* [9.2] 12/27/22 All files documented
	Alliances.cs [9.2a]
	Directions.cs [9.2b]
	Drivers.cs [9.2c]
	EquipSlots.cs [9.2d]
	Facings.cs [9.2e]
	Locomotions.cs [9.2f]
	Phases.cs [9.2g]
	StatTypes.cs [9.2h]
	Targets.cs [9.2i]
	Teams.cs [9.2j]
	WalkAroundInput.cs [9.2k]
Assets/Scripts/EventArgs/*
	InfoEventArgs.cs
Assets/Scripts/Exceptions/*
	Modifiers/*
	BaseException.cs
	ValueChangeException.cs
Assets/Scripts/Extensions/*
	DirectionsExtensions.cs
	FacingsExtensions.cs
	GameObjectExtensions.cs
Assets/Scripts/ItemBuilder/*
	ItemBuilderController.cs
Assets/Scripts/MapCreator/*
	LevelLoad.cs
	MapCreator.cs
	SerializableVector3.cs
	Serializer.cs
Assets/Scripts/Misc/* 12/22/22 All files documented
	Demo.cs
Assets/Scripts/Model/*
	CombatTurn.cs
	ConversationData.cs
	Info.cs
	LevelData.cs
	Point.cs
	PoolData.cs
	SpeakerData.cs
Assets/Scripts/Multiplayer/*
	Deprecated. MP worked in a prior version
Assets/Scripts/RL/*
	DuelRLAgent.cs
	DuelRLArea.cs
	GridworldTacticsAgent.cs
	GridworldTacticsArea.cs
	RLBlackBoxActions.cs
	RLEnvConfig.cs
Assets/Scripts/StoryBuilder/* [1.2h]
	CutSceneController.cs
	StoryBuilderController.cs
	StoryCutScene.cs
	StoryItem.cs
	StoryModeController.cs
	StoryObject.cs
	StoryPartyController.cs
	StoryPoint.cs
	StorySave.cs
	StoryShopController.cs
	StoryShopScrollList.cs
Assets/Scripts/View Model Component/*
	Ability/*
		CombatAbilityArea.cs
		CombatAbilityRange.cs
	Actor/*
		Alliance.cs
		Driver.cs
	AI/*
		AbilityPicker/*
			CombatBaseAbilityPicker.cs
			CombatFixedAbilityPicker.cs
			CombatRandomAbilityPicker.cs
		AttackOption.cs
		CombatComputerPlayer.cs
		CombatPlanOfAttack.cs
	Board.cs
	CameraControls.cs
	CameraRig.cs
	ConversationPanel.cs
	FacingIndicator.cs
	Poolable.cs
	Tile.cs
	UIBackButton.cs
	UIUnitInfoPanel.cs
Assets/Scripts/WalkAround/*
	WalkAroundActionObject.cs [1.16d]
	WalkAroundManager.cs [1.16e]
	WalkAroundMapGenerator.cs [1.16f]


Assets/PlayerUnitObjectText/* [1.20]
	Animation/*
	Prefabs/*
	Scripts/*
		PUOTextAnchorController.cs
		PUOTextController.cs
		PUOTextOverlayCanvasController.cs
		PUOTextType.cs


[1.2] Builders
[1.2a] Builder preface
Ideally, the project is flexible enough that any turn based grid game can be represented in some way in this project.
Practically, the project can handle tactics games within a certain narrow framework. I would like to iterate on this to add more customizability and flexibility
Currently, the range of what can be customized is quite narrow. The abstraction of these features is non-existent.
At some point, a rethinking of the current system needs to be done and abstracted into a flexibily framework to focus on more ambitious goals.
The existing builders figure around a certain subset of the genre: turn based tactics games with a narrow rule set. The nonexistent GameRuleBuilder could expand upon this and over more types of games..
The current builders base the game around two versions. Basically each version is like its own game. All FFT classes, abilities, items etc are in one version. The AT version is pretty preliminary.
I would like it so that users can create their own versions with their own classes, items, characters, rule sets etc.
The data for the abilities, characters, items (and maybe some other parts) comes from spreadsheets that build scriptable objects. This is an easy way for someone with access to the unity editor to add game content.
Through the playable game itself, in Assets theres a Custom folder. Custom Campaigns, CommandSets, Items, Jobs, Levels (maps), Spells, Stories, and Units can be created and shared with others.
The building and saving used for the scriptable thing and the custom things were both considered from a list of different ways of doing it around 2018.
It is unclear if this is the best or preferable way of creating and customizing various things. Some research could be done into a better system.
The custom builders mostly work. They are not well bug tested. The UI is clunky, unintuitive, and ugly. The code behind it is opaque, not really abstraced.
Some code and prefabs and approaches are used in multiple builders. Nothing is very well documented.
Obviously, this can be improved.
[1.2b] Ability Builder
Create new abilities (spells) here. Set various options like what it does, who it hits etc. Can assign to a command set. The command set can be equipped by a class.
A class can be put on a unit or the command set can be used as a secondary ability set to allow units to cast the spells in the game.
[1.2c] Campaign Builder
Build campaigns here. Campaigns are series of levels played back to back where persistence is achieved between characters.
[1.2d] Character Builder
Build characters here. Characters can be given names, classes, items, etc and used in games.
[1.2e] Class Builder
Build classes here. Classes can be given abilities and different base stats.
[1.2f] ItemBuilder
Build items here. Items can be used by custom characters
[1.2g] MapBuilder
Maps/Levels are created here. Make maps to use in various game modes here

[1.2h] StoryBuilder [1.2h]
Assets/Scripts/StoryBuilder/* [1.2h]
stories have persistence of characters over multiple maps. The maps are accessible in a game world view. Campaigns can be parts of stories with a UI.

[1.2h1] CutSceneController.cs
Assets/Scripts/StoryBuilder/CutSceneController.cs 
/// Controls the cut scenes that can occur before combat, during combat, and after combat
/// Cut scenes generally feature characters' text bubbles showing various story or combat related text

STOPPED HERE
	Assets/Scripts/StoryBuilder/StoryBuilderController.cs
	Assets/Scripts/StoryBuilder/StoryCutScene.cs
	Assets/Scripts/StoryBuilder/StoryItem.cs
	Assets/Scripts/StoryBuilder/StoryModeController.cs
	Assets/Scripts/StoryBuilder/StoryObject.cs
	Assets/Scripts/StoryBuilder/StoryPartyController.cs
	Assets/Scripts/StoryBuilder/StoryPoint.cs
	Assets/Scripts/StoryBuilder/StorySave.cs
	Assets/Scripts/StoryBuilder/StoryShopController.cs
	Assets/Scripts/StoryBuilder/StoryShopScrollList.cs


[1.2i] GameRuleBuilder
TBD
Be able to add different versions, or abstract all versions under certain ways
thought experiment: what would have to change to make chess playable? to make it so units could be on more than one tile, etc.

[1.2j] VersionBuilder
Only FFT and AT so far. But would be nicet o have different versions with different rules
Need more flexibility in the system.
Need a better vision for this.
I don't believe there is a code file for this



[1.20] PlayerUnitObjectText
When action effects happen, shows text above the PlayerUnitObject (ie 50 points of damage)
Based on a different package
stand in for now until a longer term version can be done
to update the animation, need to go into the demo scene (not sure which one) and mess with the animator attached to the object
The animations show the different effect types
I believe this code connects with PlayerManager.cs for usage and calling of the text

[1.16d] WalkAroundActionObject.cs
Assets/Scripts/WalkAround/WalkAroundActionObject.cs
/// object for PlayerUnit actions in WalkAround mode
/// held in a queue in PlayerManager, which sorts through them for various turns
/// In WalkAround mode not set turn orders. PlayerManager has a queue, as actions are decided they are coded as CombatTurns, 
/// then changed into WalkAroundActionObjects and added to the queue

[1.16e] WalkAroundManager.cs DEPRECATED
Assets/Scripts/WalkAround/WalkAroundManager.cs

[1.16f] WalkAroundActionObject.cs
Assets/Scripts/WalkAround/WalkAroundMapGenerator.cs
/// Unclear where this is called from or if this is still used
/// Generates a map based on a string
/// In WA mode can move from map to map. this helps generate the map

[9.1] Misc
Assets/Scripts/Misc/Demo.cs
Misc file showing outline of how state behavior works
Not implementing anywhere but useful for understanding things at a high level

[9.2] Enums
Collection of enums. enum is a special "class" that represents a group of constants.

Assets/Scripts/Enums/*
	[9.2a] Alliances.cs
		PlayerUnit relationship status with another PlayerUnit. Ie allied, hostile, etc.
	[9.2b] Directions.cs
		Directions a PlayerUnit can face N,E,S,W
	[9.2c] Drivers.cs
		How a PlayerUnit has its active turn selected. Ie player input, AI, etc.
	[9.2d] EquipSlots.cs
		Slots equipment can be equipped on a PlayerUnit
	[9.2e] Facings.cs
		Way a PlayerUnit can face another PlayerUnit/other type of object. Front, Side, Back
	[9.2f] Locomotions.cs
		When a PlayerUnit moves, ways the PU can move across the map. Ie walk, fly, teleport.
	[9.2g] Phases.cs
		Phases GameLoop.cs can be in
	[9.2h] StatTypes.cs
		Not implemented
	[9.2i] Targets.cs
		When using an ability, the target types (self, ally, target etc)
	[9.2j] Teams.cs
		Not implemented
	[9.2k] WalkAroundInput.cs
		WA mode, different types of inputs that can be done for a PlayerUnit turn



Below is 12/21/22 chromebook version mixed with new files

[1.0] Scripts
[1.1] Main Menu
[1.1a] UIMainMenu.cs
[1.2] Builders
[1.2a] Builder preface
[1.2b] Ability Builder
[1.2c] Campaign Builder
[1.2d] Character Builder
[1.2e] Class Builder
[1.2f] ItemBuilder
[1.2g] MapBuilder
[1.2h] StoryBuilder
[1.2i] GameRuleBuilder
[1.2j] VersionBuilder
[1.6] Combat
[1.6a] CombatState.cs
[1.6b] InputController.cs
[1.6c] CombatUITarget.cs
[1.6d] CombatTurn.cs
[1.6e] Tile.cs
[1.6f] PlayerManager.cs
[1.6f1] PlayerManager.cs WA map generation
[1.6g] CombatComputerPlayer.cs (and dumb AI in general)
[1.6h] CombatCommandSelectionState.cs
[1.7] CustomGame
[1.8] Cut Scene
[1.11] MultiplayerMenu
[1.13] StoryMode
[1.14] StoryParty
[1.15] StoryShop
[1.16] WalkAround
[1.16a] WalkAroundMainState.cs
[1.16b] DEPRECATED WalkAroundPlayerState.cs, WalkAroundAbilitySelectState.cs, WalkAroundCommandSelectionState.cs, WalkAroundConfirmAbilityTargetState.cs, WalkAroundMoveTargetState.cs, WalkAroundTargetAbilityState.cs
[1.16c] WalkAroundInitState.cs
[1.17] Prefabs
[1.17a] Tile Prefabs
[1.18] RL
[1.18a] GridworldTacticsArea.cs
[1.18b] GridworldTacticsAgent.cs
[1.18c] DuelRLArea.cs
[1.18c] DuelRLAgent.cs
[1.19] Multiscript concepts
[1.19a] Ending Combat
[1.20] Combat Text

[9.0] Code Improvements
Unify the menu codes so not a bunch of one off code for each menu

[1.1] MainMenu
Top menu to navigate between scenes
Can launch game modes from here
Can launch builders from here (item builder, ability builder map builder etc)
Simple canvas with buttons with On click functionality
Scripts

[1.1a] UIMainMenu.cs
Scripts/Common/UIMainMenu.cs
Handles OnClick button functions
Launches game modes and builders

[1.6] Combat

[1.6a] CombatState.cs
Scripts/Controller/CombatStates/CombatState.cs
Combat and WalkAround scenes shift from state to state based on the game.
Inherited by most states in Combat and WalkAround scenes
Has much functionality that other states rely on easy access to
input listeners, menus for pop ups, and objects that persist across states

[1.6b] InputController.cs

handles fireEvent (mouse click) and moveEvent (WASD)
listeners for this are used in CombatState, which is inherited from most states

[1.6c] CombatUITarget.cs
set in scene in CombatState.cs script
controls UI elements for actor and target on what is highlighted on click/mouseover (ie lower right panel whose turn it is or lower middle panel for which unit is being targeted by a spell etc
used in Combat and WalkAround scenes
Called from many scripts
[1.6d] CombatTurn.cs
Contains information on a PlayerUnit's turn as it is inputted by user/AI
Ie action, move, move tile, unit, etc
Used in Combat and WA modes, reset each time a new unit does its new turn stuff
[1.6e] Tile.cs
Can change tile materials and textures with functions here
Called through board.cs
[1.6f] PlayerManager.cs
[1.6f1] PlayerManager.cs WA map generation
maps linked through sWalkAroundMapDictionary
each map generated based on seed and x,y coordinates based on distance from origin (original map at 0,0)
Actual creating/loading of maps based on the random seed, coordinates, and LevelData.cs instructions
[1.6g] CombatComputerPlayer.cs (and dumb AI in general)
Drivers check to see if human or ai controlled. Drivers are set at beginning of combat based on options
PlayerUnitObject.cs holds the driver and some info that this file uses to speed up turn selection
When unit has a turn and enters CombatCommandSelectionState.cs, if AI then begins process of coming up with CombatTurn.planOfAttack
the dumbAI heuristics in this file help fill in the planOfAttack (where to move, which unit to attack etc)
[1.6h] CombatCommandSelectionState.cs
Entered from GameLoopState.cs
Unit begins planning turn here. Different tracks for human and AI controlled based on drivers

[1.16] WalkAround
Experimental game mode that is not tested. At a high level this game mode lets you move around from combat map to combat map with
persistence between units and created maps. Stretch goal would be to allow multiple users in the same walkaround collection of maps.

[1.16a] WalkAroundMainState.cs
executes WA mode like units moving around. using abilities etc
if combat is triggered, change from here into combat state
Combat triggered from PlayerManager sending a CombatStart notification which this script listens to
Not a scene change, everything stays in WA scene
entered from WalkAroundInitState.cs which sets up the map
works tightly with PlayerManager which may not be ideal
[1.16b] DEPRECATED WalkAroundPlayerState.cs, WalkAroundAbilitySelectState.cs, WalkAroundCommandSelectionState.cs, WalkAroundConfirmAbilityTargetState.cs, WalkAroundMoveTargetState.cs, WalkAroundTargetAbilityState.cs
never actually implemented, just coded out
this way would take each player through each states when selecting commands (as if in combat)
implemented way does the action cmmand in WAMS
[1.16c] WalkAroundInitState.cs
Sets up board and units for walkaround mode
entered from CombatController (which is attached to scene object)
CombatController knows to go here or WalkAround based on something saved in playerprefs
exits to WalkAroundMainState after setting up the board
Each transition from board to board, enter this state and reset-up the board
Map Generation and units:
Goes through WAMapGenerator then LevelData
uses seed and x,y coordinates to generate the map and the units
using PlayerManager's sMapDictionary to save visits to map and currentMap
save and load functions in CalcCode to handle loading and saving of various maps and units
map data saved in map dict
unit data saved per map (for now basically just which map units have been killed or not), can be expanded




[1.17] Prefabs
[1.17a] Tile Prefabs
Tile_box prefab is the board tile
For combat/WA scenes is a board holder that holds the tiles
has various materials:
default for what it is
material in case it is an edge piece
can expand the Tile.cs script for more TileType attributes (currently just default and allow exit map)
Has textures
different textures for different tile highlights
can handle this with textures through the Tile.cs file for now
[1.18] RL
RL mode using UnityML
Generally following the UnityML examples (as of version 1) where there is an Area script for env logic and an Agent script for handing agent actions
Generally I also modify some parts of the AT code to take advantage of previously written code rather than try to do things from scratch in agent/ara
[1.18a] GridworldTacticsArea.cs
Implements Gridworld RL env with game. Agent gets reward for reaching the goal, then the goal resets.
Board is setup with special code in CombatInitState.cs
Waits for board to be set up then tells agent when its turn is reached. Produces obs for agent when asked (the game board), resets episode when done by calling to Board.cs
Relies on some expanded functionality in PlayerManager.cs and Board.cs for help producing obs
[1.18b] GridworldTacticsAgent.cs
Waits for GridworldTacticsAgent.cs to tell it's turn, then calls for obs from Area.ca, takes an action and relays it to Area.cs which does the game logic
heuristic mode is kind of wonky
[1.18c] DuelRLArea.cs
Minigame of AT that pits one unit vs. another in a small board
I didn't really generalize it though it could be
Relies on functions in CombatStateInit (setting up and resetting the board), PlayerManager (various convenience functions and getting observations)
Basically uses the main games code and game loop
Sets up an RL driver, at run time game loop reaches points where it needs input from here so calls a notification, this listens to notification then tells agent it is it's turn and starts taht
At end of episode, also has a notificton to do end of episode work (end episode, set reward etc)
When agent produces a turn, sends notification back to CombatCommandSelectionState with the CombatPlanOfAttack which is used for the agent
[1.18c] DuelRLAgent.cs
Handles agent logic for Duel RL mode
Waits for DuelRLArea.cs, gets obs, sends action ot Area
[1.19] Multiscript concepts
[1.19a] Ending Combat
Combat victory conditions are set in CombatStartInit.cs
Sets a victory type by attaching CombatVictoryConditions to the owner part of CombatController (all states in CombatState state machine inherit owner)
In GameLoopState, there's a regular check to see if a victor team has been set and thus if there is a victor and if so calls for combat end here by changing state to CombatCutSceneState then CombatEndState (some exceptions like for quitting or other game modes)
might be better to do with a notification
in DuelRL there is code to send it back to CombatStateInit for a restart, ideally what restart code in all modes
PlayerManager has code where if anything changes in playerunit state that may trigger a victory type, checks if there is a victor



