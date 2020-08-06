using BlueQuery.ResponseTypes;
using BlueQuery.Util;
using BlueQueryLibrary.Data;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BlueQuery.Commands
{
    struct ParamInfo
    {
        public string ParamType;                // String identifer of what the param purpose is
        public int ParamPropertyStartIndex;     // Start index of the property ex. -rename
        public int ParamValueStartIndex;        // Start index of the param value 
        public string ParamValue;               // Value of the parameter
    }

    public partial class TribeCommands
    {
        const string NAME_PARAM         = " -name ";    // Required in order to identify the tribe                  -- single use       -- if parameter '-create' is given then -name shouldn't be given as well
        const string RENAME_PARAM       = " -rename ";  // Optional, used to rename tribe                           -- single use
        const string CREATE_PARAM       = " -create ";  // Optional, used to create a tribe                         -- single use
        const string ADD_GUILD_PARAM    = " -add ";     // Optional, used to add a guild to permitted list          -- repeatable use
        const string REMOVE_GUILD_PARAM = " -remove ";  // Optional, used to remove a guild from the permitted list -- repeatable use
        const string GET_ALL_TRIBES     = " -all";      // Optional, returns all tribes the guild has access to     -- single use

        const byte MAX_TRIBE_NAME_LENGTH = 100;     // Max length of a tribe name in characters
        const byte GUILID_ID_LENGTH = 18;           // Length of a guild's ulong Id

        /// <summary>
        ///     Contains all the single use parameters available within the -tribe command
        /// </summary>
        static readonly string[] SINGLE_USE_PARAMS = new string[] { NAME_PARAM, RENAME_PARAM, CREATE_PARAM, GET_ALL_TRIBES };

        static readonly string[] REPEATABLE_USE_PARAMS = new string[] { ADD_GUILD_PARAM, REMOVE_GUILD_PARAM };

        public static async Task Tribe(CommandContext _ctx)
        {
            string str = _ctx.RawArgumentString;

            ParamInfo[] @params = GetAllParameters(_ctx, str);

            // If no arguments were given return
            if (@params == null)
            {
                await _ctx.RespondAsync("No parameters given. The tribe command isn't useful by itself. Try passing some parameters to make it do something.");
                return;
            }

            List<TempGuild> guilds = new List<TempGuild>();

            // Checking for multiple uses of a single use parameter
            {                
                for (int i = 0; i < SINGLE_USE_PARAMS.Length; i++)
                {
                    int paramCount = @params.Where(p => p.ParamType.Equals(SINGLE_USE_PARAMS[i])).Count();

                    if (paramCount > 1)
                    {
                        await _ctx.RespondAsync($"Single use parameter used too many times. The parameter {SINGLE_USE_PARAMS[i]} was provided more than once.");
                        return;
                    }
                }
            }            


            /* Validating / Processing -add parameters */
            
            // Processing guildIds here to prevent code duplication later on
            if (@params.Any(p => p.ParamType.Equals(ADD_GUILD_PARAM) || p.ParamType.Equals(REMOVE_GUILD_PARAM)))
            {
                // If an error ocurred during processing, propogate error to user & return from encapsulating function
                if (!ProcessGuildIds(out var _guildIds, out string procErrorMsg))
                {
                    await _ctx.RespondAsync(procErrorMsg);
                    return;
                }
                guilds = _guildIds;
            }
            

            /* ----- Create Tribe ----- */

            // Checking to see if we are creating a tribe
            if (@params.Any(p => p.ParamType.Equals(CREATE_PARAM)))
            {
                // Only -add and -create parameters are allowed when creating a tribe
                if (@params.Any(p => p.ParamType != CREATE_PARAM && p.ParamType != ADD_GUILD_PARAM))
                {
                    await _ctx.RespondAsync("Invalid parameters given. When creating a tribe you can only use the -create and -add parameters.");
                    return;
                }
                else
                {
                    // If an error ocurred during processing, propogate error to user & return from encapsulating function
                    //if (!ProcessGuildIds(out var guildIds, out string procErrorMsg))
                    //{
                    //    await _ctx.RespondAsync(procErrorMsg);
                    //    return;
                    //}

                    string tribeName = (@params.Single(p => p.ParamType.Equals(CREATE_PARAM)).ParamValue);
                    if (!ValidateTribeName(ref tribeName, out string errorMsg))
                    {
                        await _ctx.RespondAsync(errorMsg);
                        return;
                    }

                    // If a tribe with the given name already exist, error
                    if (TribeDatabaseContext.Provider.DoesTribeExist(tribeName, out _, out string tribeErrorMsg))
                    {
                        await _ctx.RespondAsync(tribeErrorMsg);
                        return;
                    }

                    guilds.Add(new TempGuild(_ctx.Guild.Id, Mode.Add));

                    Tribe newTribe = new Tribe
                    {
                        NameId = @params.Single(p => p.ParamType.Equals(CREATE_PARAM)).ParamValue,                        
                        Owner = _ctx.Member.Id
                    };

                    // Adds all guilds to the new tribe (includes duplicate checks inside function)
                    newTribe.ApplyGuilds(guilds.ToArray());

                    // Insert call
                    TribeDatabaseContext.Provider.InsertTribe(newTribe);
                   
                    await Messenger.SendMessage(_ctx , new TribeInfoResponse(_ctx.Client, newTribe));
                    return;
                }
            }
                
            // If the -all param is present return
            if (@params.Any(p => p.ParamType.Equals(GET_ALL_TRIBES)))
            {
                var tribes = TribeDatabaseContext.Provider.GetTribesFromGuild(_ctx.Guild.Id);

                await _ctx.RespondAsync(tribes.FirstOrDefault().NameId);
                return;
            }

            // If the -name param hasn't been provided, error
            if (!@params.Any(p => p.ParamType.Equals(NAME_PARAM)))
            {
                await _ctx.RespondAsync("Missing required parameter. You must provide the -name *<your tribe name here>* parameter.");
                return;
            }

            // Validating that the target tribe exist
            Tribe tribe;
            {
                // Checking to make sure the targe tribe exist
                if (!TribeDatabaseContext.Provider.DoesTribeExist(@params.Single(p => p.ParamType.Equals(NAME_PARAM)).ParamValue, out Tribe _tribe, out string tribeErrorMsg))
                {
                    await _ctx.RespondAsync(tribeErrorMsg);
                    return;
                }
                tribe = _tribe;
            }

            // Add guilds if given
            if (guilds.Count != 0) tribe.ApplyGuilds(guilds.ToArray());

            /* ----- Get Tribe Info ----- */
            
            if (@params.All(p => p.ParamType.Equals(NAME_PARAM) || p.ParamType.Equals(ADD_GUILD_PARAM) || p.ParamType.Equals(REMOVE_GUILD_PARAM)))
            {
                // If new guildIds were provided, perform an update
                if (guilds.Count != 0) TribeDatabaseContext.Provider.UpdateTribe(tribe);

                await Messenger.SendMessage(_ctx, new TribeInfoResponse(_ctx.Client, tribe));
                return;
            }


            /* ----- Renaming Tribe ----- */

            // If the -rename parameter is present we need to rename the tribe
            if (@params.Any(p => p.ParamType.Equals(RENAME_PARAM)))
            {
                // If the given guildIds don't process correctly, error
                //if (!ProcessGuildIds(out List<ulong> guildIds, out string procErrorMsg))
                //{
                //    await _ctx.RespondAsync(procErrorMsg);
                //    return;
                //}

                // variable is used to store our processed tribe name
                string newTribeName = (@params.Single(p => p.ParamType.Equals(RENAME_PARAM)).ParamValue);
                if (!ValidateTribeName(ref newTribeName, out string errorMsg))
                {
                    await _ctx.RespondAsync(errorMsg);
                    return;
                }

                // Checking to make sure the given tribe name exist in the database.
                // IMPORTANT -- here we pass in the processed tribe name created above, not query the name from the collection
                if (TribeDatabaseContext.Provider.DoesTribeExist(newTribeName, out _, out string _tribeErrorMsg))
                {
                    await _ctx.RespondAsync(_tribeErrorMsg);
                    return;
                }                

                tribe.NameId = newTribeName;

                // Update call
                TribeDatabaseContext.Provider.UpdateTribe(tribe);
              
                await Messenger.SendMessage(_ctx, new TribeInfoResponse(_ctx.Client, tribe));
                return;
            }
           

            // Parses guild ids from string to unsigned long
            bool ProcessGuildIds(out List<TempGuild> ids, out string errorMsg)
            {
                var toBeAddedIds = @params.Where(p => p.ParamType.Equals(ADD_GUILD_PARAM)).Select(p => p.ParamValue).ToList();
                var toBeRemovedIds = @params.Where(p => p.ParamType.Equals(REMOVE_GUILD_PARAM)).Select(p => p.ParamValue).ToList();

                ids = new List<TempGuild>();

                // If the user is adding and removing the same id, error
                if (toBeAddedIds.Any(add => toBeRemovedIds.Any(remove => remove.Equals(add))))
                {
                    errorMsg = $"Attempt to add and remove the same guild id at once rejected. You cannot add and remove the same guild id in one call.";
                    return false;
                }                

                if (toBeAddedIds.Count != 0)
                    foreach (var strId in toBeAddedIds)
                    {
                        if (ulong.TryParse(strId, out ulong parsedId) && strId.Length == GUILID_ID_LENGTH)
                            ids.Add(new TempGuild(parsedId, Mode.Add));                       
                        else
                        {
                            errorMsg = $"Invalid guild id given. The id {strId} was invalid. A guild's id must be 18 characters in length and only contain numbers.";
                            return false;
                        }
                    }

                if (toBeRemovedIds.Count != 0)
                    foreach (var strId in toBeRemovedIds)
                    {
                        if (ulong.TryParse(strId, out ulong parsedId) && strId.Length == GUILID_ID_LENGTH)
                            ids.Add(new TempGuild(parsedId, Mode.Remove));
                        else
                        {
                            errorMsg = $"Invalid guild id given. The id {strId} was invalid. A guild's id must be 18 characters in length and only contain numbers.";
                            return false;
                        }
                    }

                // No error, therefore empty message
                errorMsg = string.Empty;
                return true;
            }

            // Processes and Validates a given tribe name
            // false == invalid
            // true == valid
            bool ValidateTribeName(ref string tribeName, out string errorMsg)
            {
                tribeName = tribeName.Trim();

                // We trimmed it so no need to check for whitespace               
                if (string.IsNullOrEmpty(tribeName))
                {
                    errorMsg = $"Invalid tribe name given. The tribe name '{tribeName}' cannot be an empty string or only contain whitespaces.";
                    return false;
                }
                else if (tribeName.Length > 100)
                {
                    errorMsg = $"Invalid tribe name given. The tribe name '{tribeName}' exceeds the 100 character limit of tribe names. Choose a smaller name.";
                    return false;
                }
                else
                {
                    errorMsg = string.Empty;
                    return true;
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


        /// <summary>
        ///     Processes all parameters inside request string<br/>
        ///     This is done by using the IndexOf function to obtain the starting positions of all parameter types ex. -name<br/>
        ///     All the information gathered about the positions of the params is stored in a ParamInfo array.<br/>
        ///     Each parameter gets it's own ParamInfo instance.
        /// </summary>
        /// <param name="_ctx"> Entire message context </param>
        /// <param name="srcStr"> The source string </param>
        /// <returns> Array of information about each parameter inside the srouce string </returns>
        private static ParamInfo[] GetAllParameters(CommandContext _ctx, string srcStr)
        {
            // If the given string was empty return null
            if (string.IsNullOrWhiteSpace(srcStr)) return null;

            // Collection that holds all parameter's info
            var paramsInfo = new List<ParamInfo>();

            // Searching / Checking all single use params
            for (int i = 0; i < SINGLE_USE_PARAMS.Length; i++)
            {
                ProcessSingleUseParam(SINGLE_USE_PARAMS[i]);
            }

            for (int i = 0; i < REPEATABLE_USE_PARAMS.Length; i++)
            {
                ProcessRepeatableParam(REPEATABLE_USE_PARAMS[i]);
            }            

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

            return pOrdered;

            #region Nested Utility Functions
            // Gets a parameter that can only occur once inside a request
            void ProcessSingleUseParam(in string _param)
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
            void ProcessRepeatableParam(in string _param)
            {
                // Helps IndexOf not find the same _param repeatedly 
                int posOffset = 0;

                string srcStrCpy = srcStr;


                while (true)
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
            #endregion
        }
    }
}
