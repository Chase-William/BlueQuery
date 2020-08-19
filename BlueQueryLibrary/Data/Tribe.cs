using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
    public struct TempGuild
    {         
        public TempGuild(ulong _id, Mode _mode)
        {
            Id = _id;
            Mode = _mode;
        }

        public ulong Id;
        public Mode Mode;
    }    
        
    public class Tribe
    {
        const string BASE_DIR = "tribes/";  // Base directory all tribes reside in

        private int id;

        /// <summary>
        ///     Our true primary key
        /// </summary>
        public int Id
        {
            get => id;
            set
            {
                id = value;

                // Create the directory if it hasn't been created yet
                if (!Directory.Exists(BASE_DIR + FolderName))
                {
                    Directory.CreateDirectory(BASE_DIR + FolderName);
                }
            }
        }

        /// <summary>
        ///     Name of the tribe which is unique
        /// </summary>
        public string NameId { get; set; }

        /// <summary>
        ///     Contains discord guilds which have been permitted to access the data located in FolderName variable
        /// </summary>
        public List<GuildInfo> PermittedGuilds { get; set; } = new List<GuildInfo>();

        /// <summary>
        ///     The tribe's folder name id<br/>
        ///     This is the same as the id of the tribe
        /// </summary>
        public string FolderName
        {
            get => Id.ToString();
        }

        /// <summary>
        ///     Contains information about all the blueprints this tribe contains
        /// </summary>
        public List<BlueprintInfo> Blueprints { get; set; } = new List<BlueprintInfo>();

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
                        PermittedGuilds.Add(new GuildInfo(guild.Id));
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

        public async Task<Tuple<bool, string>> CreateBlueprint(string bpNameOnly, string bpNameWithEx, string imgUrl)
        {
            try
            {
                HttpClient client = new HttpClient();
                Stream imgStream = await (await client.GetAsync(imgUrl)).Content.ReadAsStreamAsync();

                Image img = Image.FromStream(imgStream);
                img.Save($"{BASE_DIR}\\{FolderName}\\{bpNameWithEx}");
                // adding the blueprint to this tribe
                Blueprints.Add(new BlueprintInfo(bpNameWithEx));
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "An issue occured while attempting to download the image from the provided url.");
            }

            Blueprints.Add(new BlueprintInfo(bpNameOnly));

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        ///     Checks to see
        /// </summary>
        /// <param name="blueprintName"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool DoesBlueprintExist(string blueprintName, out BlueprintInfo blueprint, out string errMsg)
        {
            blueprint = null;
            errMsg = string.Empty;

            bool state = Blueprints.Exists(p => p.NameId.Equals(blueprintName));

            if (!state)
                errMsg = $"Invalid blueprint name given. The blueprint name {blueprintName} doesn't exist.";
            else
                blueprint = Blueprints.Single(p => p.NameId.Equals(blueprintName));

            return state;
        }
    }

    /// <summary>
    ///     Contains information about a guild<br/>
    ///     @prop - Id, Primary Key for a guild<br/>
    ///     @prop - DateAdded, Date this guild was originally added to the tribe
    /// </summary>
    public struct GuildInfo
    {
        /// <summary>
        ///     Primary Key for the guild
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        ///     Date originally added
        /// </summary>
        public DateTime DateAdded { get; set; }

        public GuildInfo(ulong _id)
        {
            Id = _id;
            DateAdded = DateTime.Now;
        }
    }

    /// <summary>
    ///     Contains information about a blueprint<br/>
    ///     @prop - NameId, Primary Key for the blueprint, also is the name of the image stored on disk<br/>
    ///     @prop - DateAdded, Date this blueprint was originally added to the tribe
    /// </summary>
    public class BlueprintInfo
    {
        /// <summary>
        ///     Creates a blueprint using the given name<br/>
        ///     Also notes when this blueprint was officially created
        /// </summary>
        /// <param name="_nameId"></param>
        public BlueprintInfo(string _nameId)
        {
            NameId = _nameId;
            DateAdded = DateTime.Now;
        }

        /// <summary>
        ///     Primary Key for the blueprint, also is the name of the image stored on disk
        /// </summary>
        public string NameId { get; set; }

        /// <summary>
        ///     Date this blueprint was originally added to the tribe
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        ///     Contains the filename of the image that is associated with this blueprint
        /// </summary>
        public string ImgFileName => NameId;
    }
}
