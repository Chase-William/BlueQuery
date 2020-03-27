using BlueQuery.Commands.Crafting.CommandStorageTypes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace BlueQuery.Commands.Crafting
{
    public partial class CraftingCommands
    {

        /// <summary>
        /// 
        ///     Initiates the crafting process.<br/><br/>
        /// 
        ///     Example Uses:<br/>
        ///     ?c advanced rifle bullet -amount 2<br/>
        ///     ?c advanced rifle bullet -amt 2<br/>
        ///     ?c advanced rifle bullet -a 2<br/>
        /// </summary>
        [Command("craft")]
        [Description("Returns a formatted description of what is needed to craft the specified items.")]
        [Aliases("Craft", "c", "C")]
        public async Task OnCraft(CommandContext ctx)
        {
            await Craft(ctx);
        }

        /// <summary>
        /// 
        ///     Selects a blueprint from the SavedCraftingIntruction's blueprint list.<br/><br/>
        /// 
        ///     Notes:<br/>
        ///         Later this method will most likely be decoupled from crafting to serve a more broad purpose.<br/><br/>
        /// 
        ///     Example Uses:<br/>
        ///     ?s 2<br/>
        ///     ?select 3<br/>
        /// </summary>
        [Command("select")]
        [Description("Returns a formatted description of what is needed to craft the specified items.")]
        [Aliases("Select", "s", "S")]
        public async Task OnSelect(CommandContext ctx)
        {
            await Select(ctx);
        }
    }
}
