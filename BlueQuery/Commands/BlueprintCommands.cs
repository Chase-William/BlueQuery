using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using BlueQuery.Util;

namespace BlueQuery.Commands
{
    public class BlueprintCommands
    {
        public readonly string[] SINGLE_USE_PARAMS = { " -a ", " -t " };
        public readonly string[] REPEATABLE_USE_PARAMS = { " -b ", " -d " };


        [Command("Save")]
        [Description("Saves the given blueprint.")]
        [Aliases("save")]
        public async Task OnSave(CommandContext _ctx)
        {
            if(!ParseUtil.ParseRequestStr(_ctx.RawArgumentString, SINGLE_USE_PARAMS, REPEATABLE_USE_PARAMS, out ParamInfo[] @params, out string errMsg))
            {
                await _ctx.RespondAsync(errMsg);
                return;
            }

            await _ctx.RespondAsync("Save Command!");
        }
    }
}
