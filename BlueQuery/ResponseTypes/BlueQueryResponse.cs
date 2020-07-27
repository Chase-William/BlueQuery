using BlueQuery.Commands.Crafting.CommandStorageTypes;
using BlueQueryLibrary.Blueprints;
using BlueQueryLibrary.Blueprints.DefaultBlueprints;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueQuery.ResponseTypes
{
    /// <summary>
    ///     A generic glass for all BlueQuery responses.
    ///     Contains functions for formatting its Content.
    /// </summary>
    public abstract class BlueQueryResponse
    {
        public const int MESSAGE_LENGTH_LIMIT = 1950;
        public const int DEFAULT_START_OFFSET = 3;
        public static int startOffset = DEFAULT_START_OFFSET;

        public string Header { get; protected set; }
        public List<string> Content { get; protected set; } = new List<string> { new string(string.Empty) };


        /// <summary>
        ///     Returns a formatted string with all the information of the crafting requirements.
        /// </summary>  
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

        /// <summary>
        ///     Returns a formatted string of the header.
        /// </summary>
        public string GetFormattedHeader()
        {
            return Formatter.BlockCode(Header, "fix");
        }

        /// <summary>
        ///     Returns a formatted chunk of the content.
        /// </summary>
        public string GetFormattedContent(int _index)
        {
            return Formatter.BlockCode(Content[_index], "py");
        }
    }
}
