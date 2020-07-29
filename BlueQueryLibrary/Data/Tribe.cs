using LiteDB;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace BlueQueryLibrary.Data
{
    public class Tribe
    {
        public string NameId { get; private set; }

        /// <summary>
        ///     Contains discord guilds which have been permitted to access the data located in FolderName variable
        /// </summary>
        public List<ulong> PermittedGuilds { get; private set; }

        public string FolderName { get; set; }
        
    }
}
