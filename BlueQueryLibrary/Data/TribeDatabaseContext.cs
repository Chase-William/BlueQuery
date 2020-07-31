using LiteDB;
using System;
using System.Collections.Generic;
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
        ///     @return - true == already exist, false == doesn't exist
        /// </summary>
        /// <param name="_tribeName"> Tribename to be checked </param>
        /// <param name="_existingTribe"> Existing tribe instance, null if no tribe found </param>
        /// <returns> Whether or not the given tribe name is already taken </returns>               
        public bool DoesTribeExist(string _tribeName, out Tribe _existingTribe)
        {
            using var db = new LiteDatabase(BLUEQUERY_DATABASE);
            var col = db.GetCollection<Tribe>(TRIBE_COLLECTION);
            col.EnsureIndex(t => t.NameId);
            var existingTribe = col.FindOne(t => t.NameId == _tribeName.ToLower());
            _existingTribe = existingTribe;
            // Returning the results
            // true == invalid tribe name, already exist
            // false == valid tribe name, doesn't exist
            return existingTribe == null ? false : true;
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
            col.Insert(_tribe);
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
    }
}
