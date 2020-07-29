using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueQuery.Commands
{
    public partial class TribeCommands
    {
        public static async Task CreateTribe(CommandContext _ctx)
        {
            // If an invalid tribe name was given report the error and return
            if (string.IsNullOrWhiteSpace(_ctx.RawArgumentString))
            {
                await _ctx.RespondAsync("Invalid tribe name. The tribe cannot be empty or only whitespace.");
                return;
            }

            // Trim tribename to remove leading / trailing whitespace
            string tribeName = _ctx.RawArgumentString.Trim();
            
            // Check to see if a tribe with the same name already exist
        }





















































        public static async Task Keep(CommandContext _ctx)
        {
            // If no image of a blueprint was provided return an error
            if (_ctx.Message.Attachments == null || _ctx.Message.Attachments.Count == 0)
            {
                await _ctx.RespondAsync("An image of the blueprint must be attached when creating a blueprint.");
                return;
            }


        }
    }
}
