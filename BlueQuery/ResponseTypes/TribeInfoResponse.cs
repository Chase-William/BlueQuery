using BlueQueryLibrary.Data;
using DSharpPlus;
using DSharpPlus.Entities;

namespace BlueQuery.ResponseTypes
{
    public class TribeInfoResponse : BlueQueryResponse
    {
        public TribeInfoResponse(DiscordClient _client, Tribe _tribe) => FormatTribe(_client, _tribe);

        private async void FormatTribe(DiscordClient _client, Tribe _tribe)
        {
            int index = 0;

            Header = $"Tribe: {_tribe.NameId}";
            Content[index] = $"Permitted Guilds:\n";

            for (int i = 0; i < _tribe.PermittedGuilds.Count; i++)
            {
                string content = string.Empty;
                if (_client.Guilds.ContainsKey(_tribe.PermittedGuilds[i].Id))
                {
                    DiscordGuild results;
                    try
                    {
                        results = await _client.GetGuildAsync(_tribe.PermittedGuilds[i].Id);
                        content += $"   {results.Id}: {results.Name}\n";
                    }
                    catch
                    {
                        content += $"   {_tribe.PermittedGuilds[i].Id}: <? Query Error>\n";
                    }
                }
                else
                {
                    content += $"   {_tribe.PermittedGuilds[i].Id}: <?>\n";
                }

                content += $"      Added: {_tribe.PermittedGuilds[i].DateAdded.ToShortDateString()}\n";

                // nextContent += _client.Guilds.ContainsKey(_tribe.PermittedGuilds[i]) ? $"   {await _client.GetGuildAsync(_tribe.PermittedGuilds[i])}\n" : $"   {_tribe.PermittedGuilds[i]}; <?>\n";

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
