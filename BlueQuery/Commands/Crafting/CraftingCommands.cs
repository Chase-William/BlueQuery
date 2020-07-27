using BlueQuery.Commands.Crafting.CommandStorageTypes;
using BlueQueryLibrary.Blueprints.DefaultBlueprints;
using DSharpPlus.CommandsNext;
// Provides us the DSharpPlus formatter
using DSharpPlus;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlueQuery.Util;
using BlueQuery.ResponseTypes;
using System;

namespace BlueQuery.Commands.Crafting
{
    public partial class CraftingCommands
    {       
        /// <summary>
        ///     Determines the resources needed to craft the specific blueprint and its quantity.<br/><br/>
        ///     
        ///     Functionalities:<br/>
        ///         - Parameter one is the blueprint, parameter two is the quantity.<br/>
        ///         - If not parameter two is present, automatically assume the user wants the resources needed for one instance.
        /// 
        /// </summary>
        public static async Task Craft(CommandContext _ctx)
        {
            // Default value of amount
            int amount = 1;

            string args = _ctx.RawArgumentString;

            // If the command aguments are empty inform the user
            if (string.IsNullOrWhiteSpace(args))
            {
                await _ctx.RespondAsync("No blueprint type was provided.");
                return;
            }     
            // If the arugments arn't empty trim the front and rear because this protects our logic.
            // ?c -a 300 would pass if we didn't trim the space before the "-a"
            else
            {
                args = args.Trim();
            }

            // splits the arguments into their individual segments
            // indices:`
            // 0 == item identifier
            // 1 == quantity to be crafted
            string[] parameters = args.Split(new string[] { "-amount", "-amt", "-a" }, System.StringSplitOptions.RemoveEmptyEntries);

            // We want to remove only the first and trailing white spaces on the blueprint type, because our dictionary contains the inner spaces.
            parameters[0] = parameters[0].Trim();            

            // If the parameter.length doesn't equal 2 or 1 then we cannot accept this request.
            if (parameters.Length < 1 || parameters.Length > 2)
            {
                await _ctx.RespondAsync("Invalid number of arguments present.");
                return;                
            }
            // We only want to parse the parameters[1] index if it was provided, other we use the default value of 1.
            else if (parameters.Length == 2)
            {
                // We want to remove spaces from the number for parsing.
                parameters[1] = parameters[1].Trim();
                // If the user enters a non-numeric string report an error and return.
                if (!int.TryParse(parameters[1], out amount) || amount < 0)
                {
                    await _ctx.RespondAsync("Invalid amount.");
                    return;
                }
            }
            string[] keys;
            // All switches contain abbreviations
            switch (parameters[0])
            {
                // Wildcard will fetch all blueprints.
                case "*":
                    try
                    {
                        // Getting all blueprint keys
                        keys = BlueQueryLibrary.Data.Blueprints.DefaultBlueprints.Keys.ToArray();
                        await Messenger.SendMessage(_ctx, new SelectionListingResponce(keys, amount));
                    }
                    catch
                    {
                        // Logging error
                        Program.Client.DebugLogger.LogMessage(LogLevel.Error, "BlueQueryBot", $"Error retrieving blueprints, requested blueprint: '{parameters[0].ToLower()}'", DateTime.Now);
                        await _ctx.RespondAsync(Messenger.RETRIEVING_BLUEPRINTS_ERROR_MSG);
                    }
                    break;
                // Advanced Rifle Bullets
                case "arb":
                    // do something
                    break;
                // No abbreviations detected, therefore use contains search.
                default:
                    try
                    {
                        keys = BlueQueryLibrary.Data.Blueprints.DefaultBlueprints.Keys.Where(x => x.ToLower().Contains(parameters[0].ToLower())).ToArray();
                    }
                    catch
                    {
                        Program.Client.DebugLogger.LogMessage(LogLevel.Error, "BlueQueryBot", $"Error retrieving blueprints, requested blueprint: '{parameters[0].ToLower()}' request.", DateTime.Now);
                        await _ctx.RespondAsync(Messenger.RETRIEVING_BLUEPRINTS_ERROR_MSG);
                        return;
                    }

                    switch (keys.Length)
                    {
                        // No blueprints were found.
                        case 0:
                            await _ctx.RespondAsync("No blueprint matched your search.");
                            break;
                        // Only one blueprint was found so we can calculate immediately.
                        case 1:
                            await Messenger.SendMessage(_ctx, new ResourceCostResponse(keys.First(), amount));
                            break;
                        // Multiple blueprints were found.
                        default:
                            await Messenger.SendMessage(_ctx, new SelectionListingResponce(keys, amount));
                            break;
                    }
                    break;
            }
        }

        public static async Task Select(CommandContext _ctx)
        {
            // arguments of command
            string rawNum = _ctx.RawArgumentString.Trim();

            // If their are no options available, inform the user and return.
            if (SavedCraftInstructions.Content == null)
            {
                await _ctx.RespondAsync("Your last session has expired.");
                return;
            }
            // If the user enters a non-numeric string report an error and return.
            if (!int.TryParse(rawNum, out int selectedIndex))
            {
                await _ctx.RespondAsync("Your selection was not a number.");
                return;
            }                                                       
            // If the selected index is out of bounds of our options, inform the user.
            if (selectedIndex > SavedCraftInstructions.Content.Keys.Length || selectedIndex <= 0)
            {
                await _ctx.RespondAsync("Your selection was out of bounds of the options.");
                return;
            }

            await Messenger.SendMessage(_ctx, new ResourceCostResponse(selectedIndex, SavedCraftInstructions.Content.Amount));

            // Reseting the keyOptions to null so this cannot be called again.
            SavedCraftInstructions.Content = null;
        }     
    }    
}
