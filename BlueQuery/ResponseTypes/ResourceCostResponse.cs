using BlueQuery.Commands.Crafting.CommandStorageTypes;
using BlueQueryLibrary.ArkBlueprints.DefaultBlueprints;
using System.Linq;
using BlueQueryLibrary.Lang;

namespace BlueQuery.ResponseTypes
{
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
        ///     This overload should be called from a Craft command that knows the specific blueprint.<br/>
        ///     
        ///     It takes the string key of the specific blueprint.
        /// </summary>
        public ResourceCostResponse(string _blueprintKey, int _amount)
        {
            PerformResourceCalculations(_blueprintKey, _amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_blueprintKey"></param>
        /// <param name="_amount"></param>
        private void PerformResourceCalculations(string _blueprintKey, int _amount)
        {
            int index = 0;
            // The content;s header
            Content[0] = "#Resources:\n\n";
            // Once we have the key, we can assign the header which declares the item being crafted and the amount.
            Header = $"{_blueprintKey} x {_amount}:";

            Bundle extras = new Bundle();
            extras.BundledInformation.Add(Blueprint.BUNDLED_AMOUNT_KEY, _amount);

            // Getting the calculated cost for the blueprint
            CalculatedResourceCost[] costs = BlueQueryLibrary.Data.Blueprints.DefaultBlueprints[_blueprintKey].GetResourceCost(extras).ToArray();

            // Getting the padding offset needed to format our "x {cost}".
            int offset = costs.Aggregate(string.Empty, (longest, bp) => bp.Type.Length > longest.Length ? bp.Type : longest).Length;

            // Adding the main blueprint's resources to the content.
            for (int i = 0; i < costs.Length; i++)
            {
                // Storing the next content to be added in a local variable for referencing.
                var nextContent = $"{"".PadRight(startOffset)}{costs[i].Type.PadRight(offset)} x {costs[i].Amount}\n";

                // If the content will exceed the max length after adding this to it, added it to the next index of the content array.
                if ((Content[index].Length + nextContent.Length) <= MESSAGE_LENGTH_LIMIT)
                {
                    // Padding right allows the text after to be formatted neatly vertically.
                    Content[index] += nextContent;
                }
                else
                {
                    // Incrementing the content array and adding the content.
                    Content.Add(new string(string.Empty));
                    Content[++index] += nextContent;
                }

                // Adding the nested blueprints to the content.
                
                GetFormattedBlueprintCost(costs[i]);
                startOffset = DEFAULT_START_OFFSET;                
            }            
        }
    }
}
