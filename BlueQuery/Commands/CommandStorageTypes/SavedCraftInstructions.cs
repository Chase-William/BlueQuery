using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BlueQuery.Commands.CommandStorageTypes
{
    /// <summary>
    ///     Saves data related to the last time ?craft was called;
    /// </summary>
    public class SavedCraftInstructions
    {
        public const int TIMEOUT_TIME = 10000;

        public Timer Timer = new Timer(TIMEOUT_TIME);

        public string[] Keys { get; set; }
        public int? Quantity { get; set; }
        
        public SavedCraftInstructions()
        {
            Timer.Elapsed += delegate
            {
                Keys = null;
                Quantity = null;
                Timer.Stop();
            };
        }
    }
}
