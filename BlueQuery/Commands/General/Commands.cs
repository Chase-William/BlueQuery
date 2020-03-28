using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace BlueQuery.Commands.General
{
    public class Commands
    {

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
    }
}
