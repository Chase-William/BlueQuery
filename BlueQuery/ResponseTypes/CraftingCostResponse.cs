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
    public class CraftingCostResponse : BlueQueryResponse
    {
        public const int DEFAULT_START_OFFSET = 3;
        public static int startOffset = DEFAULT_START_OFFSET;

        /// <summary>
        ///     This overload should be called from a Selection command.<br/>
        ///     
        ///     It takes the index of a selected blueprint that can be looked up inside of the <see cref="SavedCraftInstructions.Content.Keys"></see>
        /// </summary>
        public CraftingCostResponse(int _blueprintKeyIndex, int _amount)
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
        public CraftingCostResponse(string _blueprintKey, int _amount) => PerformResourceCalculations(_blueprintKey, _amount);

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

        protected void FormatBlueprintCost(CalculatedResourceCost[] _costs)
        {
            // Main loop
            for (int i = 0; i < _costs.Length; i++)
            {
                // Moving the alignment over so that we can show these resources are inside the blueprint/resource.
                // Ex:
                //  blueprint/resource
                //      Nested Resources
                //      Nested Resource                             

                // https://www.youtube.com/watch?v=5cg9jv83SMo
                // Using this to find the largest string inside our collection so we will know how to format.
                int offset = _costs.Aggregate(string.Empty, (longest, bp) => bp.Type.Length > longest.Length ? bp.Type : longest).Length;
                Format(_costs[i], startOffset);

                // Checking to see if this current cost has any children cost
                if (_costs[i].CalculatedResourceCosts != null)
                {
                    startOffset += DEFAULT_START_OFFSET;    // Adding to the offset to signify what is about to be calculated is what is needed to craft the current resource
                    FormatBlueprintCost(_costs[i].CalculatedResourceCosts.ToArray());   // Calling this function recursively to record the cost off all possible children cost
                    startOffset -= DEFAULT_START_OFFSET;    // Subtracting from the offset when we return
                }
            }
        }

        private void Format(CalculatedResourceCost _cost, int _offset)
        {
            int index = 0;
            // Storing the next content to be added in a local variable for referencing.
            string nextContent = $"{"".PadRight(startOffset)}{_cost.Type.PadRight(_offset)} x {_cost.Amount.ToString("0.0")}\n";

            if ((Content[index].Length + nextContent.Length) <= MESSAGE_LENGTH_LIMIT)
            {
                // Padding right allows the text after to be formatted neatly vertically.
                Content[index] += nextContent;
            }
            else
            {
                // Incrementing the content array and adding the content.
                Content[++index] += nextContent;
            }
        }
    }
}
