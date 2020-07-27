using System.Threading.Tasks;
using BlueQuery.ResponseTypes;
using DSharpPlus.CommandsNext;

namespace BlueQuery.Util
{
    public static class Messenger
    {
        public const string RETRIEVING_BLUEPRINTS_ERROR_MSG = "Error retrieving blueprints.";

        /// <summary>
        ///     Forwards the chunks of of our messages to the RespondAsync Command.
        ///     @param - _ctx, the command context used by the DSharpPlus API
        ///     @param - _response, the generic instance of a response our bot has created that needs to be shown to the user 
        /// </summary>
        public static async Task SendMessage(CommandContext _ctx, BlueQueryResponse _response)
        {
            // We send the header as its own message.
            await _ctx.RespondAsync(_response.GetFormattedHeader());
            for (int i = 0; i < _response.Content.Count; i++)
            {
                // We send each content seperately.
                // Each Content is a maximum of 2k characters.
                await _ctx.RespondAsync(_response.GetFormattedContent(i));
            }
        }
    }
}
