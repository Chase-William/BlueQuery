using BlueQueryLibrary.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json.Converters;
using System.Net.Http.Headers;

namespace BlueQuery.ResponseTypes
{
    /// <summary>
    ///     Formats a tribe and its data into a array of strings that meet discord's guidelines.
    /// </summary>
    public class TribeInfoResponse : BlueQueryResponse
    {
        public TribeInfoResponse(DiscordClient _client, Tribe _tribe) => FormatTribe(_client, _tribe);

        private async void FormatTribe(DiscordClient _client, Tribe _tribe)
        {
            int index = 0;
            string content;

            Header = $"Tribes: {_tribe.NameId}";
            Content[index] = $"#Property / Value:\n\n#Guilds:\n";

            for (int i = 0; i < _tribe.PermittedGuilds.Count; i++)
            {
                if (_client.Guilds.ContainsKey(_tribe.PermittedGuilds[i].Id))
                {
                    DiscordGuild results;
                    // If something goes wrong with our request then catch
                    try
                    {
                        results = await _client.GetGuildAsync(_tribe.PermittedGuilds[i].Id);
                        content = $"\t{results.Id}: {results.Name}\n";
                    }
                    catch
                    {
                        content = $"\t{_tribe.PermittedGuilds[i].Id}: <? Query Error>\n";
                    }
                }
                else
                {
                    content = $"\t{_tribe.PermittedGuilds[i].Id}: <?>\n";
                }

                content += $"\t\tAdded: {_tribe.PermittedGuilds[i].DateAdded.ToShortDateString()}\n";

                AppendContent(ref index, content);              
            }

            AppendContent(ref index, "#Blueprints:\n");
            for (int i = 0; i < _tribe.Blueprints.Count; i++)
            {                
                AppendContent(ref index, $"\tBlueprint: {_tribe.Blueprints[i].NameId}\n");
            }
        }        
    }

    /// <summary>
    ///     Used to format all tribes a guild has access to.
    /// </summary>
    public class AllTribesResponse : BlueQueryResponse
    {
        public AllTribesResponse(DiscordClient _client, Tribe[] _tribes) => FormatTribes(_client, _tribes);

        private void FormatTribes(DiscordClient _client, Tribe[] _tribes)
        {
            int index = 0;

            Header = $"Tribe(s) Found: {_tribes.Length}";
            Content[index] = "#Results:\n";
            string content;

            for (int i = 0; i < _tribes.Length; i++)
            {
                content = $"\t{_tribes[i].NameId}\n";
                AppendContent(ref index, content);
            }
        }
    }


    public class BlueprintInfoResponse : BlueQueryResponse
    {
        public BlueprintInfoResponse(DiscordClient _client, BlueprintInfo _blueprint) => FormatBlueprint(_client, _blueprint);
        
        private void FormatBlueprint(DiscordClient _client, BlueprintInfo _blueprint)
        {
            int index = 0;
            string content = string.Empty;

            Header = $"Blueprint: {_blueprint.NameId}";
            content = $"#Property / Value:\n\tImage: {(string.IsNullOrEmpty(_blueprint.ImgFileName) ? "No Image" : _blueprint.ImgFileName)}";

            AppendContent(ref index, content);
        }
    }
}
