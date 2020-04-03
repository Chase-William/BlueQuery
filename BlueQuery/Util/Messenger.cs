using System.Threading.Tasks;
using BlueQuery.ResponseTypes;
using DSharpPlus.CommandsNext;

namespace BlueQuery.Util
{
    public static class Messenger
    {
        public const string RETRIEVING_BLUEPRINTS_ERROR_MSG = "Error retrieving blueprints.";

        /// <summary>
        ///     Takes a CraftingResponce and sends it properly to the receiver.
        /// </summary>
        public static async Task SendMessage(CommandContext _ctx, BlueQueryResponse _responce)
        {
            // We send the header as its own message.
            await _ctx.RespondAsync(_responce.GetFormattedHeader());
            for (int i = 0; i < _responce.Content.Count; i++)
            {
                // We send each content seperately.
                // Each Content is a maximum of 2k characters.
                await _ctx.RespondAsync(_responce.GetFormattedContent(i));
            }
        }
    }
}
