using BlueQueryLibrary.Lang;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQueryLibrary.ArkBlueprints.DefaultBlueprints
{
    public class CraftableResource : IResourceCalculator
    {
        public const string CRAFT_METHOD = "craftMethod";
        public const byte CHEM_BENCH_CODE = 0;
        public const byte MORTAR_AND_PESTLE_CODE = 1;

        public SortedList<string, Blueprint> CraftingMethods { get; set; }
    
        public virtual IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle)
        {
            var calculatedResources = new List<CalculatedResourceCost>();

            Blueprint craftingbp = CraftingMethods.Values[0];            

            for (int i = 0; i < craftingbp.Resources.Count; i++)
            {
                calculatedResources.Add(new CalculatedResourceCost
                {
                    Type = craftingbp.Resources.Keys[i],
                    // (resource value * how many) / by how many are produced.
                    Amount = (craftingbp.Resources.Values[i] * (int)_bundle.BundledInformation[Blueprint.BUNDLED_AMOUNT_KEY]) / craftingbp.Yield                                        
                });
            }

            return calculatedResources;
        }
    }
}
