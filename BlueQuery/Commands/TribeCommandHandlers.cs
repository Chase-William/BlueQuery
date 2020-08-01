using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueQuery.Commands
{
    public partial class TribeCommands
    {
        //[Command("CreateTribe")]
        //[Description("Creates a tribe which owns blueprints and determines who has access to zed blueprints.")]
        //[Aliases("createtribe", "CT", "ct")]
        //public async Task OnCreateTribe(CommandContext ctx) => await CreateTribe(ctx);

        [Command("Tribe")]
        [Description("Base command for interacting with a tribe.")]
        [Aliases("tribe", "t", "T")]
        public async Task OnTribe(CommandContext ctx) => await Tribe(ctx);
        
        [Command("Keep")]
        [Description("Saves the blueprint to the discord server's database.")]
        [Aliases("keep", "k", "K")]
        public async Task OnKeep(CommandContext ctx) => await Keep(ctx);
    }
}
