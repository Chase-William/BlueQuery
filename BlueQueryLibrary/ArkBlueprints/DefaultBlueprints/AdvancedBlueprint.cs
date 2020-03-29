using System.Collections.Generic;
using System.Linq;
using System;

namespace BlueQueryLibrary.ArkBlueprints.DefaultBlueprints
{
    public class AdvancedBlueprint : Blueprint
    {
        //public float SimpleBullets { get; set; }
        public SortedList<string, double> CraftedResources { get; set; }

        // Adding special implmentation to handle the simplebullets
        public override IEnumerable<CalculatedResourceCost> GetResourceCost(double _amount)
        {
            double calculatedAmount = default;
            // Getting the base resources of this blueprint.
            var resources = base.GetResourceCost(_amount).ToList();
            // Iterating through the crafted resources that can contain other crafted resources.
            // We want to generate a tree of calculated cost to return.
            for (int i = 0; i < CraftedResources.Count; i++)
            {
                // (resource value * how many) / by how many are produced.
                calculatedAmount = (CraftedResources.Values[i] * _amount) / Yield;
                // Appending the new calculated cost
                resources.Add(new CalculatedResourceCost
                {
                    Type = CraftedResources.Keys[i],                    
                    Amount = calculatedAmount,
                    // Getting a list of all the resource's that belong to the crafted blueprints nested inside this blueprint and so on.
                    CalculatedResourceCosts = Data.Blueprints.DefaultBlueprints[CraftedResources.Keys[i]].GetResourceCost(calculatedAmount)
                });
            }                        
            // Returning the structured data representing the entire tree of calculated cost to be crafted.
            return resources;
        }       

        // We need to iterate through all the sub blueprint cost and return them
        /// <summary>
        ///     This function accumulates <aggregates> the cost of the main blueprint, and along with any blueprint that is needed as a resource. 
        /// </summary>
        //public IEnumerable<CalculatedResourceCost> GetNestedBlueprintCost(string _blueprintKey, int _amount)
        //{
        //    // If the blueprint is an advanced blueprint we will need recursion to calculate it's cost
        //    //      This is because advanced blueprints are crafted with items that themselves need to be crafted.
        //    //      Hence we need to determine the cost of the those items as well as the main item itself.
        //    //if (Data.Blueprints.DefaultBlueprints[_blueprintKey] is AdvancedBlueprint advancedBlueprint)
        //    //{
        //    //    return advancedBlueprint.GetResourceCost(_amount);
        //    //}
        //    //// The blueprint is a simple blueprint so we won't need recursion to calculate its resources cost.
        //    //else
        //    //{
        //    //    Blueprint bp = Data.Blueprints.DefaultBlueprints[_blueprintKey];
        //    //    List<CalculatedResourceCost> cost = bp.GetResourceCost(_amount).ToList();
        //    //}

        //    return Data.Blueprints.DefaultBlueprints[_blueprintKey].GetResourceCost(_amount);
        //}
    }
}