using BlueQueryLibrary.Blueprints.DefaultBlueprints;
using BlueQueryLibrary.Lang;
using System.Collections.Generic;

namespace BlueQueryLibrary.Blueprints.Resources
{
    /// <summary>
    ///     Prepresents a resource that doesn't have any children that have children that need to be crafted before hand<br/>
    ///     This class is similar to SimpleBlueprint but contains funtionalities that make it differ<br/>
    ///     The following are examples of ingame resources that fall under this class:<br/>
    ///     - Charcoal<br/>
    ///     - Sparkpowder<br/>
    ///     - Narcotics<br/>
    ///     Generally simple things that use the Chem bench or mortar and pestle
    /// </summary>
    public class SimpleResource : IResourceCalculator
    {
        public const string CRAFT_METHOD = "craftMethod";
        public const byte CHEM_BENCH_CODE = 0;
        public const byte MORTAR_AND_PESTLE_CODE = 1;

        public SortedList<string, SimpleBlueprint> CraftingMethods { get; set; }
    
        public virtual IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle)
        {
            var calculatedResources = new List<CalculatedResourceCost>();

            SimpleBlueprint craftingBp = CraftingMethods.Values[0];            

            for (int i = 0; i < craftingBp.Resources.Count; i++)
            {
                calculatedResources.Add(new CalculatedResourceCost
                {
                    Type = craftingBp.Resources.Keys[i],
                    // (resource value * how many) / by how many are produced.
                    Amount = (craftingBp.Resources.Values[i] * (int)_bundle.BundledInformation[SimpleBlueprint.BUNDLED_AMOUNT_KEY]) / craftingBp.Yield                                        
                });
            }

            return calculatedResources;
        }
    }
}
