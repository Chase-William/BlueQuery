using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using BlueQuery.Util;
using System.Linq;

namespace BlueQuery.Commands
{
    public class BlueprintCommands
    {
        const string NAME_PARAM = " -name ";
        const string CREATE_PARAM = " -create ";
        const string TRIBE_PARAM = " -tribe ";

        public readonly string[] SINGLE_USE_PARAMS = { NAME_PARAM, TRIBE_PARAM, CREATE_PARAM };

        [Command("Blueprint")]
        [Description("Base command for performing CRUD operations on blueprints.")]
        [Aliases("blueprint", "bp")]
        public async Task OnSave(CommandContext _ctx)
        {
            if(!StrParseUtil.ParseRequestStr(_ctx.RawArgumentString, SINGLE_USE_PARAMS, null, out ParamInfo[] @params, out string errMsg))
            {
                await _ctx.RespondAsync(errMsg);
                return;
            }

            /* ----- Command Specific Logic Below ----- */

            // If no arguments were given return
            if (@params == null)
            {
                await _ctx.RespondAsync("No parameters given. The blueprint command isn't useful by itself. Try passing some parameters to make it do something.");
                return;
            }

            if (@params.Any(p => p.ParamType.Equals(CREATE_PARAM)))
            {
                await _ctx.RespondAsync("CREATE-PARAM detected.");
                return;
            }

            await _ctx.RespondAsync("Save Command!");
        }
    }
}
