using System.Collections.Generic;
using System.Linq;
using BlueQueryLibrary.ArkBlueprints;

namespace BlueQueryLibrary
{
    /// <summary>
    ///     Provides utility functions for interacting with the database.
    /// </summary>
    public class BlueprintProvider
    {
        private readonly BlueQueryContext blueprintContext;

        public BlueprintProvider(BlueQueryContext _blueprintContext)
        {
            this.blueprintContext = _blueprintContext;
        }

        /// <summary>
        ///     Gets a blueprint by ID
        /// </summary>
        //public Blueprint GetBlueprint(int _id)
        //{
        //    return blueprintContext.Giganotosaurus.Where(e => e.Id == _id).FirstOrDefault();
        //}       

        /// <summary>
        ///     Gets all the giganotosaurus blueprints.
        /// </summary>
        public Giganotosaurus[] GetAllGiganotosaurus()
        {
            return blueprintContext.Saddles.Where(e => e is Giganotosaurus).Cast<Giganotosaurus>().ToArray();
        }
    }
}
