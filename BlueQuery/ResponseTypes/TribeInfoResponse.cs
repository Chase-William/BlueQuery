using BlueQueryLibrary.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueQuery.ResponseTypes
{
    public class TribeInfoResponse : BlueQueryResponse
    {
        public void FormatTribe(Tribe _tribe)
        {
            int index = 0;

            string nextContent = _tribe.NameId;

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
