using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using System.Timers;
using BlueQueryLibrary.ArkBlueprints.DefaultBlueprints;
using BlueQuery.Commands.CommandStorageTypes;

namespace BlueQuery.Commands
{
    public class Commands
    {
        //public const int TIMEOUT_TIME = 10000;
        //public Timer KeyOptionsTimer { get; set; } = new Timer(TIMEOUT_TIME);
        ////private string[] keyOptions;
        ////public string[] KeyOptions
        ////{
        ////    get => keyOptions;
        ////    set
        ////    {
        ////        keyOptions = value;
        ////        KeyOptionsTimer.Start();
        ////    }
        ////}

        public SavedCraftInstructions SavedCraftInstructions { get; set; }

        public Commands()
        {
            SavedCraftInstructions = new SavedCraftInstructions();
        }

        [Command("ping")] // let's define this method as a command
        [Description("Pings the bot and responds with the latency.")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command        
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        public async Task Get(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            System.Console.WriteLine("msg content" + ctx.Message.Content);
            System.Console.WriteLine("raw args" + ctx.RawArgumentString);
        }


        /// <summary>
        ///     
        /// </summary>
        [Command("craft")]
        [Description("Returns a formatted description of what is needed to craft the specified items.")]
        [Aliases("Craft", "c", "C")]
        public async Task Craft(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            // arguments of command
            string args = ctx.RawArgumentString.Trim().ToLower();

            // splits the arguments into their individual segments
            // indices:
            // 0 == item identifier
            // 1 == quantity to be crafted
            string[] contents = args.Split(" ");

            // If the user enters a non-numeric string report an error and return.
            if (!int.TryParse(contents[1], out int quantity))
            {
                await ctx.RespondAsync("Your choosen quantity was not a number.");
                return;
            }

            // All switches contain abbreviations
            switch (contents[0])
            {
                // Advanced Rifle Bullets
                case "arb":
                    // do something
                    break;

                // No abbreviations detected, therefore use contains search.
                default:
                    string[] keys = Program.BQL.Blueprints.DefaultBlueprints.Keys.Where(x => x.ToLower().Contains(contents[0])).ToArray();                    

                    switch (keys.Length)
                    {
                        // No blueprints were found.
                        case 0:
                            await ctx.RespondAsync("No blueprint matched your search.");
                            break;
                        // Only one blueprint was found.
                        case 1:
                            break;
                        // Multiple blueprints were found.
                        default:
                            // Contains all the found blueprints.
                            string blueprints = "Blueprints Found:\n";
                            // Generate a string of the blueprints found.
                            for (int i = 0; i < keys.Length; i++)
                            {
                                blueprints += $"({i + 1}) {keys[i]}\n";
                            }
                            // Saving the options
                            SavedCraftInstructions = new SavedCraftInstructions(keys, quantity);
                            // Responding with the blueprint options
                            await ctx.RespondAsync(blueprints);
                            break;
                    }
                    break;
            }
        }        

        [Command("select")]
        [Description("Returns a formatted description of what is needed to craft the specified items.")]
        [Aliases("Select", "s", "S")]
        public async Task Select(CommandContext ctx)
        {
            // arguments of command
            string rawNum = ctx.RawArgumentString.Trim();                       

            // If their are no options available, inform the user and return.
            if (SavedCraftInstructions == null)
            {
                await ctx.RespondAsync("Your last session has expired.");
                return;
            }
            // If the user enters a non-numeric string report an error and return.
            if (!int.TryParse(rawNum, out int selectedIndex))
            {
                await ctx.RespondAsync("Your selection was not a number.");
                return;
            }

            await ctx.RespondAsync(SavedCraftInstructions.Keys[--selectedIndex]);
            Program.BQL.Craft.GetDefaultBlueprintCraftingCost(SavedCraftInstructions.Keys[--selectedIndex], SavedCraftInstructions.Quantity);

            // Reseting the keyOptions to null so this cannot be called again.
            SavedCraftInstructions = null;
        }
    }
}
