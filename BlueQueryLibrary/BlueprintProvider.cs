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
        public Blueprint GetBlueprint(int _id)
        {
            return blueprintContext.Blueprints.Where(e => e.Id == _id).FirstOrDefault();
        }       
    }
}
