using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace BlueQueryLibrary.Data
{
    public enum Mode
    {
        Remove = 0,
        Add
    }

    /// <summary>
    ///     A temporary struct that holds data refering to how the guild should be treated by the receiving tribe
    /// </summary>
    public readonly struct TempGuild
    {         
        public TempGuild(ulong _id, Mode _mode)
        {
            Id = _id;
            Mode = _mode;
        }

        public readonly ulong Id;
        public readonly Mode Mode;
    }    
    
    public class Tribe
    {
        const string BASE_DIR = "tribes/";  // Base directory all tribes reside in

        /// <summary>
        ///     Our true primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name of the tribe which is unique
        /// </summary>
        public string NameId { get; set; }

        /// <summary>
        ///     Contains discord guilds which have been permitted to access the data located in FolderName variable
        /// </summary>
        public List<GuildInfo> PermittedGuilds { get; set; } = new List<GuildInfo>();

        private string folderName;
        /// <summary>
        ///     Base folder relative to the tribe.<br/>
        ///     Creates a directory for its tribe if one doesn't already exist.
        /// </summary>
        public string FolderName
        {
            get => folderName;
            set
            {
                folderName = value;

                // Create the directory if it hasn't been created yet
                if (!Directory.Exists(BASE_DIR + FolderName))
                {
                    Directory.CreateDirectory(BASE_DIR + FolderName);
                }
            }
        }

        /// <summary>
        ///     Contains information about all the blueprints this tribe contains
        /// </summary>
        public Dictionary<string, BlueprintInfo> Blueprints { get; set; } = new Dictionary<string, BlueprintInfo>();

        /// <summary>
        ///     The owner of the tribe (discord member or person, NOT GUILD) - the creator
        /// </summary>
        public ulong UserOwner { get; set; }

        /// <summary>
        ///     Applys the given guilds to the permitted guilds collection.<br/>
        ///     If a guildId is already present in the collection it will be skipped over.<br/>
        ///     If the id trying to be removed from the permitted guilds collection isn't inside that collection it will be skipped over.<br/>
        ///     @param - guild, guilds to be applied
        /// </summary>
        /// <param name="guilds"> Ids to be applied to the permitted guilds collection </param>
        public void ApplyGuilds(TempGuild[] guilds)
        {
            foreach (var guild in guilds)
            {                                
                if (guild.Mode.Equals(Mode.Add))
                {
                    // If the guild id already exist inside the permitted guild collection then skip
                    if (PermittedGuilds.All(p => p.Id != guild.Id))
                    {
                        PermittedGuilds.Add(new GuildInfo(guild.Id, DateTime.Now));
                    }                                      
                }
                // If the permitted guild isn't inside the collection then skip
                else if (PermittedGuilds.Any(p => p.Id.Equals(guild.Id)))
                {                    
                    GuildInfo toBeDeleted = PermittedGuilds.Single(p => p.Id.Equals(guild.Id));
                    PermittedGuilds.Remove(toBeDeleted);                                       
                }                              
            }            
        }

        public void CreateBlueprint()
        {

        }
    }

    /// <summary>
    ///     Contains information about a guild<br/>
    ///     @prop - Id, Primary Key for a guild<br/>
    ///     @prop - DateAdded, Date this guild was originally added to the tribe
    /// </summary>
    public readonly struct GuildInfo
    {
        /// <summary>
        ///     Primary Key for the guild
        /// </summary>
        public readonly ulong Id;
        /// <summary>
        ///     Date originally added
        /// </summary>
        public readonly DateTime DateAdded;

        public GuildInfo(ulong _id, DateTime _dateAdded)
        {
            Id = _id;
            DateAdded = _dateAdded;
        }
    }

    /// <summary>
    ///     Contains information about a blueprint<br/>
    ///     @prop - NameId, Primary Key for the blueprint, also is the name of the image stored on disk<br/>
    ///     @prop - DateAdded, Date this blueprint was originally added to the tribe
    /// </summary>
    public struct BlueprintInfo
    {
        /// <summary>
        ///     Primary Key for the blueprint, also is the name of the image stored on disk
        /// </summary>
        //public string NameId { get; set; }

        /// <summary>
        ///     Date this blueprint was originally added to the tribe
        /// </summary>
        public DateTime DateAdded { get; set; }
    }
}
