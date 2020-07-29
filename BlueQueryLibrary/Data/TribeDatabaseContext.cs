using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQueryLibrary.Data
{
    public class TribeDatabaseContext
    {
        public TribeDatabaseContext Provider => new TribeDatabaseContext();

        /// <summary>
        ///     Checks to see if the given tribe name already exist within the BlueQuery.db
        /// </summary>
        /// <param name="_tribename"> Tribename to be checked </param>
        /// <returns> Whether or not the given tribe name is already taken </returns>
        public static bool DoesTribeExist(string _tribename)
        {
            // Typed collection
            using (var db = new LiteDatabase("BlueQuery.db"))
            {
                // Get collection instance
                var col = db.GetCollection<Tribe>("tribes");

                // Create an index over the Field name (if it doesn't exist)
                col.EnsureIndex(t => t.NameId);                
                var customer = col.FindOne(t => t.NameId == _tribename);

                // Returning the results
                // true == valid tribe name, doesnt exist yet
                // false == invalid tribe name, already exist
                return customer == null ? true : false;
            }
        }
    }
}
