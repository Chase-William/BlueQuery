using BlueQuery.Commands.Crafting.CommandStorageTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQuery.ResponseTypes
{
    public class SelectionListingResponse : BlueQueryResponse
    {
        public SelectionListingResponse(string[] _keys, int _amount)
        {
            int index = 0;
            Header = "Blueprint Search Results:";

            // Generate a string of the blueprints found.
            for (int i = 0; i < _keys.Length; i++)
            {
                var nextContent = $"({i + 1}) {_keys[i]}\n";

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
            }

            // Saving the options
            SavedCraftInstructions.Content = new SavedCraftInstructions.SCIContent(_keys, _amount);
        }
    }
}
