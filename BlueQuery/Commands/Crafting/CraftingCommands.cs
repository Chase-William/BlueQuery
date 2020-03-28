using BlueQuery.Commands.Crafting.CommandStorageTypes;
using BlueQueryLibrary.ArkBlueprints.DefaultBlueprints;
using DSharpPlus.CommandsNext;
// Provides us the DSharpPlus formatter
using DSharpPlus;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            // indices:
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

            // All switches contain abbreviations
            switch (parameters[0])
            {
                // Advanced Rifle Bullets
                case "arb":
                    // do something
                    break;
                // No abbreviations detected, therefore use contains search.
                default:
                    string[] keys = BlueQueryLibrary.Data.Blueprints.DefaultBlueprints.Keys.Where(x => x.ToLower().Contains(parameters[0])).ToArray();

                    switch (keys.Length)
                    {
                        // No blueprints were found.
                        case 0:
                            await _ctx.RespondAsync("No blueprint matched your search.");
                            break;
                        // Only one blueprint was found so we can calculate immediately.
                        case 1:
                            await _ctx.RespondAsync(GetCalculatedBlueprintCostFormatted(keys[0], amount).GetResponce());
                            break;
                        // Multiple blueprints were found.
                        default:
                            // Contains all the found blueprints.
                            string blueprints = string.Empty;
                            // Generate a string of the blueprints found.
                            for (int i = 0; i < keys.Length; i++)
                            {
                                blueprints += $"({i + 1}) {keys[i]}\n";
                            }
                            // Saving the options
                            SavedCraftInstructions.Content = new SavedCraftInstructions.SCIContent(keys, amount);
                            // Responding with the blueprint options
                            await _ctx.RespondAsync(new CraftingResponce() 
                            { 
                               Header = "Blueprint Search Results:",
                               Content = blueprints
                            }.GetResponce());
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

            // Getting the cost based off the user provided parameters and forwarding them to the user.
            await _ctx.RespondAsync(GetCalculatedBlueprintCostFormatted(SavedCraftInstructions.Content.Keys[--selectedIndex],
                                                                        SavedCraftInstructions.Content.Amount)
                                                                        .GetResponce());           

            // Reseting the keyOptions to null so this cannot be called again.
            SavedCraftInstructions.Content = null;
        }

        /// <summary>
        ///     Calculates the cost a for a amount of a blueprint type.
        /// </summary>
        public static CraftingResponce GetCalculatedBlueprintCostFormatted(string _blueprintKey, int _amount)
        {          
            string content = "#Resources:\n\n";

            // Getting the calculated cost for the blueprint
            CalculatedResourceCost[] costs = BlueQueryLibrary.Data.Blueprints.DefaultBlueprints[_blueprintKey].GetResourceCost(_amount).ToArray();

            // https://www.youtube.com/watch?v=5cg9jv83SMo
            // Using this to find the largest string inside our collection so we will know how to format.
            int offset = costs.Aggregate(string.Empty, (longest, bp) => bp.Type.Length > longest.Length ? bp.Type : longest).Length;          

            // Adding each resource cost to the responce body.
            for (int i = 0; i < costs.Length; i++)
            {
                // Padding right allows the text after to be formatted neatly vertically.
                content += $"\t{costs[i].Type.PadRight(offset)} x {costs[i].Amount}\n";
            }

            return new CraftingResponce
            {
                Header = $"{_blueprintKey} x {_amount}:",
                Content = content
            };
        }

        public struct CraftingResponce
        {
            public string Header { get; set; }
            public string Content { get; set; }

            public string GetResponce()
            {
                return Formatter.BlockCode(Header, "fix") + Formatter.BlockCode(Content, "py");
            }
        }
    }    
}
