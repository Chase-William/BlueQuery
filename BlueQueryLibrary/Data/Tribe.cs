using LiteDB;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace BlueQueryLibrary.Data
{
    public class Tribe
    {
        /// <summary>
        ///     Our true primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name of the Tribe which shouldn't be repeated
        /// </summary>
        public string NameId { get; set; }

        /// <summary>
        ///     Contains discord guilds which have been permitted to access the data located in FolderName variable
        /// </summary>
        public List<ulong> PermittedGuilds { get; set; }

        /// <summary>
        ///     Base folder that contains all information related to this Tribe
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        ///     The owner of the tribe - the creator
        /// </summary>
        public ulong Owner { get; set; }
    }
}
