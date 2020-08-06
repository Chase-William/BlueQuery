using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace BlueQuery.Commands
{
    public class BlueprintCommands
    {
        [Command("Save")]
        [Description("Saves the given blueprint.")]
        [Aliases("save")]
        public async Task OnSave(CommandContext _ctx)
        {
            await _ctx.RespondAsync("Save Command!");
        }
    }
}
