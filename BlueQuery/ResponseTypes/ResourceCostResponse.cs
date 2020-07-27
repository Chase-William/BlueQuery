using BlueQuery.Commands.Crafting.CommandStorageTypes;
using BlueQueryLibrary.Blueprints.DefaultBlueprints;
using System.Linq;
using BlueQueryLibrary.Lang;
using BlueQueryLibrary.Data;
using BlueQueryLibrary.Blueprints;

namespace BlueQuery.ResponseTypes
{
    /// <summary>
    ///     Class for getting the cost of crafting items. 
    /// </summary>
    class ResourceCostResponse : BlueQueryResponse
    {
        /// <summary>
        ///     This overload should be called from a Selection command.<br/>
        ///     
        ///     It takes the index of a selected blueprint that can be looked up inside of the <see cref="SavedCraftInstructions.Content.Keys"></see>
        /// </summary>
        public ResourceCostResponse(int _blueprintKeyIndex, int _amount)
        {                                  // The content;s header
            Content[0] = "#Resources:\n\n";
            // Getting the blueprint's key (its name)
            string key = SavedCraftInstructions.Content.Keys[--_blueprintKeyIndex];            

            PerformResourceCalculations(key, _amount);
        }

        /// <summary>
        ///     Initiates this class instance and calls PerformResourceCalculations()<br/>
        ///     @param - _blueprintKey, blueprint to be crafted<br/>
        ///     @param - _amount, the amount or quantity of _blueprintKey's value to be crafted
        /// </summary>
        public ResourceCostResponse(string _blueprintKey, int _amount) => PerformResourceCalculations(_blueprintKey, _amount);

        /// <summary>
        ///     Initiates the entire calculation and process<br/>
        ///     @param - _blueprintKey, the blueprint to be crafted<br/>
        ///     @param - _amount, the amount or quantity of _blueprintKey's value to be crafted
        /// </summary>
        private void PerformResourceCalculations(string _blueprintKey, int _amount)
        {
            //int index = 0;
            // Assigning the leading text in the Content body
            Content[0] = "#Resources:\n\n";
            // Assigning the header of our response message to the user's request
            Header = $"{_blueprintKey} x {_amount}:";

            // Adding any extra information the GetResourceCost function will need to take into consideration when calculating the cost
            // We use a bundle because later in development we might want to add other parameters, therefore this creates extra flexability
            Bundle extras = new Bundle();
            extras.BundledInformation.Add(SimpleBlueprint.BUNDLED_AMOUNT_KEY, _amount);

            // Getting the 
            CalculatedResourceCost[] costs = Blueprints.DefaultBlueprints[_blueprintKey].GetResourceCost(extras).ToArray();

            FormatBlueprintCost(costs);
        }
    }
}
