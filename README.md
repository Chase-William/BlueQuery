<p align="center">
    <img src="https://raw.githubusercontent.com/ChaseRoth/BlueQuery/master/Branding/bluequery512.png" alt="BlueQuery Icon"/>
</p>

# BlueQuery

BlueQuery is a discord bot that aims to improve players` <a href="https://store.steampowered.com/app/346110/ARK_Survival_Evolved/">Ark Survival Evolved</a> gaming experience. This is done by providing tools to make tedious tasks simpler and more organized.

**Table of Contents**

- <a href="#costCMD">Cost Command<a/>
	- <a href="#amountParam">Amount Parameter</a>
- <a href="#selectCMD">Select Command</a>
- <a href="#tribeCMD">Tribe Command</a>
	- <a href="#nameParam">Name Parameter</a>
	- <a href="#renameParam">Rename Parameter</a>
	- <a href="#addParam">Add Parameter</a>
	- <a href="#removeParam">Remove Parameter</a>
	- <a href="#createParam">Create Parameter</a>
	- <a href="#allParam">All Parameter</a>
	
<a name="costCMD"></a>
# Cost Command
The *cost* command is designed to allow users to get the cost required to make the specified item(s). This command does more than just get the cost for the target item itself. It also returns the cost for all sub resources that are needed to craft the target item.

	?cost <ItemName>

Running this command will return the cost required to make one instance of the provided item.

<a name="amountParam"></a>
## Amount Parameter

The *-amount* parameter lets users get the cost of a certain quantity of the specified item.

	?cost <ItemName> -amount 100

This will return the cost to make 100 of the specified item.

<br/>

<a name="selectCMD"></a>
# Select Command
The *select* command allows the user to choose an item from a list of items. This is used with the cost command because if the given item’s name matches multiple items then the bot needs to know which one to use.

	?cost Rocket
User Request ^

	Blueprint Search Results:

```csharp
(1) Rocket Launcher
(2) Rocket Propelled Grenade
(3) Rocket Turret
(4) Rocket Homing Missle
(5) Rocket Pod
```
Bot Response ^

	?select 3
The user enters the *select* command to choose the 3rd item that matched their original search.

<br/>

<a name="tribeCMD"></a>
# Tribe Command
The *tribe* command is the basis for how the user will interact with their tribe(s). This includes all **CRUD** operations and the various ways of how this is implemented with a tribe.


	?tribe

Running the *tribe* command by itself will result in nothing being done because parameters are required for this command to be useful.

<a name="nameParam"></a>
### Name Parameter
The *-name* parameter is required in most cases and is used to identify the target tribe the user desires to perform queries against.

Dependent Parameters:
- Add
- Remove
- Rename

<a name="renameParam"></a>
### Rename Parameter
The *-rename* parameter renames an existing tribe to the provided name.

	?tribe -name <TribeName> -rename <NewName>

<a name="addParam"></a>
### Add Parameter
The *-add* parameter is a repeatable param that is used to add a *id* of a discord guild to the target tribe.

	?tribe -name <TribeName> -add <GuildId>

This will append the provided discord guild *id* to the tribe`s permitted guilds collection . Therefore the provided guild will now be able to interact with the target tribe.

	?tribe -add <GuildId> -name <TribeName> -add <GuildId> -add <GuildId>

In this example you can see the user has provided 3 different *-add* parameters. On top of that, they are being placed before the *-name* param. This request will work because BlueQuery is very flexable when it comes to parameter arrangement.

<a name="removeParam"></a>
### Remove Parameter
The *-remove* parameter is a repeatable param that is used to remove existing guild ids that are in the target tribe`s permitted guilds collection.

	?tribe -remove <GuildId>

<a name="createParam"></a>
### Create Parameter

The *-create* parameter creates a tribe with a give name.

	?tribe -create <TribeName>

When creating a tribe, the sender`s guild *id* will be automatically be added to the tribe. You can also include the *-add* parameter to add extra guilds when creating the tribe.

	?tribe -create <TribeName> -add <GuildId>

<a name="allParam"></a>
### All Parameter

The *-all* parameter allows the users to get all the tribes that their guild has access to.

	?tribe -all

The *name* and *id* of the guild will be returned.
