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

        public string Header { get; protected set; }
        public List<string> Content { get; protected set; } = new List<string> { new string(string.Empty) };
        
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

        /// <summary>
        ///     Appends the string content to the Content collection based off a set of conditions.<br/>
        ///     @param - index, Index of Content collection which can be incremented based on circumstance.<br/>
        ///     @param - content, String of content to be appended or added to Content collection, readonly
        /// </summary>
        /// <param name="index"> Index of content collection </param>
        /// <param name="content"> String content to be appended </param>
        protected void AppendContent(ref int index, in string content)
        {
            if ((Content[index].Length + content.Length) <= MESSAGE_LENGTH_LIMIT)
            {
                // Padding right allows the text after to be formatted neatly vertically.
                Content[index] += content;
            }
            else
            {
                // Incrementing the content array and adding the content.
                Content[++index] += content;
            }
        }
    }
}
