﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueQueryLibrary.Data
{
    public class TribeDatabaseContext
    {
        public const string BLUEQUERY_DATABASE = "BlueQuery.db";
        public const string TRIBE_COLLECTION = "tribes";

        public static TribeDatabaseContext Provider => new TribeDatabaseContext();

        /// <summary>
        ///     Checks to see if the given tribe name already exist within the BlueQuery.db
        ///     <br/>
        ///     @param - _tribeName, target tribe name<br/>
        ///     @param out - _existingTribe, tribe instance found, null if nothing found<br/>
        ///     Returns boolean indicating whether the tribe exist<br/>
        ///     True - exist<br/>
        ///     False - doesn't exist
        /// </summary>
        /// <param name="_tribeName"> Tribename to be checked </param>
        /// <param name="_existingTribe"> Existing tribe instance, null if no tribe found </param>
        /// <returns> Whether or not the given tribe name is already taken </returns>               
        public bool DoesTribeExist(string _tribeName, out Tribe _existingTribe, out string errorMsg)
        {
            using var db = new LiteDatabase(BLUEQUERY_DATABASE);
            var col = db.GetCollection<Tribe>(TRIBE_COLLECTION);
            col.EnsureIndex(t => t.NameId);
            var existingTribe = col.FindOne(t => t.NameId == _tribeName.ToLower());
            _existingTribe = existingTribe;

            // Returning the results
            // true == invalid tribe name, already exist
            // false == valid tribe name, doesn't exist                        
            if (existingTribe == null)
            {
                errorMsg = "Invalid tribe name given. The tribe name given doesn't exist.";
                return false;
            }
            else
            {
                errorMsg = "A tribe with the same name already exist.";
                return true;
            }
        }

        /// <summary>
        ///     Inserts the given tribe into the BlueQuery.db tribe collection
        ///     <br/>
        ///     @param - _tribe, tribe to be inserted into the database
        /// </summary>
        /// <param name="_tribe"> Tribe to be inserted </param>
        public void InsertTribe(Tribe _tribe)
        {
            using var db = new LiteDatabase(BLUEQUERY_DATABASE);
            var col = db.GetCollection<Tribe>(TRIBE_COLLECTION);
            col.EnsureIndex(t => t.NameId);
            int id = col.Insert(_tribe).AsInt32;
            _tribe.Id = id;
        }

        /// <summary>
        ///     Returns a tribe that matches the given tribename
        ///     <br/>
        ///     @param - _tribeName, target tribe name<br/>
        ///     @return - tribe instance found, null if nothing found
        /// </summary>
        /// <param name="_tribeName"></param>
        /// <returns> Tribe instance found </returns>
        public Tribe GetTribe(string _tribeName)
        {
            using var db = new LiteDatabase(BLUEQUERY_DATABASE);
            var col = db.GetCollection<Tribe>(TRIBE_COLLECTION);
            col.EnsureIndex(t => t.NameId);
            Tribe tribe = null;

            try
            {
                tribe = col.FindOne(t => t.NameId == _tribeName);
            }
            catch { }

            return tribe;
        }
      
        /// <summary>
        ///     Updates a given tribe<br/>
        ///     @param - tribe, tribe to be updated
        /// </summary>
        /// <param name="tribe"> Tribe to be updated </param>
        /// <returns> Indicates success of failure of update </returns>
        public bool UpdateTribe(Tribe tribe)
        {
            using var db = new LiteDatabase(BLUEQUERY_DATABASE);
            var col = db.GetCollection<Tribe>(TRIBE_COLLECTION);
            return col.Update(tribe);                        
        }

        /// <summary>
        ///     Gets all the tribes that the given guild has access to
        /// </summary>
        /// <param name="guildId"> Sender guild </param>
        /// <returns> List of all the tribes the guild has access to </returns>
        public List<Tribe> GetTribesFromGuild(ulong guildId)
        {
            using var db = new LiteDatabase(BLUEQUERY_DATABASE);
            var col = db.GetCollection<Tribe>(TRIBE_COLLECTION);

            return col.FindAll().Where(p => p.PermittedGuilds.Any(p => p.Id.Equals(guildId))).ToList();
        }
    }
}
