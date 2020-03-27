using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

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


       
    }
}
