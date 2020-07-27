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
        protected void GetFormattedBlueprintCost(CalculatedResourceCost _cost)
        {
            int index = 0;
            string content = string.Empty;

            if (_cost.CalculatedResourceCosts != null)
            {
                // Moving the alignment over so that we can show these resources are inside the blueprint/resource.
                // Ex:
                //  blueprint/resource
                //      Nested Resources
                //      Nested Resource
                startOffset += DEFAULT_START_OFFSET;
                CalculatedResourceCost[] costs = _cost.CalculatedResourceCosts.ToArray();

                // https://www.youtube.com/watch?v=5cg9jv83SMo
                // Using this to find the largest string inside our collection so we will know how to format.
                int offset = costs.Aggregate(string.Empty, (longest, bp) => bp.Type.Length > longest.Length ? bp.Type : longest).Length;

                // Adding the main blueprint's resources to the content.
                for (int i = 0; i < costs.Length; i++)
                {
                    // Storing the next content to be added in a local variable for referencing.
                    var nextContent = $"{"".PadRight(startOffset)}{costs[i].Type.PadRight(offset)} x {costs[i].Amount.ToString("0.0")}\n";

                    // If the content will exceed the max length after adding this to it, added it to the next index of the content array.
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
