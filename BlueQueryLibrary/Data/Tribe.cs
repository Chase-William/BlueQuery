using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace BlueQueryLibrary.Data
{
    public enum Mode
    {
        Remove = 0,
        Add
    }

    public struct TempGuild
    {         
        public TempGuild(ulong _id, Mode _mode)
        {
            Id = _id;
            Mode = _mode;
        }

        public ulong Id { get; set; }
        public Mode Mode { get; set; }
    }    
    
    public class Tribe
    {
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

        /// <summary>
        ///     Base folder that contains all information related to this tribe
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        ///     The owner of the tribe - the creator
        /// </summary>
        public ulong Owner { get; set; }

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
                        PermittedGuilds.Add(new GuildInfo
                        {
                            Id = guild.Id,
                            DateAdded = DateTime.Now
                        });
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
    }

    public struct GuildInfo
    {
        public ulong Id { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
