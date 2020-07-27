using System.Collections.Generic;
using System.Linq;
using System;
using BlueQueryLibrary.Lang;

namespace BlueQueryLibrary.Blueprints.DefaultBlueprints
{
    /// <summary>
    ///     Represents an advanced blueprint that extends SimpleBlueprint<br/>
    ///     The main purpose of this class is to signify that this blueprint has resources that need to be crafted before or in of themselves<br/>
    ///     Example blueprint: Heavy Auto Turret <- contains a normal auto turret that contains electronics which needs to be crafted before the normal auto turret can be crafted
    /// </summary>
    public class AdvancedBlueprint : SimpleBlueprint
    {
        /// <summary>
        ///     Contains all the resources that themselves need to be crafted before they can exist.
        ///     Crafted 
        /// </summary>
        public SortedList<string, double> CraftedResources { get; set; }

        public override IEnumerable<CalculatedResourceCost> GetResourceCost(Bundle _bundle)
        {
            int amount = (int)_bundle.BundledInformation[BUNDLED_AMOUNT_KEY];
            int calculatedAmount = default;
            // Getting the base resources of this blueprint.
            var resources = base.GetResourceCost(_bundle).ToList();
            // Iterating through the crafted resources that can contain other crafted resources.
            // We want to generate a tree of calculated cost to return.
            for (int i = 0; i < CraftedResources.Count; i++)
            {
                // (resource value * how many) / by how many are produced.
                calculatedAmount = (int)(CraftedResources.Values[i] * amount) / Yield;
                // Appending the new calculated cost                

                // Creating a bundle that contains the amount of the current item in this recursive function that need to be crafted
                Bundle extras = new Bundle();
                extras.BundledInformation.Add(BUNDLED_AMOUNT_KEY, calculatedAmount);

                resources.Add(new CalculatedResourceCost
                {
                    Type = CraftedResources.Keys[i],                    
                    Amount = calculatedAmount,
                    // Getting a list of all the resource's that belong to the crafted blueprints nested inside this blueprint and so on.
                    CalculatedResourceCosts = Data.Blueprints.DefaultBlueprints[CraftedResources.Keys[i]].GetResourceCost(extras)
                });
            }                        
            // Returning the structured data representing the entire tree of calculated cost to be crafted.
            return resources;
        }       
    }
}