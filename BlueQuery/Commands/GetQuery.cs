using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using BlueQueryLibrary;

namespace BlueQuery.Commands
{
    public class GetQuery
    {
        /// <summary>
        ///     Test the bot connection.
        /// </summary>
        [Command("ping")] // let's define this method as a command
        [Description("Pings the bot and responds with the latency.")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command        
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        /// <summary>
        ///     Gets all of a specific type from the database and returns the information back to the user.
        /// </summary>
        [Command("get")]
        [Description("Gets all of a specified type from the database and returns the information back to the user.")]
        public async Task WriteToDatabase(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            ctx.Message
        }
    }
}
