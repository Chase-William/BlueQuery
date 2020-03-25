using BlueQueryLibrary.Data;
using BlueQueryLibrary.Crafting;

namespace BlueQueryLibrary
{
    public class BlueQueryLibrary
    {
        /// <summary>
        ///     Contains all the blueprints 
        /// </summary>
        public readonly Blueprints Blueprints = new Blueprints();
        public readonly Craft Craft = new Craft();

        public BlueQueryLibrary() { }
    }
}
