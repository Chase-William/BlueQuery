using BlueQueryLibrary.Data;
using DSharpPlus;
using DSharpPlus.Entities;

namespace BlueQuery.ResponseTypes
{
    /// <summary>
    ///     Formats all the tribes that the sender guild has access to, to an array of strings that meet discord's guidelines.
    /// </summary>
    public class GuildInfoResponse : BlueQueryResponse
    {
        public GuildInfoResponse(DiscordClient _client, Tribe _tribe) => FormatTribe(_client, _tribe);

        private void FormatTribe(DiscordClient _client, Tribe _tribe)
        {
            int index = 0;

            Header = $"Tribe: {_tribe.NameId}";
            Content[index] = $"Permitted Guilds:\n";

            for (int i = 0; i < _tribe.PermittedGuilds.Count; i++)
            {
                string content = string.Empty;

                if ((Content[index].Length + content.Length) <= MESSAGE_LENGTH_LIMIT)
                {
                    // Padding right allows the text after to be formatted neatly vertically.
                    Content[index] += content;
                }
                else
                {
                    // Incrementing the content array and adding the content.
                    Content[++index] += content;
                }
            }
        }
    }
}
