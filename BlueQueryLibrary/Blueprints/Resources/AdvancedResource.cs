using BlueQueryLibrary.Blueprints.DefaultBlueprints;
using BlueQueryLibrary.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueQueryLibrary.Blueprints.Resources
{
    /// <summary>
    ///     Represents a resource that contains children which have children that need to be crafted before hand<br/>
    ///     This class is similar to AdvancedBlueprint but contains functionalites that make it differ<br/>\
    ///     The following are examples of ingame resources that fall under this class:<br/>
    ///     - Gunpowder
    /// </summary>
    public class AdvancedResource : SimpleResource
    {
        public override IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle)
        {
            int amount = (int)_bundle.BundledInformation[SimpleBlueprint.BUNDLED_AMOUNT_KEY];
            int calculatedAmount = default;
            // Getting the base resources of this blueprint.
            var resources = base.GetResourceCost(_bundle).ToList();

            AdvancedBlueprint blueprint = (AdvancedBlueprint)CraftingMethods.Values[0];

            // Iterating through the crafted resources that can contain other crafted resources.
            // We want to generate a tree of calculated cost to return.
            for (int i = 0; i < blueprint.CraftedResources.Count; i++)
            {
                // (resource value * how many) / by how many are produced.
                calculatedAmount = (int)(blueprint.CraftedResources.Values[i] * amount) / blueprint.Yield;
                // Appending the new calculated cost

                Bundle extras = new Bundle();
                extras.BundledInformation.Add(SimpleBlueprint.BUNDLED_AMOUNT_KEY, calculatedAmount);

                resources.Add(new CalculatedResourceCost
                {
                    Type = blueprint.CraftedResources.Keys[i],
                    Amount = calculatedAmount,
                    // Getting a list of all the resource's that belong to the crafted blueprints nested inside this blueprint and so on.
                    CalculatedResourceCosts = Data.Blueprints.DefaultBlueprints[blueprint.CraftedResources.Keys[i]].GetResourceCost(extras)
                });
            }
            // Returning the structured data representing the entire tree of calculated cost to be crafted.
            return resources;
        }
    }
}
