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

            if (string.IsNullOrWhiteSpace(_ctx.RawArgumentString))
            {
                await _ctx.RespondAsync("No blueprint type was provided.");
                return;
            }
            // Removing all spaces from the request and making all chars lower case.
            string args = Regex.Replace(_ctx.RawArgumentString.ToLower(), @"\s+", "");           

            // splits the arguments into their individual segments
            // indices:
            // 0 == item identifier
            // 1 == quantity to be crafted
            string[] parameters = args.Split(new string[] { "-amount", "-amt", "-a" }, System.StringSplitOptions.RemoveEmptyEntries);

            // If the parameter.length doesn't equal 2 or 1 then we cannot accept this request.
            if (parameters.Length < 1 || parameters.Length > 2)
            {
                await _ctx.RespondAsync("Invalid number of arguments present.");
                return;                
            }
            // We only want to parse the parameters[1] index if it was provided, other we use the default value of 1.
            else if (parameters.Length == 2)
            {                
                // If the user enters a non-numeric string report an error and return.
                if (!int.TryParse(parameters[1], out amount))
                {
                    await _ctx.RespondAsync("Your choosen quantity was not a number.");
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
                        // Only one blueprint was found.
                        case 1:
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
                            await _ctx.RespondAsync(new DiscordResponse() 
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

            string blueprint = SavedCraftInstructions.Content.Keys[--selectedIndex];
            int amount = SavedCraftInstructions.Content.Amount;

            var response = new DiscordResponse
            {
                //Header =  "```cs\nMeow!! x 300```",
                //Header = $"```fix\n= {blueprint} x {amount}```",
                Header = $"{blueprint} x {amount}:",
                Content = GetCalculatedBlueprintCostFormatted(blueprint, amount)
            };
                          
            

            // Getting the cost based off the user provided parameters and forwarding them to the user.
            await _ctx.RespondAsync(response.GetResponce());           


            // Reseting the keyOptions to null so this cannot be called again.
            SavedCraftInstructions.Content = null;
        }

        /// <summary>
        ///     Calculates the cost a for a amount of a blueprint type.
        /// </summary>
        public static string GetCalculatedBlueprintCostFormatted(string _blueprintKey, int _amount)
        {
            string content = "#Resources:\n";

            // Getting the calculated cost for the blueprint
            CalculatedResourceCost[] costs = BlueQueryLibrary.Data.Blueprints.DefaultBlueprints[_blueprintKey].GetResourceCost(_amount).ToArray();

            // Adding each resource cost to the responce body.
            for (int i = 0; i < costs.Length; i++)
            {
                content += $"\t{costs[i].Type} x {costs[i].Amount}\n";
            }

            return content;
        }

        public struct DiscordResponse
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
