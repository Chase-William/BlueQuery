using BlueQueryLibrary.Data;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueQuery.Commands
{
    public partial class TribeCommands
    {
        private const string RENAME_PARAM = " -rename ";
        private const string NEWNAME_PARAM = " -newname ";
        private const string ADD_GUILD_PARAM = " -add ";

        public static async Task CreateTribe(CommandContext _ctx)
        {
            string str = _ctx.RawArgumentString;
            if (string.IsNullOrWhiteSpace(str))
            {
                await _ctx.RespondAsync("Invalid arguments. You must provide atleast the name of the tribe Ex. (?createtribe -name <tribe name here>)");
                return;
            }

            Tribe newtribe = new Tribe
            {
                PermittedGuilds = new List<ulong>()
            };
            
            // Checking to make sure the name field is being provided
            if (str.Contains(" -name "))
            {
                string[] parts = str.Split(new string[] { " -name " }, StringSplitOptions.RemoveEmptyEntries);

                // Formatting error
                if (parts.Length > 1)
                {
                    await _ctx.RespondAsync("Formatting error. See '!help createtribe' for help.");
                    return;
                }

                // Checking for guilds to be added
                if (parts[0].Contains(" -add "))
                {
                    // splitting for guilds
                    string[] toBePermittedGuilds = parts[0].Split(" -add ");

                    // Assigning the name to our larger scope variable array
                    parts[0] = toBePermittedGuilds[0];

                    // iterating through all the guilds
                    for (int i = 1; i < toBePermittedGuilds.Length; i++)
                    {
                        ulong id;
                        // parsing each guild into
                        if(ulong.TryParse(toBePermittedGuilds[i], out id))
                        {
                            newtribe.PermittedGuilds.Add(id);
                        }
                        else
                        {
                            await _ctx.RespondAsync("Invalid guild id given.");
                            return;
                        }
                    }

                }               

                string name = parts[0].Trim();

                // If the name is invalid, error
                if (string.IsNullOrWhiteSpace(name))
                {
                    await _ctx.RespondAsync("Invalid tribe name. The tribe name cannot be empty or whitespace");
                    return;
                }

                // Check to see if a tribe with the same name already exist
                if (TribeDatabaseContext.Provider.DoesTribeExist(name.ToLower(), out _))
                {
                    await _ctx.RespondAsync("Invalid tribe name. The tribe name given already exist, please choose a different tribe name.");
                    return;
                }

                newtribe.NameId = name;
            }
            else
            {
                await _ctx.RespondAsync("Invalid arguments. Missing required -name parameter ex. ?createtribe -name <tribe name here>");
                return;
            }

            newtribe.Owner = _ctx.Member.Id;
            newtribe.PermittedGuilds.Add(_ctx.Guild.Id); // adding this guild to the permitted guild list

            TribeDatabaseContext.Provider.InsertTribe(newtribe);

            await _ctx.RespondAsync($"Tribe {newtribe.NameId} has been created!");
        }

        public static async Task Tribe(CommandContext _ctx)
        {
            string str = _ctx.RawArgumentString;

            GetAllParameters(str);

            // Rename operation desired
            if (str.Contains(RENAME_PARAM) && str.Contains(NEWNAME_PARAM))
            {
                int iRename = str.IndexOf(" -rename ");
                int iNewName = str.IndexOf(" -newname ");

                string renameValue = str.Substring(iRename + RENAME_PARAM.Length, iNewName - (iRename + RENAME_PARAM.Length));
                string newNameValue = str.Substring(iNewName + NEWNAME_PARAM.Length);

                RenameTribe(_ctx, str);
                return;
            }
        }

        struct ParamInfo
        {
            public string ParamType { get; set; }               // String identifer of what the param purpose is
            public int ParamPropertyStartIndex { get; set; }        // Start index of the property ex. -rename
            public int ParamValueStartIndex { get; set; }       // Start index of the param value 
            public string ParamValue { get; set; }              // Value of the parameter
        }

        /// <summary>
        ///     First I need to identify all the parameters that exist in the argument string<br/>
        ///     Secondly I need to get the indices of all the parameter's values starting positions and associate it with the param keyword<br/>
        ///     Thirdly I need to know the length of which to substring the param value
        /// </summary>
        /// <returns></returns>
        private static void GetAllParameters(string srcStr)
        {
            // Collection that holds all parameter's info
            var paramsInfo = new List<ParamInfo>();

            // Searching for the RENAME
            GetParam(RENAME_PARAM);
            GetParam(NEWNAME_PARAM);
            GetRepeatedParams(ADD_GUILD_PARAM);

            var pOrdered = paramsInfo.OrderBy(x => x.ParamValueStartIndex).ToArray();

            // Getting the parameters values now
            // Using the starting index of the next ordered param as the stopping point
            for (int i = 0; i < pOrdered.Length; i++)
            {
                // If we are iterating on the last item we need to read until the end because their isn't another element in the array (index out of bounds incoming)
                if (i == (pOrdered.Length - 1))
                {
                    pOrdered[i].ParamValue = srcStr.Substring(pOrdered[i].ParamValueStartIndex);
                    break;
                }
                // Reading from the start index of the current until the start index of the next minus 1
                // This is assigned back into the array but to the ParamValue property
                var test = (pOrdered[(i + 1)].ParamValueStartIndex - pOrdered[(i + 1)].ParamPropertyStartIndex);
                pOrdered[i].ParamValue = srcStr.Substring(pOrdered[i].ParamValueStartIndex, pOrdered[(i + 1)].ParamPropertyStartIndex - pOrdered[i].ParamValueStartIndex);
            }

            /* After Parsing Code Here */

            Console.WriteLine();

            // Gets a parameter that can only occur once inside a request
            void GetParam(in string _param)
            {
                if (srcStr.Contains(_param))
                {
                    int propIndex = srcStr.IndexOf(_param);
                    int startIndex = propIndex + _param.Length;

                    paramsInfo.Add(new ParamInfo
                    {
                        ParamType = _param,
                        ParamPropertyStartIndex = propIndex,
                        ParamValueStartIndex = startIndex
                    });
                }
            }

            // Gets parameters that can be repeated throughout the request
            void GetRepeatedParams(in string _param)
            {
                // Helps IndexOf not find the same _param repeatedly 
                int posOffset = 0;

                string srcStrCpy = srcStr;


                while(true)
                {
                    if (srcStrCpy.Contains(_param))
                    {
                        int propIndex = srcStr.IndexOf(_param, posOffset);
                        int startIndex = propIndex + _param.Length;

                        // Setting a offset so that the next iteration of IndexOf won't return the index of the same _param
                        posOffset = startIndex;

                        paramsInfo.Add(new ParamInfo
                        {
                            ParamType = _param,
                            ParamPropertyStartIndex = propIndex,
                            ParamValueStartIndex = startIndex
                        });

                        // -- With the following comment code we can get the guildId because the guild id is a number with no spaces
                        // -- TO MYSELF -- DO NOT REMOVE THIS COMMENT
                        // String scoped to the start of the -add value provided
                        //var startString = srcStrCpy.Substring(startIndex);
                        //var guildId = new string(startString.TakeWhile(x => x != ' ').ToArray());

                        srcStrCpy = srcStrCpy.Remove(srcStrCpy.IndexOf(_param), _param.Length);
                    }
                    else
                    {
                        break;
                    }
                }                
            }
        }

        /// <summary>
        ///     Handles a rename request of a tribe
        /// </summary>
        /// <param name="_rawStr"> raw argument string </param>
        private static async void RenameTribe(CommandContext _ctx, string _rawStr)
        {
            string[] parameters = _rawStr.Split(new string[] { " -rename ", " -newname " }, StringSplitOptions.RemoveEmptyEntries);

            // Trimming all parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = parameters[i].Trim();
            }

            // If there is a formatting error
            if (parameters.Length != 2)
            {
                await _ctx.RespondAsync("Formatting error. Request should look like: ?tribe -rename <target tribe name> -newname <new tribe name>");
                return;
            }

            // Preventing the user from trying to rename the tribe to the same name
            if (parameters[0] == parameters[1])
            {
                await _ctx.RespondAsync("You cannot rename to the same name...");
                return;
            }

            // If the target tribe doesn't exist, error
            if (!TribeDatabaseContext.Provider.DoesTribeExist(parameters[0], out Tribe existingTribe))
            {
                await _ctx.RespondAsync("The target tribe to be renamed does not exist.");
                return;
            }

            // If the new tribe name is already in use, error
            if (TribeDatabaseContext.Provider.DoesTribeExist(parameters[1].ToLower(), out _))
            {
                await _ctx.RespondAsync("The given new tribe name is already in use by another tribe.");
                return;
            }

            // Assigning the new name
            existingTribe.NameId = parameters[1];
            // Calling our database provider to make the updates to our local storage
            TribeDatabaseContext.Provider.UpdateTribe(existingTribe);
        }








        private static async void AppendAddParameters(CommandContext _ctx, string _toBeParsed , Tribe tribe)
        {
            // Checking for guilds to be added
            if (_toBeParsed.Contains(" -add "))
            {
                // splitting for guilds
                string[] toBePermittedGuilds = _toBeParsed.Split(" -add ");

                // Assigning the name to our larger scope variable array
                _toBeParsed = toBePermittedGuilds[0];

                // iterating through all the guilds
                for (int i = 1; i < toBePermittedGuilds.Length; i++)
                {
                    ulong id;
                    // parsing each guild into
                    if (ulong.TryParse(toBePermittedGuilds[i], out id))
                    {
                        tribe.PermittedGuilds.Add(id);
                    }
                    else
                    {
                        await _ctx.RespondAsync("Invalid guild id given.");
                        return;
                    }
                }

            }
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
